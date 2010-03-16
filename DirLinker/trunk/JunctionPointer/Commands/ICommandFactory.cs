using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands
{
    public interface ICommandFactory
    {
        ICommand MoveFileCommand(IFile source, IFile target, Boolean overwriteTarget);
        ICommand CreateFolder(IFolder folder);
        ICommand DeleteFolderCommand(IFolder folder);
        ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom);
    }
}
