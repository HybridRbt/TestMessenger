using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestMessenger
{
    using messenger;

    using TestMessenger.Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PortName;

        private CommuManager cm;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnComPortNameKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            PortName = "COM";
            PortName += ComPortName.Text;
            cm = new CommuManager(PortName, this);
        }

        private void OnMsgContentKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            var msg = Encoding.ASCII.GetBytes(MsgContent.Text);
            cm.Send(msg);
        }
    }
}
