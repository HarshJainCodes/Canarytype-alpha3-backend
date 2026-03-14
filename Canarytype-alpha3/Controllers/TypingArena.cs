using Canarytype_alpha3.Contracts.Login;
using Canarytype_alpha3.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Canarytype_alpha3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TypingArena : ControllerBase
    {
        private CanaryTypeDBContext _canaryTypeDBContext;
        private readonly ILogger<TypingArena> _logger;

        public TypingArena(CanaryTypeDBContext canaryTypeDBContext, ILogger<TypingArena> logger)
        {
            _canaryTypeDBContext = canaryTypeDBContext;
            _logger = logger;
        }


        List<string> randomWords = new List<string>
        {
            "change", "end", "point", "strip", "coincide", "enter", "stage", "edit", "achieve", "impose", "kill", "miss", "mount", "arise", "report"
        };

        [HttpGet]
        [Route("CheckLogin")]
        public async Task<CheckLoginResponse> CheckLogin()
        {
            var token = HttpContext.Request.Cookies["token"];

            var tokenHnadler = new JwtSecurityTokenHandler();
            JwtSecurityToken decodedToken = tokenHnadler.ReadJwtToken(token);
            Claim email = decodedToken.Claims.First(claim => claim.Type == "email");

            UserInfo user = _canaryTypeDBContext.UsersTable.First(user => user.UserEmail == email.Value);

            CheckLoginResponse checkLoginResponse = new CheckLoginResponse
            {
                UserName = user.UserName,
                UniqueUserName = user.UniqueName,
                ProfilePicUrl = user.ProfilePicURL,
            };

            return checkLoginResponse;
        }

        [HttpGet]
        [Route("RandomWords")]
        [AllowAnonymous]
        public ActionResult RandomWords()
        {
            Random rn = new Random();
            List<string> result = new List<string>();
            int index;

            for (int i = 0; i < 50; i++)
            {
                index = rn.Next(0, randomWords.Count - 1);
                result.Add(randomWords[index]);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("Submit")]
        public ActionResult Submit(UserSubmissionDTO userSubmission)
        {
            UserInfo userInfo = _canaryTypeDBContext.UsersTable.Where(x => x.UniqueName == userSubmission.UserName).FirstOrDefault();

            _canaryTypeDBContext.UsersSubmissionsTable.Add(new UserSubmissions
            {
                Score = userSubmission.score,
                SubmissionDate = userSubmission.SubmissionDate,
                UserInfoId = userInfo.Id,
                typingSpeedPerSecond = userSubmission.typingSpeedPerSecond,
                rawTypingSpeedPerSecond = userSubmission.rawTypingSpeedPerSecond,
            });

            _canaryTypeDBContext.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("UserSubmission/{UserName}")]
        public ActionResult GetAllUserSubmissions(string UserName)
        {
            _logger.LogInformation($"Sending submissions for user: {UserName}");

            UserInfo userInfo = _canaryTypeDBContext.UsersTable.Where(user => user.UniqueName == UserName).FirstOrDefault();
            if (userInfo == null)
            {
                _logger.LogError($"User: {UserName} does not exist");
                return BadRequest("User does not exist");
            }
            int userId = userInfo.Id;

            List<UserSubmissions> userSubmissions = _canaryTypeDBContext.UsersSubmissionsTable.Where(userSubmission => userSubmission.UserInfoId == userId).ToList();

            List<UserSubmissionDTO> userSubmissionDTO = userSubmissions.Select(item => new UserSubmissionDTO
            {
                SubmissionDate = item.SubmissionDate,
                score = item.Score,
                UserName = UserName,
                typingSpeedPerSecond = item.typingSpeedPerSecond,
                rawTypingSpeedPerSecond = item.rawTypingSpeedPerSecond,
            }).ToList();

            return Ok(userSubmissionDTO);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("UserOnlineMatches/{UserName}")]
        public ActionResult GetAllOnlineSubmisions(string UserName)
        {
            UserInfo userInfo = _canaryTypeDBContext.UsersTable.Where(user => user.UniqueName == UserName).FirstOrDefault();
            if (userInfo == null)
            {
                return BadRequest("User does not exist");
            }
            int userId = userInfo.Id;

            List<MatchInfo> matchInfos = _canaryTypeDBContext.MatchInfoTable.Where(match => match.Player1Id == userId ||  match.Player2Id == userId).ToList();

            List<MatchInfoResult> matchInfosResult = matchInfos.Select(info => new MatchInfoResult
            {
                matchDate = info.matchDate,
                Player1Name = GetUserNameFromId(info.Player1Id),
                Player2Name = GetUserNameFromId(info.Player2Id),
                RoomId = info.RoomId,
                Player1Submissions = info.Player1Submissions,
                Player1SubmissionsRaw = info.Player1SubmissionsRaw,
                Player2Submissions = info.Player2Submissions,
                Player2SubmissionsRaw = info.Player2SubmissionsRaw,
            }).ToList();

            matchInfosResult.Sort(delegate (MatchInfoResult m1, MatchInfoResult m2)
            {
                return m1.matchDate.CompareTo(m2.matchDate);
            });

            return Ok(matchInfosResult);
        }

        [NonAction]
        public string GetUserNameFromId(int userId)
        {
            return _canaryTypeDBContext.UsersTable.Where(user => user.Id == userId).FirstOrDefault().UniqueName;
        }

        public class UserSubmissionDTO {

            public DateTime SubmissionDate { get; set; }

            public int score {  get; set; }

            public string UserName { get; set; } // foreign key

            public float[] typingSpeedPerSecond { get; set; }

            public float[] rawTypingSpeedPerSecond { get; set; }
        }
    }
}
