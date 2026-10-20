using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_App.User
{
    internal class User
    {
        public string Username { get; init; }
        private string Password { get; init; }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
