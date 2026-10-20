using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_App
{
    internal class Account
    {
        public decimal Balance { get; protected set; }
        public string Name { get; protected set; }
        public Account(string name)
        {
            Name = name;
        }
    }
}
