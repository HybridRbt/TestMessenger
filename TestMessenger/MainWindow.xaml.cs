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
        private CommuManager instance1;
        private CommuManager instance2;

        public MainWindow()
        {
            InitializeComponent();
            Settings.Default.PCPort = "COM2";
            Settings.Default.SBPort = "COM3";
        }

        private void OnTextBox5KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            byte[] msg = new byte[1];
            msg[0] = Convert.ToByte(this.textBox5.Text);
            this.instance1.Send(msg);
        }
    }
}
