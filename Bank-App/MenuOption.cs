using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_App
{
    internal class MenuOption
    {
        public string Title { get; init; }
        public Action Function { get; init; }

        public MenuOption(string title, Action function)
        {
            Title = title;
            Function = function;
        }

        public void Execute()
        {
            Function();
        }
    }
}
