namespace Bank_App
{
    internal class UserDatabase
    {
        private readonly List<User> Users = new List<User>();

        public List<User> GetUsers()
        {
            return Users;
        }

        public bool AddUser(string username, string password)
        {
            if (Users.Any(u => u.Username == username))
            {
                return false;
            }

            Users.Add(new User(username, password));
            return true;
        }

        public bool RemoveUser(string username)
        {
            User? user = GetUserWithUsername(username);
            if (user == null)
            {
                return false;
            }

            return Users.Remove(user);
        }
        public User? GetUserWithUsername(string username)
        {
            return Users.Find(u => u.Username == username);
        }
    }
}
