using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Commands
{
    public class CreateFolderCommand : ICommand
    {
        
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Status
        {
            get { throw new NotImplementedException(); }
        }

        public event RequestUserReponse AskUser;
    }
}
