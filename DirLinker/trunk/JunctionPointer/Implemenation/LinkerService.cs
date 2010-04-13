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
        private ICommandDiscovery _commandDiscovery;
        private IFolderFactoryForPath _folderFactory;
        private LinkOperationData _operationData;

        public LinkerService(ICommandDiscovery commandDiscovery, IFolderFactoryForPath folderFactory)
        {
            _commandDiscovery = commandDiscovery;
            _folderFactory = folderFactory;
        }

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
            QueueCommands();
        }

        
        public void CancelOperation()
        {
        }

        public void SetOperationData(LinkOperationData linkData)
        {
            _operationData = linkData;
        }

        private void QueueCommands()
        {
            IFolder linkTo = _folderFactory(_operationData.LinkTo);
            IFolder linkFrom = _folderFactory(_operationData.CreateLinkAt);

            _commandDiscovery.GetCommandListForTask(
                            linkTo, 
                            linkFrom, 
                            overwriteTargetFiles: _operationData.OverwriteExistingFiles, 
                            copyBeforeDelete: _operationData.CopyBeforeDelete); 
        }

    }
}
