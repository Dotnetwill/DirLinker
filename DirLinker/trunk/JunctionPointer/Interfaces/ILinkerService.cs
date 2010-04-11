using System;
using DirLinker.Data;
using System.Windows.Threading;

namespace DirLinker.Interfaces
{
    public interface ILinkerService
    {
        FeedbackData GetStatusData(Dispatcher dispatcher);
        void PerformOperation();
        void CancelOperation();
    }
}
