namespace Canarytype_alpha3.Data
{
    public class MatchInfo
    {
        public string RoomId { get; set; }  // this will identify each match uniquely.

        public int Player1Id { get; set; }  // foreign key for player1

        public int Player2Id { get; set; }  // foreign key for player2

        public DateTime matchDate { get; set; }

        public float[]? Player1Submissions { get; set; } = [];

        public float[]? Player2Submissions { get; set; } = [];

        public float[]? Player1SubmissionsRaw {  get; set; } = [];

        public float[]? Player2SubmissionsRaw { get; set; } = [];

        public virtual UserInfo Player1 { get; set; }

        public virtual UserInfo Player2 { get; set; }
    }

    public class  MatchInfoResult
    {
        public string RoomId { get; set; }

        public string Player1Name {  get; set; }

        public string Player2Name { get; set; }

        public DateTime matchDate { get; set; }

        public float[]? Player1Submissions { get; set; } = [];

        public float[]? Player2Submissions { get; set; } = [];

        public float[]? Player1SubmissionsRaw { get; set; } = [];

        public float[]? Player2SubmissionsRaw { get; set; } = [];
    }
}
