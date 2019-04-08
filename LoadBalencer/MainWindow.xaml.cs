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
        readonly ObservableCollection<Server> allServers;
        private bool handle = true;
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
            loadBalencer = new LoadBalencerServer(allServers, model);
            //AlgoritmComboBox.DataContext = model.Dictionary;
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
            _ = loadBalencer.CheckServerStatusAsync(allServers);
        }

        private void AlgoritmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine(AlgoritmComboBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last());
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }

        private void AlgoritmComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }

        private void Handle()
        {
            switch (AlgoritmComboBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "1":
                    //Handle for the first combobox
                    break;
                case "2":
                    //Handle for the second combobox
                    break;
                case "3":
                    //Handle for the third combobox
                    break;
            }
        }
    }
}
