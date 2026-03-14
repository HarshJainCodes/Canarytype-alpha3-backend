namespace Canarytype_alpha3.Data
{

    public interface IUserProfileStats
    {
        public int PersonalBestSpeed { get; set; }

        public int AverageSpeed { get; set; }

        public string ProfileURL { get; set; }
    }

    public class UserProfileStats : IUserProfileStats
    {
        public int PersonalBestSpeed { get; set; }

        public int AverageSpeed { get; set; }

        public string ProfileURL { get; set; }
    }
}
