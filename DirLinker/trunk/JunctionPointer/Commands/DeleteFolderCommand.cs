using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands
{
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
            _Folder.DeleteFolder();
            _FolderDeleted = true;
        }

        public void Undo()
        {
            if (_FolderDeleted)
            {
                _Folder.CreateFolder();
            }
        }

        public string Status
        {
            get { throw new NotImplementedException(); }
        }

        public event RequestUserReponse AskUser;

    }
}
