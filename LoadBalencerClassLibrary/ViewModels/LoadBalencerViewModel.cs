using LoadBalencerClassLibrary.Algoritms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerViewModel : BindableBase
    {
        int port = 80;
        ObservableCollection<IAlgorithm> allItems;

        public LoadBalencerViewModel()
        {
            this.allItems = new ObservableCollection<IAlgorithm>() { new First() { Name = "First" }, new HealthyAlgorithm() { Name = "Health Based" } };
        }
        public int Port
        {
            get => port;
            set
            {
                if (SetProperty(ref port, value)) this.port = value;
            }
        }

        public ObservableCollection<IAlgorithm> Algorithms
        {
            get => allItems;
            set
            {
                if (SetProperty(ref allItems, value)) this.allItems = value;
            }
        }
        private IAlgorithm selectedAlgorithm;
        public IAlgorithm SelectedAlgorithm
        {
            get
            {
                return selectedAlgorithm ?? new First();
            }
            set{
                if (SetProperty(ref selectedAlgorithm, value)) this.selectedAlgorithm = value;
            }
        }

        public ObservableCollection<Server> Servers { get; set; }
    }
}
