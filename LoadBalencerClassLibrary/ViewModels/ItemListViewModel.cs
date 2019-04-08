using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LoadBalencerClassLibrary.ViewModels
{
    public class ItemListViewModel : BindableBase
    {
        ObservableCollection<IAlgorithm> allItems = new ObservableCollection<IAlgorithm>() { new First() { Name = "Option 1" }, new First() { Name = "Option 2" } };
        public ObservableCollection<IAlgorithm> AllItems
        {
            get => allItems; set
            {
                if (SetProperty(ref allItems, value)) this.allItems = value;
            }
        }

        private IAlgorithm _currentItem;
        public IAlgorithm CurrentItem
        {
            get { return _currentItem != null ? _currentItem : allItems.First(); }
            set
            {
                if (SetProperty(ref _currentItem, value)) this._currentItem = value;
            }
        }
    }}
