using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom);
    }
}
