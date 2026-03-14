using Canarytype_alpha3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Canarytype_alpha3.Hubs
{
    public class ChatHub : Hub
    {
        // since this is a two player match, this queue can have at most one players at max
        // if there are more than two players then something is wrong
        // as soon as the size of queue gets to two a match is made and both the players are added to a group
        // if a player leaves disconnects then he is removed from the queue.
        private static Queue<TypingLobby> _connections = new Queue<TypingLobby>();
        private static LinkedList<PrivateLobby> _players = new LinkedList<PrivateLobby>();

        private static Random random = new Random();
        private CanaryTypeDBContext _dbContext; // DI

        public static readonly Object _matchLock = new Object();

        public ChatHub(CanaryTypeDBContext canaryTypeDBContext) { 
            _dbContext = canaryTypeDBContext;
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected", "You have connected the hub successfully");
            Console.WriteLine($"Client: {Context.ConnectionId} connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client: {Context.ConnectionId} disconnected");

            // remove the user from the queue if they are disconnected
            if (_connections.Count > 0 )
            {
                _connections.Dequeue();
            }

            // remove the user if they have started a private lobby
            if (_players.Count > 0 )
            {
                PrivateLobby? player = _players.Where((player) =>
                {
                    return player.ConnectionId == Context.ConnectionId;
                }).FirstOrDefault();

                if (  player != null )
                {
                    _players.Remove( player );

                    Console.WriteLine(_players.Count );
                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(TypingLobby typingLobbyClient)
        {
            typingLobbyClient.ConnectionId = Context.ConnectionId;
            _connections.Enqueue(typingLobbyClient);

            Console.WriteLine($"{typingLobbyClient.UserName} has been added to the queue");

            if (_connections.Count > 1)
            {
                TypingLobby player1 = _connections.Dequeue();
                TypingLobby player2 = _connections.Dequeue();

                int player1ID = _dbContext.UsersTable.Where(user => user.UniqueName == player1.UserName).FirstOrDefault().Id;
                int player2ID = _dbContext.UsersTable.Where(user => user.UniqueName == player2.UserName).FirstOrDefault().Id;

                string groupName = GenerateGroupName(10);
                _dbContext.MatchInfoTable.Add(new MatchInfo
                {
                    RoomId = groupName,
                    matchDate = DateTime.Now,
                    Player1Id = player1ID,
                    Player2Id = player2ID,
                });

                await _dbContext.SaveChangesAsync();
                await Groups.AddToGroupAsync(player1.ConnectionId, groupName);
                await Groups.AddToGroupAsync(player2.ConnectionId, groupName);

                await Clients.Group(groupName).SendAsync("MatchStarted", groupName, player1, player2);
            }
        }

        public async Task ClientSubmitResult(string groupName, string playerName, float[] typingResult, float[] rawTypingResult)
        {

            MatchInfo matchInfo;

            UserInfo player;

            lock (_matchLock)
            {
                matchInfo = _dbContext.MatchInfoTable.Where(match => match.RoomId == groupName).FirstOrDefault();
                player = _dbContext.UsersTable.Where(user => user.UniqueName == playerName).FirstOrDefault();

                if (player.Id == matchInfo.Player1Id)
                {
                    matchInfo.Player1Submissions = typingResult;
                    matchInfo.Player1SubmissionsRaw = rawTypingResult;
                }
                else
                {
                    matchInfo.Player2Submissions = typingResult;
                    matchInfo.Player2SubmissionsRaw = rawTypingResult;
                }

                _dbContext.SaveChanges();
            }

            
            if (matchInfo.Player1Submissions.Length > 0 && matchInfo.Player2Submissions.Length > 0)
            {
                // send back data to users
                // we need to send player name as we only store id in MatchInfo table

                string player1Name = _dbContext.UsersTable.Where(user => user.Id == matchInfo.Player1Id).FirstOrDefault().UniqueName;
                string player2Name = _dbContext.UsersTable.Where(user => user.Id == matchInfo.Player2Id).FirstOrDefault().UniqueName;

                MatchInfoResult matchInfoResult = new MatchInfoResult
                {
                    RoomId = matchInfo.RoomId,
                    Player1Name = player1Name,
                    Player2Name = player2Name,
                    matchDate = matchInfo.matchDate,
                    Player1Submissions = matchInfo.Player1Submissions,
                    Player2Submissions = matchInfo.Player2Submissions,
                    Player1SubmissionsRaw = matchInfo.Player1SubmissionsRaw,
                    Player2SubmissionsRaw = matchInfo.Player2SubmissionsRaw,
                };
                await Clients.Group(groupName).SendAsync("MatchResult", matchInfoResult);
            }
        }

        // private lobby stuff
        public async Task CreatePrivateRoom(TypingLobby typingLobby)
        {
            typingLobby.ConnectionId = Context.ConnectionId;
            string roomId = GenerateGroupName(5);

            _players.AddLast(new PrivateLobby
            {
                ConnectionId = typingLobby.ConnectionId,
                UserName = typingLobby.UserName,
                RoomCode = roomId,
            });

            await Clients.Caller.SendAsync("PrivateRoomCode", roomId);
        }

        public async Task JoinPrivateRoomWithCode(PrivateLobby privateLobby)
        {
            privateLobby.ConnectionId = Context.ConnectionId;
            // check if a room with given code exists
            PrivateLobby? potentialPrivateLobby = _players.Where(room => room.RoomCode == privateLobby.RoomCode).FirstOrDefault();

            if (potentialPrivateLobby != null)
            {
                PrivateLobby player1 = potentialPrivateLobby;
                PrivateLobby player2 = privateLobby;

                int player1ID = _dbContext.UsersTable.Where(user => user.UniqueName == player1.UserName).FirstOrDefault().Id;
                int player2ID = _dbContext.UsersTable.Where(user => user.UniqueName == player2.UserName).FirstOrDefault().Id;

                string groupName = privateLobby.RoomCode;
                _dbContext.MatchInfoTable.Add(new MatchInfo
                {
                    RoomId = groupName,
                    matchDate = DateTime.Now,
                    Player1Id = player1ID,
                    Player2Id = player2ID,
                });

                await _dbContext.SaveChangesAsync();
                await Groups.AddToGroupAsync(player1.ConnectionId, groupName);
                await Groups.AddToGroupAsync(player2.ConnectionId, groupName);

                await Clients.Group(groupName).SendAsync("MatchStarted", groupName, player1, player2);
            } else
            {
                await Clients.Caller.SendAsync("PrivateRoomNotExists", false);
            }
        }

        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);
            await Clients.Group(conn.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{conn.UserName} has joined {conn.ChatRoom}");
        }

        public static string GenerateGroupName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
