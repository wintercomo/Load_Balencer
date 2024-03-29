﻿using LoadBalencerClassLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
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
        readonly LoadBalencerServer loadBalencer;
        readonly LoadBalencerViewModel loadBalencerViewModel;
        public MainWindow()
        {
            InitializeComponent();
            loadBalencerViewModel = (LoadBalencerViewModel)loadBalencerSettings.DataContext;
            loadBalencer = new LoadBalencerServer(loadBalencerViewModel);
            serverList.ItemsSource = loadBalencerViewModel.Servers;
            AlgoritmComboBox.ItemsSource = loadBalencerViewModel.Algorithms;
            loadBalencerSettings.DataContext = loadBalencerViewModel;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TcpListener tcplistener = loadBalencer.StartAServer(Int32.Parse(loadBalencerPortBox.Text));
            startLoadBalancerBtn.IsEnabled = false;
            loadBalencerPortBox.IsEnabled = false;
            while (true)
            {
                loadBalencer.StartHealthChecker();
                await Task.Run(async () => await loadBalencer.HandleHttpRequest(tcplistener));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _ = loadBalencer.CheckServerStatusAsync(loadBalencerViewModel.Servers);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.dll");

            loadBalencerViewModel.Algorithms.Clear();
            foreach (var file in files)
            {
                Type[] types;
                try
                {
                    types = Assembly.LoadFrom(file).GetTypes();

                    var interestingTypes =
                    types.Where(t => t.IsClass &&
                                     t.GetInterfaces().Contains(typeof(IAlgorithm)))
                                     .ToList();
                    foreach (var item in interestingTypes)
                    {
                        IAlgorithm instance = (IAlgorithm)Activator.CreateInstance(item);
                        loadBalencerViewModel.Algorithms.Add(instance);
                    }
                    loadBalencerViewModel.SelectedAlgorithm = loadBalencerViewModel.Algorithms.First();
                }
                catch
                {
                    continue;  // Can't load as .NET assembly, so ignore
                }
            }
        }

        private void AddServerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadBalencerViewModel.Servers.Add(new Server(serverURLBox.Text, int.Parse(serverPortBox.Text), "Not Running"));
            }
            catch (Exception)
            {
                MessageBox.Show("Server information was not in the correct format");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            loadBalencerViewModel.Servers.Remove((Server)serverList.SelectedItem);
        }
    }
}
