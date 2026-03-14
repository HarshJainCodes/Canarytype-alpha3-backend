using Canarytype_alpha3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canarytype_alpha3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserStats : ControllerBase
    {
        private CanaryTypeDBContext _canaryTypeDBContext;
        private ILogger<UserStats> _logger;

        public UserStats(CanaryTypeDBContext canaryTypeDBContext, ILogger<UserStats> logger)
        {
            _canaryTypeDBContext = canaryTypeDBContext;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetStat")]
        public async Task<IUserProfileStats> UserProfileStats([FromQuery(Name = "UserName")] string UserName)
        {
            UserInfo user = _canaryTypeDBContext.UsersTable.Where(user => user.UniqueName == UserName).FirstOrDefault();

            if (user == null)
            {
                _logger.LogError("Attempted to fetch ");
                throw new Exception($"User {UserName} does not exist");
            }
            else
            {
                float avarageSpeed = _canaryTypeDBContext.UsersSubmissionsTable.Where(submission => submission.UserInfoId == user.Id).OrderByDescending(x => x.SubmissionDate).Take(10).Select(x => x.typingSpeedPerSecond).ToList().Average(x => x[x.Length - 1]);
                float personalBestSpeed = _canaryTypeDBContext.UsersSubmissionsTable.Where(submission => submission.UserInfoId == user.Id).OrderByDescending(x => x.typingSpeedPerSecond[x.typingSpeedPerSecond.Length - 1]).Select(x => x.typingSpeedPerSecond[x.typingSpeedPerSecond.Length - 1]).FirstOrDefault();

                UserProfileStats userProfileStats = new UserProfileStats
                {
                    AverageSpeed = (int)avarageSpeed,
                    PersonalBestSpeed = (int)personalBestSpeed,
                    ProfileURL = user.ProfilePicURL,
                };

                return userProfileStats;
            }
        }
    }
}
