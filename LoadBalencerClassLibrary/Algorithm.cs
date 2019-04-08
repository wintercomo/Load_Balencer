using System;
using System.Collections.Generic;
using System.Text;

namespace LoadBalencerClassLibrary
{
    public class Algorithm : BindableBase
    {
        string name;
        string value;

        public string Name
        {
            get => name;
            set
            {
                if (SetProperty<string>(ref name, value)) this.name = value;
            }
        }

        public string Value
        {
            get => value;
            set
            {
                if (SetProperty<string>(ref name, value)) this.name = value;
            }
        }
    }
}
