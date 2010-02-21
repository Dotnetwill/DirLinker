using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;
using System.Drawing;
using JunctionPointer.Exceptions;
using System.IO;

namespace JunctionPointer.Commands
{
    public class CopyCommand : ICommand
    {
        IFile _Source;
        IFile _Target;
        Boolean _Overwrite;

        public CopyCommand(IFile source, IFile target, Boolean overwrite)
        {
            _Source = source;
            _Target = target;
            _Overwrite = overwrite;
        }

        public void Execute()
        {
            if (_Target.Exists() && _Overwrite)
            {
                if ((_Target.GetAttributes() & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    return;
                }
            }

            if (!_Target.Exists() || _Overwrite)
            {
                _Source.CopyFile(_Target, _Overwrite);
            }

            Executed = true;
        }

        public void Undo()
        {
            //We only want to undo things we've done.  Should this throw an exception?
            if (!Executed)
                throw new CommandRunnerException("Can't undo something that hasn't been done.");

            if (!_Source.Exists())
            {
                _Target.CopyFile(_Source, false);
            }
        }

        public String Status
        {
            get
            {
                return String.Format("Copying file: {0} to {1}", _Source.FullFilePath, _Target.FullFilePath);
            }
        }

        public Boolean Executed { get; private set; }
        
        protected event RequestUserReponse _AskUser;
        
        public event RequestUserReponse AskUser
        {
            add { _AskUser += value; }
            remove { _AskUser -= value; }
        }
    }
}
