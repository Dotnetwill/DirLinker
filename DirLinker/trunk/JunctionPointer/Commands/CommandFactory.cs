using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;

namespace DirLinker.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom)
        {
            return new CreateLinkCommand(linkTo, linkFrom);
        }

        public ICommand DeleteFolderCommand(IFolder folder)
        {
            return new DeleteFolderCommand(folder);
        }

        public ICommand CreateFolder(IFolder folder)
        {
            return new CreateFolderCommand(folder);
        }

        public ICommand MoveFileCommand(IFile source, IFile target, Boolean overwriteTarget)
        {
            return new MoveFileCommand(source, target, overwriteTarget);
        }
    }
}
