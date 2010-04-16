using System;
using DirLinker.Data;


namespace DirLinker.Interfaces.Views
{
    public class ValidationArgs : EventArgs
    {
        public ValidationArgs(String path)
        {
            PathToValidate = path;
            Valid = true;
            
        }
        public String ErrorMessge { get; set; }
        public Boolean Valid { get; set; }
        public String PathToValidate { get; set; }
    }
    
    public delegate void PerformLink(Object sender, EventArgs e);
    public delegate void PathValidater(Object sender, ValidationArgs e);

    public interface ILinkerView
    {
        System.Windows.Forms.Form MainForm { get; }

        void SetOperationData(LinkOperationData data);
        
        event PathValidater ValidatePath;
        event PerformLink PerformOperation;
    }
}
