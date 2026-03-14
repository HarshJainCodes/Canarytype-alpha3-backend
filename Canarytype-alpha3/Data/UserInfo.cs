using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Canarytype_alpha3.Data
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string UserName {  get; set; }

        public string UniqueName {  get; set; }
        
        // in case user manually logs in
        public string? Password { get; set; }

        public string UserEmail { get; set; }

        public string ProfilePicURL { get; set; }

        // this is for the individual user submission
        public virtual ICollection<UserSubmissions> UserSubmissions { get; set; }

        // this is for the multiplayer submission
        public virtual ICollection<MatchInfo> MatchesAsPlayer1 { get; set; }

        public virtual ICollection<MatchInfo> MatchesAsPlayer2 { get; set; }
    }
}
