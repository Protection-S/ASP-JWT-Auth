namespace MicroServicesProj.Hubs
{
    public static class UserStatusStore
    {
        private static readonly Dictionary<string, bool> _userStatuses = new Dictionary<string, bool>();

        public static void SetUserStatus(string username, bool isOnline)
        {
            _userStatuses[username] = isOnline;
        }

        public static bool GetUserStatus(string username)
        {
            return _userStatuses.ContainsKey(username) && _userStatuses[username];
        }

        public static Dictionary<string, bool> GetAllUserStatuses()
        {
            return _userStatuses;
        }
    }

}
