using System;
using System.Windows.Forms;
using DirLinker.Interfaces.Views;

namespace DirLinker.Views
{
    public partial class DirLinkerView : Form, ILinkerView
    {
        public DirLinkerView()
        {
            InitializeComponent();
        }

        protected event PathValidater m_ValidatePath;

        protected event PerformLink m_PerformOperation;

        public event PerformLink PerformOperation
        {
            add{ m_PerformOperation += value; }
            remove { m_PerformOperation -= value; }
        }

        public event PathValidater ValidatePath
        {
            add { m_ValidatePath += value; }
            remove { m_ValidatePath -= value;  }
        }

        public string LinkPoint { get; set; }

        public string LinkTo { get; set; }

        public bool CopyBeforeDelete { get; set; }

        public bool OverWriteTargetFiles { get; set; }

 
        /// <summary>
        /// Triggers the ValidatePath event.
        /// </summary>
        public virtual void CallValidatePath(ValidationArgs ea)
        {
            if (m_ValidatePath != null)
                m_ValidatePath(this, ea);
        }
        
        /// <summary>
        /// Triggers the PerformOperation event.
        /// </summary>
        public virtual void CallPerformOperation(EventArgs ea)
        {
            if (m_PerformOperation != null)
                m_PerformOperation(this, ea);
        }

        public Form MainForm
        {
            get { return this; }
        }

        public void Setup()
        {
            BindToFields();
            RegisterHandlers();
        }

        private void BindToFields()
        {
            LinkFrom.DataBindings.Add("Text", this, "LinkTo", false, DataSourceUpdateMode.OnPropertyChanged);
            LinkPointEdit.DataBindings.Add("Text", this, "LinkPoint", false, DataSourceUpdateMode.OnPropertyChanged);
            chkTargetFileOverwrite.DataBindings.Add("Checked", this, "OverWriteTargetFiles", false, DataSourceUpdateMode.OnPropertyChanged);

            CopyToTarget.CheckedChanged += (object sender, EventArgs e) => CopyBeforeDelete = CopyToTarget.Checked;
            //DeleteIt.CheckedChanged 
            
        }


        private void RegisterHandlers()
        {
            RegisterFolderBrowser(BrowsePoint, LinkPointEdit);
            RegisterFolderBrowser(BrowseTarget, LinkFrom);
            
            Go.Click += Go_Click;

            CopyToTarget.CheckedChanged += (sender, e) => chkTargetFileOverwrite.Enabled = !chkTargetFileOverwrite.Enabled;
            CloseBtn.Click += (sender, e) => Application.Exit();

        }

        private void RegisterFolderBrowser(Button button, TextBox textbox)
        {
            button.Tag = textbox;
            button.Click += BrowseForFolder;
        }

        void Go_Click(object sender, EventArgs e)
        {
            if (FormValid())
            {
                CallPerformOperation(new EventArgs());
            }
        }

        private bool FormValid()
        {
            Boolean valid = true;

            valid &= ValidateEditor(LinkPointEdit);
            valid &= ValidateEditor(LinkFrom);

            return valid;
        }

        private Boolean ValidateEditor(TextBox textBox)
        {
            ValidationArgs validEA = new ValidationArgs(textBox.Text);
            CallValidatePath(validEA);

            if (!validEA.Valid)
            {
                ErrorProvider.SetError(textBox, "Please enter a valid path");
                textBox.TextChanged += RealTimeValidation;
            }
            else
            {
                ErrorProvider.SetError(textBox, String.Empty);
                textBox.TextChanged -= RealTimeValidation;
            }

            return validEA.Valid;
        }

        void RealTimeValidation(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            ValidateEditor(textBox);
        }

        void BrowseForFolder(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag is TextBox)
            {
                TextBox textBox = button.Tag as TextBox;

                using (FolderBrowserDialog browser = new FolderBrowserDialog())
                {
                    if(textBox.Text.Trim() != String.Empty)
                    {
                        browser.SelectedPath = textBox.Text;
                    }

                    if(browser.ShowDialog() == DialogResult.OK)
                    {
                        textBox.Text = browser.SelectedPath;
                    }
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Delegate not registered to button or the textbox is specified.");
            }
        }
      
    }
}
