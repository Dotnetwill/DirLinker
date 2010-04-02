using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.ComponentModel;
using DirLinker.Interfaces;
using DirLinker.Implementation;

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
    public class TransactionalCommandRunner : INotifyPropertyChanged, ITransactionalCommandRunner
    {

        /// <summary>
        /// Safely marshalls messages across threads
        /// </summary>
        public class ThreadMessenger
        {
            private Dispatcher _Dispatcher;
            private Action<String, Int32> _DispatchMessage;

            public ThreadMessenger(Dispatcher dispatch, Action<String, Int32> dispatchMethod)
            {
                _Dispatcher = dispatch;
                _DispatchMessage = dispatchMethod;
            }

            public void StatusUpdate(String message, Int32 percentageComplete)
            {
                _Dispatcher.BeginInvoke((Action)delegate { _DispatchMessage(message, percentageComplete); });
            }

            //public DialogResult RequestUserFeedback(String message, MessageBoxButtons options)
            //{
            //    _Dispatcher.BeginInvoke()
            //    return DialogResult.Cancel;            
            //}
        }

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
        /// What the current command is doing.
        /// **Thread safe and bindable**
        /// </summary>
        public String CurrentStatus { get; private set; }

        /// <summary>
        /// Percentage out of a 100 the current operation is at
        /// **Thread safe and bindable**
        /// </summary>
        public Int32 PercentageComplete { get; private set; }

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
        public void RunAsync()
        {
            _bgWorker.DoWork += (sender, args) => RunCommandQueue(args.Argument as ThreadMessenger);

            _bgWorker.RunWorkerCompleted += NotifyWorkCompleted;

            ThreadMessenger messenger = new ThreadMessenger(Dispatcher.CurrentDispatcher, (Action<string, int>)StatusUpdateDelegate);

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
        
        private void RunCommandQueue(ThreadMessenger messenger)
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

        private void ProcessUndoStack(ThreadMessenger messenger)
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
