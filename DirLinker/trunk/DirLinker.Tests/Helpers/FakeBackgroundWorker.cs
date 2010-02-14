using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;
using System.ComponentModel;

namespace DirLinker.Tests.Helpers
{
    class FakeBackgroundWorker : IBackgroundWorker
    {
        public event System.ComponentModel.DoWorkEventHandler DoWork;

        public event System.ComponentModel.RunWorkerCompletedEventHandler RunWorkerCompleted;

        public void RunWorkerAsync()
        {
            DoWork(this, new DoWorkEventArgs(null));
        }

    }
}
