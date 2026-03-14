namespace Canarytype_alpha3.Data
{
    public class UserConnection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string? ChatRoom { get; set; }

        public string? ConnectionId { get; set; }
    }

    public class TypingLobby
    {
        public string UserName { get; set; }
        
        public string? ConnectionId { get; set; }
    }

    public class PrivateLobby
    {
        public string UserName { get; set; }

        public string? ConnectionId { get; set; }

        public string RoomCode { get; set; }

    }
}
