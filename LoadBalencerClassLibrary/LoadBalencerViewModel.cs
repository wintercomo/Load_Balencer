using LoadBalencerClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LoadBalencerClassLibrary
{
    public class LoadBalencerViewModel : BindableBase
    {
        int port = 8080;
        ItemListViewModel model = new ItemListViewModel();
        ObservableCollection<IAlgorithm> algorithms;
        public LoadBalencerViewModel()
        {
            this.algorithms = model.AllItems;
        }

        public int Port
        {
            get => port;
            set
            {
                if (SetProperty(ref port, value)) this.port = value;
            }
        }
        public IAlgorithm GetCurrentItem()
        {
            return model.CurrentItem;
        }

        public ObservableCollection<IAlgorithm> Algorithms
        {
            get => algorithms;
            set
            {
                if (SetProperty(ref algorithms, value)) this.algorithms = value;
            }
        }
    }
}
