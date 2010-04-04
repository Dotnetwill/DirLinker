using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DirLinker.Data
{
    public class FeedbackData : INotifyPropertyChanged
    {
        private String _Message;
        private Int32 _PercentageComplete;
        private PropertyChangedEventHandler _propertyChanged;

        public String Message
        {
            get { return _Message; }
            set 
            { 
                _Message = value;
                NotifyChange("Message");
            }
        }

        public Int32 PercentageComplete
        {
            get
            {
                return _PercentageComplete;
            }
            set
            {
                _PercentageComplete = value;
                NotifyChange("PercentageComplete");
            }
        }

        private void NotifyChange(String propName)
        {
            PropertyChangedEventHandler handler = _propertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged -= value; }
            remove { _propertyChanged -= value; }
        }

    }
}
