using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirLinker.Interfaces.Views
{
    public interface IWorkingView : IWin32Window
    {
        String FeedbackTitle { set; }
        String FeedbackCaption { set; }
        DialogResult AskUser(String message, MessageBoxButtons options);

        DialogResult ShowDialog(IWin32Window owner);
        void Close();
    }
}
