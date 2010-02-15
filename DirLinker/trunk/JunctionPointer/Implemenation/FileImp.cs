using System;
using JunctionPointer.Interfaces;
using System.IO;

namespace JunctionPointer.Implemenation
{
    class FileImp : IFile
    {
        public FileImp(String fullPath)
        {
            m_FullPath = fullPath;
        }
        private String m_FullPath;

        public string FileName
        {
            get
            {
                return Path.GetFileName(m_FullPath);
            }
        }

        public string Folder
        {
            get
            {
                return Path.GetDirectoryName(m_FullPath);
            }
        }

        public String FullFilePath 
        {
            get
            {
                return m_FullPath;
            }
        }

        public void CopyFile(IFile fullTargetPathWithFileName, Boolean overwrite)
        {
            File.Copy(m_FullPath, fullTargetPathWithFileName.FullFilePath, overwrite);
        }

        public FileAttributes GetAttributes()
        {
            return File.GetAttributes(m_FullPath);
        }

        public void SetAttributes(FileAttributes attributes)
        {
            File.SetAttributes(m_FullPath, attributes);
        }

        public Boolean Exists()
        {
            return File.Exists(m_FullPath);
        }

        public void Delete()
        {
            File.Delete(FullFilePath);
        }
    }
}
