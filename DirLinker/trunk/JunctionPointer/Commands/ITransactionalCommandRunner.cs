using System;
using DirLinker.Interfaces;

namespace DirLinker.Commands
{
    public interface ITransactionalCommandRunner
    {
        void QueueRange(System.Collections.Generic.List<ICommand> commandList);
        /// <summary>
        /// This event is raised when all the work has been completed.  It will return a 
        /// a complete work report including any exceptions. 
        /// </summary>
        event WorkCompletedCallBack WorkCompleted;
        /// <summary>
        /// Returns the current number of commands in the queue
        /// </summary>
        Int32 CommandQueueCount { get; }
        /// <summary>
        /// What the current command is doing.
        /// **Thread safe and bindable**
        /// </summary>
        String CurrentStatus { get; }
        /// <summary>
        /// Percentage out of a 100 the current operation is at
        /// **Thread safe and bindable**
        /// </summary>
        Int32 PercentageComplete { get; }
        /// <summary>
        /// Adds a command to the queue to be run
        /// </summary>
        /// <param name="command">a class that implements ICommand</param>
        void QueueCommand(ICommand command);
        /// <summary>
        /// Starts a background thread and runs the current command queue.
        /// When finished The WorkCompleted event will be raised.
        /// </summary>
        void RunAsync();
        /// <summary>
        /// Requests the work is cancelled.  The cancel flag is only checked between running commands.
        /// </summary>
        void RequestCancel();
    }
}
