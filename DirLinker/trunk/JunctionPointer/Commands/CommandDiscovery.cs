using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;
using System.IO;

namespace DirLinker.Commands
{
    public class CommandDiscovery : ICommandDiscovery
    {
        private ICommandFactory _factory;
        private IFileFactoryForPath _fileFactory;

        public CommandDiscovery(ICommandFactory factory, IFileFactoryForPath fileFactory)
        {
            _factory = factory;
            _fileFactory = fileFactory;
        }

        public List<ICommand> GetCommandListForTask(IFolder linkTo, IFolder linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
        {
           List<ICommand> commandList = new List<ICommand>();

           if (!linkFrom.FolderExists())
           {
               commandList.Add(_factory.CreateFolder(linkFrom));
           }

           if (linkTo.FolderExists())
           {
               if (copyBeforeDelete)
               {
                   commandList.AddRange(CreateFolderMoveOperations(linkTo, linkFrom, overwriteTargetFiles));
               }
               commandList.Add(_factory.DeleteFolderCommand(linkTo));
           }

           commandList.Add(_factory.CreateLinkCommand(linkTo, linkFrom));
           
           return commandList;
        }

        private List<ICommand> CreateFolderMoveOperations(IFolder linkTo, IFolder linkFrom, bool overwriteTargetFiles)
        {
            var moveFolderStructureCommands = new List<ICommand>();

            linkTo.GetFileList().ForEach(f => 
                {
                    IFile targetFile = _fileFactory(Path.Combine(linkFrom.FolderPath, f.FileName));
                    
                    ICommand moveFileCommand = _factory.MoveFileCommand(f, targetFile, overwriteTargetFiles);
                    moveFolderStructureCommands.Add(moveFileCommand);
                });

            return moveFolderStructureCommands;
        }

    }
}
