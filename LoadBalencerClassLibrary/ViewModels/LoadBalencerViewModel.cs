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
        ObservableCollection<IAlgorithm> allAlgorithms;

        public LoadBalencerViewModel()
        {
            this.allAlgorithms = new ObservableCollection<IAlgorithm>() {new HealthyAlgorithm()};
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
            get => allAlgorithms;
            set
            {
                if (SetProperty(ref allAlgorithms, value)) this.allAlgorithms = value;
            }
        }
        private IAlgorithm selectedAlgorithm;
        public IAlgorithm SelectedAlgorithm
        {
            get
            {
                return selectedAlgorithm ?? new HealthyAlgorithm();
            }
            set{
                if (SetProperty(ref selectedAlgorithm, value)) this.selectedAlgorithm = value;
            }
        }

        public ObservableCollection<Server> Servers { get; set; }
    }
}
