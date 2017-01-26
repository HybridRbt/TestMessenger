using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TestMessenger
{
    public class Player
    {
        private TextBox myTb;

        public Player(TextBox tb)
        {
            myTb = tb;
        }

        public void Display(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => myTb.Text += msg));
        }

    }
}
