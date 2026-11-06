namespace Bank_App
{
    // Authenticates users and locks access after too many failed attempts.
    internal class Login
    {
        //Allows us to check if user is in UserDatabase
        private UserDatabase _database;
        public static bool isLockedOut { get; private set; } = false;

        public Login(UserDatabase database)
        {
            _database = database;
        }


        public User? TryLogin()
        {
            if (isLockedOut)
            {
                return null;
            }

            User? user = null;

            int usernameAttempts = 0;

            // Username and password have separate attempt limits.
            for (usernameAttempts = 0; usernameAttempts < 3; usernameAttempts++)
            {
                Message.WriteLine(new Message("Ange användarnamn: ", MessageType.INPUT));
                string username = UserInput.GetString();

                user = _database.GetUserWithUsername(username);
                if (user == null)
                {
                    Message.WriteLine(new Message($"Användare hittades inte. Försök kvar: {2 - usernameAttempts}", MessageType.ERROR));
                    continue;
                }

                break;
            }
            if (usernameAttempts >= 3)
            {
                Message.WriteLine(new Message("Du har skrivit fel för många gånger. Försök igen senare.", MessageType.ERROR));
                isLockedOut = true;
                return null;
            }

            int passwordAttempts = 0;
            while (passwordAttempts < 3)
            {
                Message.WriteLine(new Message ("Lösenord: ", MessageType.INPUT));
                string password = UserInput.GetString();

                if (user != null && user.CheckPassword(password))
                {
                    Console.WriteLine();
                    return user;
                }

                passwordAttempts++;
                Message.WriteLine(new Message($"Fel lösenord! Försök kvar: {3 - passwordAttempts}", MessageType.ERROR));
            }
            Message.WriteLine(new Message("Du har skrivit fel för många gånger. Försök igen senare.", MessageType.ERROR));
            isLockedOut = true;

            return null;
        }
    }
}
