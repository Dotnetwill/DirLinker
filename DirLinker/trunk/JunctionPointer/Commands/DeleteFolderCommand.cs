using System;
using System.Linq;
using JunctionPointer.Interfaces;
using JunctionPointer.Exceptions;
using System.IO;

namespace JunctionPointer.Commands
{
    //I have considered having a seperate delete file command that is seperate or run as part of this
    //command but this pattern is being implemented to support undo and deleting a file is not something
    //you can undo easily or without storing the files in temp directory.  Which I don't want to do.

    public class DeleteFolderCommand : ICommand
    {
        private IFolder _Folder;
        private Boolean _FolderDeleted;
        public DeleteFolderCommand(IFolder folder)
        {
            _Folder = folder;
        }

        public void Execute()
        {
            if (_Folder.FolderExists())
            {
                DeleteContents();
                _Folder.DeleteFolder();
                _FolderDeleted = true;
            }
        }

        private void DeleteContents()
        {
            _Folder.GetFileList().ForEach(f => 
            {
                if ((f.GetAttributes() & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    f.SetAttributes(FileAttributes.Normal);
                }
                f.Delete();
            
            });

            if (_Folder.GetSubFolderList().Count > 0)
            {
                throw new DirLinkerException("Can not delete a folder with subfolders", DirLinkerStage.Unknown);
            }

        }

        public void Undo()
        {
            if (_FolderDeleted)
            {
                _Folder.CreateFolder();
            }
        }

        public string UserFeedback
        {
            get { return String.Format("Deleting folder {0}", _Folder.FolderPath); }
        }

        public event RequestUserReponse AskUser;

    }
}
