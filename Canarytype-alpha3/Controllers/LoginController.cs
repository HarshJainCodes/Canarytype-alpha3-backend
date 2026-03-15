using Azure;
using Azure.Core;
//using CanaryEmailsService.Contracts;
using Canarytype_alpha3.Data;
using Canarytype_alpha3.Utils;
using Google.Apis.Auth;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Canarytype_alpha3.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api")]
    public class LoginController : Controller
    {
        private IConfiguration _configuration;

        private CanaryTypeDBContext _canaryTypeDBContext;
        //private readonly IBusControl _busControl;

        public LoginController(IConfiguration configuration, CanaryTypeDBContext canaryTypeDBContext)
        {
            _configuration = configuration;
            _canaryTypeDBContext = canaryTypeDBContext;
            //_busControl = busControl;
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ManualRegister(ManualLoginRequest request)
        {
            if (_canaryTypeDBContext.UsersTable.Where(user => user.UserEmail == request.Email).Count() == 1)
            {
                return Conflict("User already Exists with this email");
            }

            await _canaryTypeDBContext.UsersTable.AddAsync(new UserInfo
            {
                ProfilePicURL = "",
                UserEmail = request.Email,
                UserName = request.UserName,
                Password = request.Password,
            });

            await _canaryTypeDBContext.SaveChangesAsync();

            GenerateJWTToken(new JWTInputs
            {
                Name = request.UserName,
                Email = request.Email,
            }, HttpContext);

            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ManualLogin(ManualLoginRequest request)
        {
            if (_canaryTypeDBContext.UsersTable.Where(user => user.UserEmail == request.Email && user.UserName == request.UserName).Count() == 0)
            {
                return NotFound("Username or Email is incorrect");
            }

            UserInfo user = _canaryTypeDBContext.UsersTable.Where(user => user.UserName == request.UserName && user.UserEmail == request.Email).FirstOrDefault();

            if (user.Password != request.Password)
            {
                return BadRequest("Invalid Username or Password");
            }

            GenerateJWTToken(new JWTInputs
            {
                Name = request.UserName,
                Email = request.Email,
            }, HttpContext);

            return Ok();
        }


        [Route("GoogleLogin")]
        [HttpPost]
        public async Task<ActionResult> HandleLoginWithGoogle(HandleWithGoogleRequest request)
        {
            var payload = await ValidateGoogleTokenV2(request.idToken);
            string userName = payload.GetValue("name").ToString();
            string userEmail = payload.GetValue("email").ToString();
            string pictureURL = payload.GetValue("picture").ToString();

            string uniqueUserName = string.Join("", userName.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries)) + HelperFunctions.GenerateGroupName(3);
            UserInfo maybeUser = _canaryTypeDBContext.UsersTable.Where(user => user.UserEmail == userEmail).FirstOrDefault();

            if (maybeUser != null)
            {
                var token = GenerateJWTToken(new JWTInputs
                {
                    Name = userName,
                    Email = userEmail,
                }, HttpContext);

                return Ok(new LoginResponse
                {
                    UserName = userName,
                    UniqueUserName = maybeUser.UniqueName,
                    IsError = false,
                    Token = token,
                });
            }
            else
            {
                // user is first time loggin through google
                _canaryTypeDBContext.UsersTable.Add(new UserInfo
                {
                    UserName = userName,
                    UserEmail = userEmail,
                    ProfilePicURL = pictureURL,
                    UniqueName = uniqueUserName
                });

                await _canaryTypeDBContext.SaveChangesAsync();

                //await _busControl.Publish<ISendEmailMessage>(new SendEmailMessage
                //{
                //    ToEmail = userEmail,
                //    Body = "Welcome to CanaryType",
                //    Subject = "Welcome"
                //});

                GenerateJWTToken(new JWTInputs
                {
                    Name = userName,
                    Email = userEmail,
                }, HttpContext);

                return Ok(new RegisterResponse
                {
                    UserName = userName,
                    UniqueUserName = uniqueUserName,
                });
            }
        }

        [NonAction]
        private bool isExistingUser(LoginRequest userInfo)
        {
            return _canaryTypeDBContext.UsersTable.Where(x => x.UserName == userInfo.UserName).Count() > 0;
        }

        // use this if you are using button rendered by google
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { "614723879813-ucnacak95ele14f5fjbqop6f3d8l9a3q.apps.googleusercontent.com" }
            };

            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return payload;
        }

        // use this if you are using custom button provided by you
        private async Task<JObject> ValidateGoogleTokenV2(string access_token)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Cannot reach googleapis");
                }

                // Parse the JSON response to extract user info
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var userInfo = JObject.Parse(jsonResponse);

                return userInfo;
            }
        }

        [NonAction]
        public string GenerateJWTToken(JWTInputs jwtInput, HttpContext httpContext)
        {
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
            var tokenHandler = new JwtSecurityTokenHandler();

            Claim roleClaim = new Claim(ClaimTypes.Role, "User");

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, jwtInput.Name),
                    new Claim(ClaimTypes.Email, jwtInput.Email),
                    roleClaim
                }),
                Expires = DateTime.Now.AddMinutes(10),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var generatedToken = tokenHandler.WriteToken(token);

            httpContext.Response.Cookies.Append("token", generatedToken, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10),
                HttpOnly = true,
                Secure = false,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            return generatedToken;
        }
    }

    public class RegisterError
    {
        public string Message;
    }

    public class RegisterResponse
    {
        public string UserName { get; set; }

        public string UniqueUserName { get; set; }

        public string? Token { get; set; }
    }

    public class JWTInputs
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }

        public string? Password { get; set; }
    }

    public class LoginResponse
    {
        public string? UserName { get; set; }

        public string? UniqueUserName { get; set; }

        public string? Token { get; set; }

        public bool IsError { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class HandleWithGoogleRequest
    {
        public string idToken { get; set; }
    }

    public class ManualLoginRequest
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
