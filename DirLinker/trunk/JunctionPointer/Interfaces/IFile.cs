using System;

namespace JunctionPointer.Interfaces
{
   
    public interface IFile
    {

        String FileName { get; }
        String Folder { get; }
        String FullFilePath { get; }

        System.IO.FileAttributes GetAttributes();

        void CopyFile(IFile fullTargetPathWithFileName, Boolean overwrite);
        void SetAttributes(System.IO.FileAttributes attributes);
        void SetFile(String fileWithFullPath);
        void Delete();
        
        Boolean Exists();
    }
}
