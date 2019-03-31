using LoadBalencerClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoadBalencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoadBalencerServer loadBalencer;
        LoadBalencerViewModel model;
        TcpClient tcpClient;
        StreamReader streamReader;
        public MainWindow()
        {
            InitializeComponent();
            model = new LoadBalencerViewModel();
            loadBalencer = new LoadBalencerServer();
            streamReader = new StreamReader();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpListener tcplistener = loadBalencer.StartAServer(Int32.Parse(PortBox.Text));
            int bufferSize = 1;
            //Task t = Task.Run(() => loadBalencer.HandleHttpRequest(tcplistener, bufferSize));
            await loadBalencer.HandleHttpRequest(tcplistener, bufferSize);
            //loadBalencer.Start(Int32.Parse(PortBox.Text));
        }

        
    }
}
