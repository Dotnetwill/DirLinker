using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;
using DirLinker.Data;
using System.Windows.Threading;

namespace DirLinker.Implemenation
{
    public class LinkerService : ILinkerService
    {
        #region ILinkerService Members

        public FeedbackData GetStatusData(Dispatcher dispatcher)
        {
            return null;
        }

        public void PerformOperation()
        {
        }

        public void CancelOperation()
        {
        }

        #endregion
    }
}
