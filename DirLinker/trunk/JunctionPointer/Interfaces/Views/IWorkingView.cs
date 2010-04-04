using System;
using System.Windows.Forms;

namespace DirLinker.Interfaces.Views
{
    public interface IWorkingView : IWin32Window
    {
        void AddFeedBack(String feedback);
        Int32 PercentageComplete { set; }
        String CurrentUserTaskText { set; }

        DialogResult AskUser(String message, MessageBoxButtons options);

        DialogResult ShowDialog(IWin32Window owner);
        void Close();
    }
}
