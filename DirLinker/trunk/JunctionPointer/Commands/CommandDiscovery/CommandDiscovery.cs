using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands.CommandDiscovery
{
    public class CommandDiscovery : ICommandDiscovery
    {
        private ICommandFactory _factory;

        public CommandDiscovery(ICommandFactory factory)
        {
            _factory = factory;
        }

        public List<ICommand> GetCommandListForTask(IFolder linkTo, IFolder linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
        {
           List<ICommand> commandList = new List<ICommand>();

           if (linkTo.FolderExists())
           {
               if (!copyBeforeDelete)
               {
                   commandList.Add(_factory.DeleteFolderCommand(linkTo));
               }
           }

           commandList.Add(_factory.CreateLinkCommand(linkTo, linkFrom));
           
           return commandList;
        }

    }
}
