using System;
using System.ComponentModel;

namespace JunctionPointer.Interfaces
{
    public interface IBackgroundWorker
    {
        event DoWorkEventHandler DoWork;
        event RunWorkerCompletedEventHandler RunWorkerCompleted;
        void RunWorkerAsync();
    }
}
