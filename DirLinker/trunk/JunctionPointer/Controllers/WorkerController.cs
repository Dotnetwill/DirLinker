using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;
using DirLinker.Interfaces.Views;
using System.Windows.Forms;
using System.Windows.Threading;

namespace DirLinker.Controllers
{

    public class WorkerController
    {
        private ILinkerService _linker;

        public WorkerController(ILinkerService linker, IWorkingView view)
        {
            _linker = linker;
        }

        public void ShowWorker(IWin32Window owner)
        {
            _linker.SetDispatcher(Dispatcher.CurrentDispatcher);
        }

    }
}
