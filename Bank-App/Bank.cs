using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_App
{
    internal static class Bank
    {
        public static UserDatabase UserDatabase { get; } = new UserDatabase();
    }
}
