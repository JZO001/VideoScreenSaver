using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace VideoScreenSaver
{

    public class ListViewItemData : INotifyPropertyChanged
    {

        private string mName = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public ListViewItemData()
        {
        }

        public virtual string Name
        {
            get { return mName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                mName = value;
                RaisePropertyChangedEvent("Name");
            }
        }

        public virtual void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
