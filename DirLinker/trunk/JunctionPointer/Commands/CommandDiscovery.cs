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
        private IFolderFactoryForPath _folderFactory;

        public CommandDiscovery(ICommandFactory factory, 
            IFileFactoryForPath fileFactory, 
            IFolderFactoryForPath folderFactory)
        {
            _factory = factory;
            _fileFactory = fileFactory;
            _folderFactory = folderFactory;
        }
        public List<ICommand> GetCommandListForFileTask(IFile linkTo, IFile linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
        {
            var commandList = new List<ICommand>();

            if (linkTo.Exists())
            {
                if (copyBeforeDelete)
                {
                    var moveFileCommand = _factory.MoveFileCommand(linkTo, linkFrom, overwriteTargetFiles);
                    commandList.Add(moveFileCommand);
                }
                else
                {
                    var delFileCommand = _factory.DeleteFileCommand(linkTo);
                    commandList.Add(delFileCommand);
                }
            }

            commandList.Add(_factory.CreateFileLinkCommand(linkTo, linkFrom));

            return commandList;
        }

        public List<ICommand> GetCommandListTask(String linkTo, string linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
        {
            return null;
        }
        
        
        public List<ICommand> GetCommandListForFolderTask(IFolder linkTo, IFolder linkFrom, bool copyBeforeDelete, bool overwriteTargetFiles)
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

           commandList.Add(_factory.CreateFolderLinkCommand(linkTo, linkFrom));
           
           return commandList;
        }

        private List<ICommand> CreateFolderMoveOperations(IFolder source, IFolder target, bool overwriteTargetFiles)
        {
            var moveFolderStructureCommands = new List<ICommand>();

            source.GetSubFolderList().ForEach(f =>
                {
                    String targetLocation = f.FolderPath.Replace(source.FolderPath, target.FolderPath);
                    IFolder moveTarget = _folderFactory(targetLocation);
                    if (!moveTarget.FolderExists())
                    {
                       moveFolderStructureCommands.Add(_factory.CreateFolder(moveTarget));
                    }

                    moveFolderStructureCommands.AddRange(CreateFolderMoveOperations(f, moveTarget, overwriteTargetFiles));
                });

            source.GetFileList().ForEach(f => 
                {
                    IFile targetFile = _fileFactory(Path.Combine(target.FolderPath, f.FileName));
                    
                    ICommand moveFileCommand = _factory.MoveFileCommand(f, targetFile, overwriteTargetFiles);
                    moveFolderStructureCommands.Add(moveFileCommand);
                });

            return moveFolderStructureCommands;
        }

    }
}
