using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;
using DirLinker.Interfaces.Views;
using System.Windows.Forms;
using System.Windows.Threading;
using DirLinker.Data;

namespace DirLinker.Controllers
{

    public class WorkerController
    {
        private ILinkerService _linker;
        private IWorkingView _view;

        public WorkerController(ILinkerService linker, IWorkingView view)
        {
            _linker = linker;
            _view = view;
        }

        public void ShowWorker(IWin32Window owner)
        {
            SetupFeedback();
            _view.Show(owner);
            _linker.PerformOperation();
        }

        private void SetupFeedback()
        {
            FeedbackData feedback = _linker.GetStatusData(Dispatcher.CurrentDispatcher);
            _view.Feedback = feedback;
        }
    }
}
