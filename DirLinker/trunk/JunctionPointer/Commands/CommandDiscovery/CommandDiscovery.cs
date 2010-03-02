using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands.CommandDiscovery
{
    public class CommandDiscovery : ICommandDiscovery
    {

        public List<ICommand> GetCommandListForTask(IFolder linkTo, IFolder linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
        {
            throw new NotImplementedException();
        }

    }
}
