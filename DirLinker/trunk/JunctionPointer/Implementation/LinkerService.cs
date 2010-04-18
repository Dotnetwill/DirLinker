using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;
using DirLinker.Data;
using System.Windows.Threading;
using DirLinker.Commands;
using System.Threading;

namespace DirLinker.Implementation
{
    public class LinkerService : ILinkerService
    {
        private ITransactionalCommandRunner _commandRunner;
        private Action _completeCallBack;
        private FeedbackData _feedback;
        private ICommandDiscovery _commandDiscovery;
        private IFolderFactoryForPath _folderFactory;
        private ThreadMessengerFactory _messengerFactory;
        private LinkOperationData _operationData;

        public LinkerService(ICommandDiscovery commandDiscovery, ITransactionalCommandRunner runner, IFolderFactoryForPath folderFactory, ThreadMessengerFactory messengerFactory)
        {
            _commandDiscovery = commandDiscovery;
            _commandRunner = runner;
            _folderFactory = folderFactory;
            _messengerFactory = messengerFactory;
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

            RunCommandRunner();
        }

        
        public void CancelOperation()
        {
            _commandRunner.RequestCancel();
        }

        public void SetOperationData(LinkOperationData linkData)
        {
            _operationData = linkData;
        }

        private void QueueCommands()
        {
            IFolder linkTo = _folderFactory(_operationData.LinkTo);
            IFolder linkFrom = _folderFactory(_operationData.CreateLinkAt);

            UpdateFeedBack("Building Task List");

            var commandList = _commandDiscovery.GetCommandListForFolderTask(
                            linkTo, 
                            linkFrom, 
                            overwriteTargetFiles: _operationData.OverwriteExistingFiles, 
                            copyBeforeDelete: _operationData.CopyBeforeDelete);

            _commandRunner.QueueRange(commandList);
    
        }

        private void RunCommandRunner()
        {
            _commandRunner.WorkCompleted += (wr) =>
            {
                UpstatusFromReport(wr);
                if (_completeCallBack != null)
                {
                    _completeCallBack();
                }
            };

            _commandRunner.RunAsync(_messengerFactory(Dispatcher.CurrentDispatcher, _feedback));

        }


        private void UpstatusFromReport(WorkReport wr)
        {
            switch (wr.Status)
            {
                case WorkStatus.Success:
                    UpdateFeedBack("Completed successfully");
                    break;
                case WorkStatus.UserCancelled:
                    UpdateFeedBack("User cancelled");
                    break;
                case WorkStatus.CommandFailWithException:
                    UpdateFeedBack("Operation failed");
                    break;
                case WorkStatus.UndoFailWithException:
                    UpdateFeedBack("Undo failed");
                    break;
            }
        }

        private void UpdateFeedBack(String message)
        {
            if (_feedback != null)
            {
                _feedback.Message = message;
            }
        }

        public Action OperationComplete
        {
            set { _completeCallBack = value; }
            get
            {
                return _completeCallBack;
            }
        }

    }
}
