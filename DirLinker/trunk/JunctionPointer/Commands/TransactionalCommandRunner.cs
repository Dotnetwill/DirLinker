using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using DirLinker.Interfaces;
using DirLinker.Implementation;
using System.Windows.Forms;
using DirLinker.Data;

namespace DirLinker.Commands
{
     /// <summary>
     /// This is delegate called when an operation has completed
     /// </summary>
     /// <param name="report">A summary of the work performed</param>
    public delegate void WorkCompletedCallBack(WorkReport report);
    /// <summary>
    /// 
    /// </summary>
    public class TransactionalCommandRunner :  ITransactionalCommandRunner
    {

      
        private Stack<ICommand> _undoStack;
        private ThreadSafeQueue<ICommand> _commandQueue;
        private IBackgroundWorker _bgWorker;

        private Boolean _cancelRequested;

        private WorkCompletedCallBack _workCompleted;
        private WorkReportGenerator _workReportCreator;

        public TransactionalCommandRunner(IBackgroundWorker backgroundWorker, ThreadSafeQueue<ICommand> queue)
        {
            _commandQueue = queue;
            _bgWorker = backgroundWorker;
            _undoStack = new Stack<ICommand>();
            _workReportCreator = new WorkReportGenerator();

            _cancelRequested = false;
        }

        /// <summary>
        /// This event is raised when all the work has been completed.  It will return a 
        /// a complete work report including any exceptions. 
        /// </summary>
        public event WorkCompletedCallBack WorkCompleted
        {
            add { _workCompleted += value; }
            remove { _workCompleted -= value; }
        }

        /// <summary>
        /// Returns the current number of commands in the queue
        /// </summary>
        public Int32 CommandQueueCount
        {
            get
            {
                return _commandQueue.Count;
            }
        }

        /// <summary>
        /// Adds a command to the queue to be run
        /// </summary>
        /// <param name="command">a class that implements ICommand</param>
        public void QueueCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command", "command is null.");
            }

            _commandQueue.Enqueue(command);
        }

        public void QueueRange(List<ICommand> commandList)
        {
            commandList.ForEach(c => QueueCommand(c));
        }

        /// <summary>
        /// Starts a background thread and runs the current command queue.
        /// When finished The WorkCompleted event will be raised.
        /// </summary>
        public void RunAsync(IMessenger messenger)
        {
            _bgWorker.DoWork += (sender, args) => RunCommandQueue(args.Argument as IMessenger);

            _bgWorker.RunWorkerCompleted += NotifyWorkCompleted;
            _bgWorker.RunWorkerAsync(messenger); 
        }


        /// <summary>
        /// Requests the work is cancelled.  The cancel flag is only checked between running commands.
        /// </summary>
        /// 
        public void RequestCancel()
        {
            _cancelRequested = true;
            _workReportCreator.UserCancelledRequested();
        }
        
        private void RunCommandQueue(IMessenger messenger)
        {
            try
            {
                foreach (ICommand command in _commandQueue.ProcessQueue())
                {
                    if (_cancelRequested)
                    {
                        ProcessUndoStack(messenger);
                        break;
                    }

                    messenger.StatusUpdate(command.UserFeedback, 0);
                    
                    command.Execute();
                    _undoStack.Push(command);
                }
            }
            catch(Exception ex)
            {
                _workReportCreator.ProcessException(ex, WorkAction.Execute);

                messenger.StatusUpdate("An Error occured, attemping to rollback changes", 0);

                ProcessUndoStack(messenger); 
            }
        }

        private void ProcessUndoStack(IMessenger messenger)
        {
            try
            {
                while (_undoStack.Count > 0)
                {
                    ICommand command = _undoStack.Pop();

                    messenger.StatusUpdate(command.UserFeedback, 0);

                    command.Undo();
                }
            }
            catch (Exception ex)
            {
                _workReportCreator.ProcessException(ex, WorkAction.Undo);

                messenger.StatusUpdate("An Error occured when rolling back changes", 0);
            }
        }


        private void NotifyWorkCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            WorkCompletedCallBack workCompletedCopy = _workCompleted;
            if (workCompletedCopy != null)
            {
                workCompletedCopy(_workReportCreator.GenerateReport());
            }

            _bgWorker.RunWorkerCompleted -= NotifyWorkCompleted;
            
            //Reset the undo stack in case we are called again without being disposed
            //We don't want to clear the command queue in case the user has been queuing 
            //up outside of this run
            _undoStack.Clear();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void StatusUpdateDelegate(String message, Int32 percentcomplete)
        {
            CurrentStatus = message;
            PercentageComplete = percentcomplete;
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("CurrentStatus"));
                handler(this, new PropertyChangedEventArgs("PercentageComplete"));
            }
        }
        #endregion 
    }
}
