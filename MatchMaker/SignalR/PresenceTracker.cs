namespace MatchMaker.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new();

        public Task<List<string>> GetOnlineUsers()
        {
            List<string> onlineUsers = new List<string>();

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key)
                    .Select(k => k.Key)
                    .ToList();
            }
            return Task.FromResult(onlineUsers);
        }
        public Task<bool> UserConnected(string username, string connectionId)
        {
            var isOnline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, [connectionId]);
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);

        }
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            var isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username))
                {
                    return Task.FromResult(isOffline);
                }

                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }

            }
            return Task.FromResult(isOffline);
        }

        public static Task<List<string>> GetConnectionsForUSer(string username)
        {
            List<string> connectionsIds = new List<string>();
            if (OnlineUsers.TryGetValue(username, out var connections))
            {
                lock (connections)
                {
                    connectionsIds = connections.ToList();
                }
            }
            //else
            //{
            //    connectionsIds = new List<string>();
            //}
            return Task.FromResult(connectionsIds);
        }


    }
}
