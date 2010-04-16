using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirLinker.Interfaces.Views;
using DirLinker.Data;

namespace DirLinker.Views
{
    public partial class ProgressView : Form, IWorkingView
    {
        private FeedbackData _data;

        public ProgressView()
        {
            InitializeComponent();
        }


        public FeedbackData Feedback
        {
            set 
            { 
                _data = value; 
                BindToData(); 
            }
        }

        private void BindToData()
        {
            progressBar1.DataBindings.Add("Value", _data, "PercentageComplete", false, DataSourceUpdateMode.OnPropertyChanged);
            _data.PropertyChanged += (s, ea) =>
                                        {
                                            if (ea.PropertyName.Equals("Message"))
                                                textBox1.AppendText(_data.Message + Environment.NewLine);
                                        };

            _data.AskUser = (m) =>  MessageBox.Show(this, m.Message, "DirLinker", m.ResponseOptions);
               
        }

        public IntPtr Handle
        {
            get { return this.Handle; }
        }

        public string CancelButtonText
        {
            set { cancelBtn.Text = value; }
        }

        public event EventHandler CancelPress
        {
            add { cancelBtn.Click += value; }
            remove { cancelBtn.Click -= value; }
        }

    }
}
