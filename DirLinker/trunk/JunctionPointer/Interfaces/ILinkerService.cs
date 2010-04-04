using System;
using DirLinker.Data;
using System.Windows.Threading;

namespace DirLinker.Interfaces
{
    public interface ILinkerService
    {
        void SetDispatcher(Dispatcher dispatcher);
        void StartOperation();
        void CancelOperation();
    }
}
