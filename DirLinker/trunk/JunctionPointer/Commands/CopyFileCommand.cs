using System;
using JunctionPointer.Interfaces;
using JunctionPointer.Exceptions;
using System.IO;
using System.Windows.Forms;

namespace JunctionPointer.Commands
{
    public delegate ICommand CopyFileCommandFactory(IFile folder);

    public class CopyFileCommand : ICommand
    {
        private IFile _Source;
        private IFile _Target;
        private Boolean _Overwrite;
        private Boolean _FileCopied;

        public CopyFileCommand(IFile source, IFile target, Boolean overwrite)
        {
            _Source = source;
            _Target = target;
            _Overwrite = overwrite;
            _FileCopied = false;
        }

        public void Execute()
        {
            Boolean isOverwriteable = TargetOverwriteable();

            if (!_Target.Exists() || isOverwriteable)
            {
                _Source.CopyFile(_Target, _Overwrite);
                _FileCopied = true;
            }

        }

        private Boolean TargetOverwriteable()
        {
            Boolean overwriteTarget = true;

            if (_Target.Exists() && _Overwrite)
            {
                if ((_Target.GetAttributes() & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    DialogResult res = RequestUserRespone(String.Format("{0} is read only.  Would you like to overwrite it?", _Target.FullFilePath));
                    if (res == DialogResult.Yes)
                    {
                        _Target.SetAttributes(FileAttributes.Normal);
                        overwriteTarget = true;
                    }
                    else if (res == DialogResult.No)
                    {
                        overwriteTarget = false;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        throw new DirLinkerException("User requested cancel", DirLinkerStage.CopyingSourceToTemp);
                    }
                }
            }
            else if(_Target.Exists() && !_Overwrite)
            {
                overwriteTarget = false;
            }

            return overwriteTarget;
        }

        private DialogResult RequestUserRespone(String message)
        {
            RequestUserReponse ask = _AskUser;
            DialogResult res = DialogResult.Cancel;

            if (ask != null)
            {
                res = _AskUser(message, MessageBoxButtons.YesNoCancel);
            }

            return res;
        }

        public void Undo()
        {
            if (_FileCopied && !_Source.Exists())
            {
                _Target.CopyFile(_Source, false);
                _Target.Delete();
            }
        }

        public String Status
        {
            get
            {
                return String.Format("Copying file: {0} to {1}", _Source.FullFilePath, _Target.FullFilePath);
            }
        }
        
        protected event RequestUserReponse _AskUser;
        
        public event RequestUserReponse AskUser
        {
            add { _AskUser += value; }
            remove { _AskUser -= value; }
        }
    }
}
