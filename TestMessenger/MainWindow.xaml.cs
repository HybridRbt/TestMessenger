using System.Text;
using System.Windows;
using System.Windows.Input;

namespace TestMessenger
{
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

        private void OnAskSensorBtnClick(object sender, RoutedEventArgs e)
        {
            cm.StartAskSensor();
        }

        private void OnStopAskSensorBtnClick(object sender, RoutedEventArgs e)
        {
            cm.StopAskSensor();
        }
    }
}
