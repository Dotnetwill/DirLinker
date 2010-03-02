using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom)
        {
            return new CreateLinkCommand(linkTo, linkFrom);
        }
    }
}
