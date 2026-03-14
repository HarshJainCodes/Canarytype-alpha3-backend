namespace Canarytype_alpha3.Data
{
    public class UserSubmissions
    {
        public int Id { get; set; }

        public DateTime SubmissionDate { get; set; }

        public int Score { get; set; }

        public int UserInfoId { get; set; } // foreign key

        public float[] typingSpeedPerSecond { get; set; }

        public float[] rawTypingSpeedPerSecond { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}
