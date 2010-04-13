using System;
using DirLinker.Interfaces;
using System.IO;
using DirLinker.Exceptions;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using OCInject;

namespace DirLinker.Implemenation
{
     public class DirLinker /*: IDirLinker*/
    {
        protected event ReportProgress m_ReportFeedback;
        //protected event UserMessage m_UserResponseRequired;
   

        protected IFolderFactoryForPath FolderFactoryForPath;
        protected IFileFactoryForPath FileFactoryForPath;

        public DirLinker(IFolderFactoryForPath folderFactory, IFileFactoryForPath fileFactory)
        {
            FolderFactoryForPath = folderFactory;
            FileFactoryForPath = fileFactory;
        }

        public void CreateSymbolicLinkFolder(String linkPoint, String linkTo, Boolean copyContentsToTarget, Boolean overwriteTargetFiles)
        {
            if (linkPoint.Equals(linkTo))
            {
                throw new ArgumentException("linkPoint and linkTo can not be the same");
            }

            IFolder linkPointFolder = FolderFactoryForPath(linkPoint);
            IFolder linkToFolder = FolderFactoryForPath(linkTo);
            
            try
            {
                if (!linkToFolder.FolderExists())
                {
                    linkToFolder.CreateFolder();
                }

                if (linkPointFolder.FolderExists())
                {
                    CopyAndDeleteFolder(linkPointFolder, linkToFolder, overwriteTargetFiles, copyContentsToTarget);
                }

                if (!linkToFolder.CreateLinkToFolderAt(linkPoint))
                {
                    throw new DirLinkerException(String.Format("Failed to create directory link error: {0}.  The contents of the link have been copied to the target folder.", Marshal.GetLastWin32Error()),
                                                DirLinkerStage.CreatingDirectoryLink);
                }

                //ReportSuccessToUser();
                
            }
            catch (Exception ex)
            {
                TraceOutException(ex);
              //  ReportExceptionErrorToUser(ex);
            }
        }

        //private void ReportSuccessToUser()
        //{
        //    AskUserQuestion("Operation Success!", MessageBoxButtons.OK);
        //}

        //private void ReportExceptionErrorToUser(Exception dirEx)
        //{
        //    AskUserQuestion(String.Format("Operation Failed: {0}", dirEx.Message), MessageBoxButtons.OK);
        //}

        protected void CopyAndDeleteFolder(IFolder sourceFolder, IFolder targetFolder,  Boolean overwriteTargetFiles, Boolean copyFilesToTarget)
        {
            if (copyFilesToTarget)
            {
                CopyFolder(sourceFolder, targetFolder, overwriteTargetFiles);
            }

            DeleteFolder(sourceFolder);
        }

        public void CopyFolder(IFolder sourceFolder, IFolder targetFolder, Boolean overWriteTargetFile)
        {
            if (!targetFolder.FolderExists())
            {
                targetFolder.CreateFolder();
            }

            CopyFilesFromFolder(sourceFolder, targetFolder, overWriteTargetFile);

            CopySubFolders(sourceFolder, targetFolder, overWriteTargetFile);
        }


        private void CopyFilesFromFolder(IFolder sourceFolder, IFolder targetFolder, Boolean overWriteTargetFile)
        {
            sourceFolder.GetFileList().ForEach(file =>
                            {
                                SendFeedbackReport("Copying Folder", String.Format("Currently copying file: {0}", file.FileName));

                                String path = Path.Combine(targetFolder.FolderPath, file.FileName);
                                IFile targetFile = FileFactoryForPath(path);
                                
                                if (!targetFile.Exists() || TargetFileWriteable(targetFile, overWriteTargetFile))
                                {
                                     file.CopyFile(targetFile, overWriteTargetFile);
                                }

                            }
                        );
        }

        private Boolean TargetFileWriteable(IFile targetFile, Boolean overWriteTargetFile)
        {
            Boolean writeable = false;

            if (overWriteTargetFile)
            {
                writeable = CheckAndClearReadonlyFlag(targetFile);
            }

            return writeable;
        }

        private Boolean CheckAndClearReadonlyFlag(IFile targetFile)
        {
            if ((targetFile.GetAttributes() & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                //String userQuestion = String.Format("File \"{0}\" is readonly.  Would you like to overwrite it?", targetFile.FullFilePath);
                //MessageBoxButtons options = MessageBoxButtons.YesNoCancel;
                //DialogResult result = AskUserQuestion(userQuestion, options);
                
                //if (result == DialogResult.Yes)
                //{
                //    targetFile.SetAttributes(FileAttributes.Normal);
                //}
                //else if (result == DialogResult.No)
                //{
                //    return false;
                //}
                //else if (result == DialogResult.Cancel)
                //{
                //    throw new DirLinkerException("User cancelled operation", DirLinkerStage.Unknown);
                //}
                //else
                //{
                //    throw new Exception("Unknown dialog result");
                //}
            }
            return true;
        }
        private void CopySubFolders(IFolder sourceFolder, IFolder targetFolder, Boolean overWriteTargetFile)
        {
            List<IFolder> subFolderList = sourceFolder.GetSubFolderList();
         
            subFolderList.ForEach(subFolder =>
                {
                    SendFeedbackReport("Copying Folder", String.Format("Currently copying file: {0}", subFolder));

                    String path = subFolder.FolderPath.Replace(sourceFolder.FolderPath, targetFolder.FolderPath);
                    IFolder targetSubFolderPath = FolderFactoryForPath(path);
                    
                    CopyFolder(subFolder, targetSubFolderPath, overWriteTargetFile);
                }
            );
        }

        private void DeleteFolder(IFolder folder)
        {
            DeleteFolderFiles(folder);

            folder.GetSubFolderList().ForEach(subFolder =>
                {
                    DeleteFolder(subFolder);
                }
            );

            folder.SetAttributes(FileAttributes.Normal);
            folder.DeleteFolder();
        }

        private void DeleteFolderFiles(IFolder folder)
        {
            folder.GetFileList().ForEach(file =>
                            {
                                SendFeedbackReport("Deleting Folder", String.Format("Currently deleting file: {0}", file.FileName));
                                file.SetAttributes(FileAttributes.Normal);
                                file.Delete();
                            }
                        );
        }
        public Boolean CheckEnoughSpace(String source, String target)
        {
            IFolder sourceFolder = FolderFactoryForPath(source);

            if (sourceFolder.DirectorySize() > sourceFolder.FreeSpaceOnDrive(Path.GetPathRoot(target)))
            {
                return false;
            }

            return true;
        }

        //public event UserMessage UserMessage
        //{
        //    add { m_UserResponseRequired += value; }
        //    remove { m_UserResponseRequired -= value; }
        //}

        public event ReportProgress ReportFeedback
        {
            add { m_ReportFeedback += value; }
            remove { m_ReportFeedback -= value; }
        }

        protected virtual void SendFeedbackReport(String stage, String currenlyDoing)
        {
            if (m_ReportFeedback != null)
            {
                FeedbackArgs args = new FeedbackArgs { ProgressTitle = stage, Progress = currenlyDoing };
                m_ReportFeedback(this, args);
            }
        }


        
         private void TraceOutException(Exception ex)
        {
            Trace.WriteLine("====Exception Caught====");
            Trace.WriteLine(ex.Message);
            Trace.WriteLine(ex.StackTrace);
            Trace.WriteLine(ex.Source);
            Trace.WriteLine(ex.GetType().ToString());
            Trace.WriteLine("====End Exception Caught====");

        }
    }
}
