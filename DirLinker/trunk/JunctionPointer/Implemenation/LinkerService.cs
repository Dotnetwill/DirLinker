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
        private FeedbackData _feedback;

        public FeedbackData GetStatusData(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            if (_feedback == null)
            {
                _feedback = new FeedbackData();
            }

            return _feedback;
        }

        public void PerformOperation()
        {
        }

        public void CancelOperation()
        {
        }

        public void SetOperationData(LinkOperationData linkData)
        {
            throw new NotImplementedException();
        }
    }
}
