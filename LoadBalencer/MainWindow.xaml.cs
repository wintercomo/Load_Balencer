using LoadBalencerClassLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        readonly ObservableCollection<Server> allServers;
        public MainWindow()
        {
            InitializeComponent();
            model = new LoadBalencerViewModel();
            allServers = new ObservableCollection<Server>
            {
                new Server("localhost", 9001, "Normal"),
                new Server("localhost", 9002, "Normal"),
                new Server("localhost", 9003, "Normal")
            };
            serverList.ItemsSource = allServers;
            loadBalencer = new LoadBalencerServer(allServers, model.Algorithms);
            //AlgoritmComboBox.DataContext = model.Dictionary;
            streamReader = new StreamReader();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpListener tcplistener = loadBalencer.StartAServer(Int32.Parse(PortBox.Text));
            int bufferSize = 2024;
            //Task t = Task.Run(() => loadBalencer.HandleHttpRequest(tcplistener, bufferSize));
            while (true)
            {
                await Task.Run(async () => await loadBalencer.HandleHttpRequest(tcplistener, bufferSize));
                
            }
            //loadBalencer.Start(Int32.Parse(PortBox.Text));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            loadBalencer.CheckServerStatusAsync(allServers);
        }
    }
}
