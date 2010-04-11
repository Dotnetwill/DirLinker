using System;
using System.Windows.Forms;
using DirLinker.Data;

namespace DirLinker.Interfaces.Views
{
    public interface IWorkingView : IWin32Window
    {
        FeedbackData Feedback { set; } 
        DialogResult AskUser(String message, MessageBoxButtons options);

        void Show(IWin32Window owner);
        void Close();
    }
}
