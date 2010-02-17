using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces;

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
            _Source.CopyFile(_Target, _Overwrite);
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public String Status
        {
            get
            {
                return String.Format("Copying file: {0} to {1}", _Source.FullFilePath, _Target.FullFilePath);
            }
        }
    }
}
