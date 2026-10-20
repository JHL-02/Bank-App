namespace Bank_App
{
    internal class UserDatabase
    {
        private readonly List<User> _users = new List<User>();
        public IReadOnlyCollection<User> Accounts => _users.AsReadOnly();
        public bool AddUser(string username, string password)
        {
            if (_users.Any(u => u.Username == username))
            {
                return false;
            }

            _users.Add(new User(username, password));
            return true;
        }

        public bool RemoveUser(string username)
        {
            User? user = GetUserWithUsername(username);
            if (user == null)
            {
                return false;
            }

            return _users.Remove(user);
        }
        public User? GetUserWithUsername(string username)
        {
            return _users.Find(u => u.Username == username);
        }
    }
}
