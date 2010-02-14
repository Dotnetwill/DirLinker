using System;
using NUnit.Framework;
using Rhino.Mocks;
using JunctionPointer.Interfaces;
using System.Collections.Generic;
using DirLinker.Tests.Helpers;
using JunctionPointer.Helpers.OCInject;
using System.IO;
using System.Windows.Forms;
using JunctionPointer.Helpers.Interfaces;

namespace DirLinker.Tests
{
    
    [TestFixture]
    public class DirLinkerTests
    {

        private readonly String m_Path = @"C:\Test\Path";

        private String GetPathAddingChar(Char c)
        {
            return String.Concat(m_Path, c);

        }

        private IFolder GetFromMapOrDefault(Dictionary<String, IFolder> folderContainer, String folderName)
        {
            if (folderContainer.ContainsKey(folderName))
            {
                return folderContainer[folderName];
            }
            else
            {
                return new FakeFolder(folderName);
            }
        }

        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_User_Cancel_File_Not_Copied()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String filename = "file1";

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return(Path.Combine(target, filename));
            fileContainer.Add(Path.Combine(target, filename), targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderContainer.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderContainer.Add(target, targetFolder);
            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.UserMessage += (sender, e) => e.Response = DialogResult.Cancel;

            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //
            //Assert
            targetFile.AssertWasNotCalled(t => t.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasNotCalled(s => s.CopyFile(targetFile, true));
        }


        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_User_No_File_Not_Copied()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String filename = "file1";
            
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return(filename);
            fileContainer.Add(Path.Combine(target, filename), targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.UserMessage += (sender, e) => e.Response = DialogResult.No;

            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

            //
            //Assert
            targetFile.AssertWasNotCalled(t => t.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasNotCalled(s => s.CopyFile(targetFile, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_User_Yes_Readonly_Cleared_File_Copied()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String filename = "file1";

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>(); 
            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return(filename);
            fileContainer.Add(Path.Combine(target, filename), targetFile);

            Dictionary<String, IFolder> folderConatiner = new Dictionary<String, IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderConatiner.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderConatiner.Add(target, targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderConatiner[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.UserMessage += (sender, e) => e.Response = DialogResult.Yes;

            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //
            //Assert
            targetFile.AssertWasCalled(t => t.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasCalled(s => s.CopyFile(targetFile, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_User_Asked()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String filename = "file1";
            String target = @"c:\target";

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);
            fileContainer.Add(Path.Combine(source, filename), sourceFile);

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return("file1");
            fileContainer.Add(Path.Combine(target, filename), targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderContainer.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderContainer.Add(target, targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.UserMessage += (sender, e) =>
                {
                    if (!e.Message.Contains("Success"))
                    {
                        Assert.IsTrue(e.Message.Contains("file1"));
                        Assert.IsTrue(e.Message.Contains("readonly"));
                        Assert.AreEqual(MessageBoxButtons.YesNoCancel, e.ResponseOptions);
                    }
                    e.Response = DialogResult.Yes;
                };

            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);
        }
        
        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_Readonly_Cleared()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String filename = "file1";

            Dictionary<String, IFile> fileContainer = new Dictionary<String,IFile>();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);            
            sourceFile.Stub(s => s.FileName).Return(filename);
            fileContainer.Add(Path.Combine(source, filename), sourceFile);

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            fileContainer.Add(Path.Combine(target, filename), targetFile);
           
            Dictionary<String, IFolder> folderContainer = new Dictionary<String,IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderContainer.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderContainer.Add(target, targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.UserMessage += (sender, e) => e.Response = System.Windows.Forms.DialogResult.Yes;
            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //Assert
            targetFile.AssertWasCalled(f => f.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasCalled(f => f.CopyFile(targetFile, true));
        }


        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Exists_When_Overwriting_Should_Check_Target_ReadOnly_Flag()
        {
            //
            //Arrange   
            String source = @"c:\source\";
            String target = @"c:\target\";
            String filename = "file1";
            
            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);
            fileContainer.Add(source + filename, sourceFile);

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.Normal);
            fileContainer.Add(target + filename, targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderContainer.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            targetFolder.Stub(d => d.FolderPath).Return(target);
            folderContainer.Add(target, targetFolder);
            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //Assert
            targetFile.AssertWasCalled(f => f.GetAttributes());
        }


        [Test]
        public void CreateSymbolicLinkFolder_Clear_File_ReadOnly_Flag_Before_Deleting_When_Copying_Files()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            
            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");

            Dictionary<String, IFolder> folderConatiner = new Dictionary<String, IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderConatiner.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderConatiner.Add(target, targetFolder);


            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderConatiner[f],
                                                                                                            f => new FakeFile(f));
            dirLinker.CreateSymbolicLinkFolder(source, target, true, false);

            //Assert
            sourceFile.AssertWasCalled(f => f.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasCalled(f => f.Delete());
        }


        [Test]
        public void CreateSymbolicLinkFolder_Dont_attemp_file_copy_if_target_exists()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String filename = "file1";

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return(filename);

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            fileContainer.Add(Path.Combine(target, filename), targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            folderContainer.Add(source, sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            folderContainer.Add(target, targetFolder);


            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            f => fileContainer[f]);
            dirLinker.CreateSymbolicLinkFolder(source, target, true, false);

            //Assert
            sourceFile.AssertWasNotCalled(f => f.CopyFile(Arg<IFile>.Is.Anything, Arg<Boolean>.Is.Anything));
        }

        [Test]
        public void CreateSymbolicLinkFolder_ClearReadOnlyFlagBeforeDeleting()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            
            MockRepository mockRepo = new MockRepository();
            IFolder sourceFolder = mockRepo.DynamicMock<IFolder>();

            using (mockRepo.Ordered())
            {
                Expect.Call(() => sourceFolder.SetAttributes(FileAttributes.Normal));
                Expect.Call(() => sourceFolder.DeleteFolder());
            }

            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);

            mockRepo.ReplayAll();

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            null);
            dirLinker.CreateSymbolicLinkFolder(source, target, false, true);

            //Assert
            sourceFolder.VerifyAllExpectations();
        }


        [Test]
        public void CreateSymbolicLinkFolder_Clear_ReadOnly_Flag_Before_Deleting_SubFolder_Only_When_Copying_Files()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";
            String subFolderName = "subfolder";
            
            MockRepository mockRepo = new MockRepository();

            IFolder subFolder = mockRepo.DynamicMock<IFolder>();
            subFolder.Stub(d => d.FolderPath).Return(Path.Combine(source, subFolderName));
            subFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            subFolder.Stub(d => d.FolderExists()).Return(true);
            subFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());

            IFolder sourceFolder = mockRepo.DynamicMock<IFolder>();
            sourceFolder.Stub(d => d.FolderPath).Return(source);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder> { subFolder });
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());

            using (mockRepo.Ordered())
            {
                Expect.Call(() => subFolder.SetAttributes(FileAttributes.Normal));
                Expect.Call(() => subFolder.DeleteFolder());

                Expect.Call(() => sourceFolder.SetAttributes(FileAttributes.Normal));
                Expect.Call(() => sourceFolder.DeleteFolder());

            }

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);

            //add another IFolder for target sub folder that is created
            IFolder targetSubFolder = new FakeFolder(Path.Combine(target, subFolderName));

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);
            folderContainer.Add(Path.Combine(target, subFolderName), targetSubFolder);
            
    

            //Act
            mockRepo.ReplayAll();
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            null);
            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //Assert

            subFolder.VerifyAllExpectations();
            sourceFolder.VerifyAllExpectations();
        }

        [Test]
        public void CreateSymbolicLinkFolder_NoCopyFiles_DeleteFolderCalled()
        {
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            null);
            dirLinker.CreateSymbolicLinkFolder(source, target, false, true);

            //Assert
            sourceFolder.AssertWasCalled(m => m.DeleteFolder());
        }

        [Test]
        public void CreateSymbolicLinkFolder_NoFilesToCopy_CreateLinkCalledCorrectly()
        {
            
            //
            //Arrange   
            String source = @"c:\source";
            String target = @"c:\target";

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                            null);
            
            //Act
            dirLinker.CreateSymbolicLinkFolder(source, target, true, true);

            //Assert
            targetFolder.AssertWasCalled(m => m.CreateLinkToFolderAt(source));
        }


        [Test]
        public void CreateSymbolicLinkFolder_CopySourceThrowsException_NoDeleteSourceCalled()
        {
            //
            //Arrange   
            String source = @"c:\source";


            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>(); 
            sourceFolder.Stub(d => d.GetFileList()).Throw(new Exception());
            
            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(source, sourceFolder);

            
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                                                            null);

            //Act
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

            //Assert
            sourceFolder.AssertWasNotCalled(m => m.DeleteFolder());
        }

     
        [Test]
        public void CreateSymbolicLinkFolder_CreateLinkReturnFail_UserNotifiedProcessFailed()
        {
            //
            //Arrange 

           

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
           

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            folderContainer.Add(@"c:\source", sourceFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                               null);

            dirLinker.UserMessage += (sender, args) =>
            {
                Assert.IsTrue(args.Message.Contains("Failed"));
                Assert.IsTrue(args.ResponseOptions == System.Windows.Forms.MessageBoxButtons.OK);
            };

            //Act
            dirLinker.CreateSymbolicLinkFolder(@"c:\source", @"c:\target", true, true);
        }

        [Test]
        public void CreateSymbolicLinkFolder_GetSourceFileListUnauthorizedAccessException_UserNotified()
        {
            //
            //Arrange 
            
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Throw(new UnauthorizedAccessException());
            
            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(@"c:\source", sourceFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                               null);

            dirLinker.UserMessage += (sender, args) =>
            {
                Assert.IsTrue(args.Message.Contains("Failed"));
                Assert.IsTrue(args.ResponseOptions == System.Windows.Forms.MessageBoxButtons.OK);
            };

            //Act
            dirLinker.CreateSymbolicLinkFolder(@"c:\source", @"c:\target", true, true);
        }


        [Test]
        public void CreateSymbolicLinkFolder_DeleteSourceFailsUnauthorizedAccessException_UserNotified()
        {
            //
            //Arrange 
     
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(s => s.FolderExists()).Return(true);
            sourceFolder.Stub(s => s.DeleteFolder()).Throw(new UnauthorizedAccessException());

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(@"c:\source", sourceFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                               null);

            dirLinker.UserMessage += (sender, args) =>
            {
                Assert.IsTrue(args.Message.Contains("Failed"));
                Assert.IsTrue(args.ResponseOptions == System.Windows.Forms.MessageBoxButtons.OK);
            };

            //Act
            dirLinker.CreateSymbolicLinkFolder(@"c:\source", @"c:\target", true, true);
        }


        [Test]
        public void CheckEnoughSpace_TargetPathTooSmall_ReturnsFalse()
        {
            //
            //Arrange 
            MockRepository mocks = new MockRepository();
            IFolder sourceFolder = mocks.DynamicMock<IFolder>();
            
            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(@"c:\sourcePath", sourceFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f) , null);

            using (mocks.Record())
            {
                sourceFolder.DirectorySize();
                LastCall.Return<Int64>(2);

                sourceFolder.FreeSpaceOnDrive(Arg<String>.Is.Anything);
                LastCall.Return<Int64>(1);
            }

            //Act

            Boolean enoughSpace = dirLinker.CheckEnoughSpace(@"c:\sourcePath", @"c:\targetPath\");

            //Assert
            Assert.IsFalse(enoughSpace);
        }


        [Test]
        public void CheckEnoughSpace_EnoughSpaceInTargetPath_ReturnsTrue()
        {
            //
            //Arrange 
            MockRepository mocks = new MockRepository();
            IFolder sourceFolder = mocks.DynamicMock<IFolder>();

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(@"c:\sourcePath", sourceFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                                                            null);

            using (mocks.Record())
            {
                sourceFolder.DirectorySize();
                LastCall.Return<Int64>(1);

                sourceFolder.FreeSpaceOnDrive(Arg<String>.Is.Anything);
                LastCall.Return<Int64>(2);
            }

            //Act

            Boolean enoughSpace = dirLinker.CheckEnoughSpace(@"c:\sourcePath", @"c:\tempPath\");

            //Assert
            Assert.IsTrue(enoughSpace);
        }
         
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateSymbolicLinkFolder_SourceTargetAreTheSame_ExceptionThrown()
        {
            String path = @"c:\testDir\";
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(null, null);

            testLinker.CreateSymbolicLinkFolder(path, path, true, true);
        }

        [Test]
        public void CreateSymbolicLinkFolder_OverwriteTargetFile_CopyFileCalledCorrectly()
        {
            //
            //Arrange
            String pathLink = @"c:\testDir\";
            String pathTo = @"c:\testTarget\";
            String filename = "file1";

            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            IFile fileStub = Helpers.CreateStubHelpers.GetIFileStub(filename, @"c:\testDir\");
            fileContainer.Add(pathLink + filename, fileStub);

            IFile targetFile = Helpers.CreateStubHelpers.GetIFileStub("", "");
            fileContainer.Add(pathTo + filename, targetFile);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String,IFolder>();
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { fileStub });
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            folderContainer.Add(pathLink, sourceFolder);

            folderContainer.Add(pathTo, new FakeFolder(pathTo));


            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                             f => fileContainer[f]);
            testLinker.CreateSymbolicLinkFolder(pathLink, pathTo, true, true);

            //Assert
            //targetFile.AssertWasCalled(f => f.SetFile(pathTo + "file1"));
            fileStub.AssertWasCalled(f => f.CopyFile(targetFile, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_DoNotOverwriteTargetFile_CopyFileCalledCorrectly()
        {
            //
            //Arrange
            String pathLink = @"c:\testDir\";
            String pathTo = @"c:\testTarget\";


            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            IFile fileStub = Helpers.CreateStubHelpers.GetIFileStub("file1", @"c:\testDir\");
            fileContainer.Add(pathLink + "file1", fileStub);
            IFile targetFile = Helpers.CreateStubHelpers.GetIFileStub("", "");
            fileContainer.Add(pathTo + "file1", targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { fileStub });
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(pathLink, sourceFolder);
            folderContainer.Add(pathTo, new FakeFolder(pathTo));

            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                                             f => fileContainer[f]);
            testLinker.CreateSymbolicLinkFolder(pathLink, pathTo, true, false);

            //Assert
            fileStub.AssertWasCalled(f => f.CopyFile(targetFile, false));
            
        }


        [Test]
        public void CopyFolder_SourceTargetDifferent_CopiesFiles()
        {
            //
            //Arrange
            IFile fileA = Helpers.CreateStubHelpers.GetIFileStub("a", @"c:\source\");
            IFile fileB = Helpers.CreateStubHelpers.GetIFileStub("b", @"c:\source\");

            List<IFile> newList = new List<IFile>();
            newList.Add(fileA);
            newList.Add(fileB);

            String target = @"c:\target\";
            Dictionary<String, IFile> fileContainer = new Dictionary<String, IFile>();
            IFile targetFileA = Helpers.CreateStubHelpers.GetIFileStub("", "");
            fileContainer.Add(target + "a", targetFileA);

            IFile targetFileB = Helpers.CreateStubHelpers.GetIFileStub("", "");
            fileContainer.Add(target + "b", targetFileB);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Return(newList);
            sourceFolder.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());

            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(t => t.FolderPath).Return(target);
            
            //
            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(null,
                                                                                                             f => fileContainer[f]);
            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert
            fileA.AssertWasCalled(f => f.CopyFile(targetFileA, true));
            fileB.AssertWasCalled(f => f.CopyFile(targetFileB, true));
        }
    
        [Test]
        public void CopyFolder_CopySubfoldersWithNoContent_CopiesCorrectly()
        {
            //
            //Arrange
            String source = @"c:\source\";
            String target = @"c:\target\";

            FakeFolder subFolderA = new FakeFolder { FolderPath = source + "a", 
                                                       SubFolderList = new List<IFolder>(), 
                                                       FileList = new List<IFile>() };

            FakeFolder subFolderB = new FakeFolder { FolderPath = source + "b", 
                                                       SubFolderList = new List<IFolder>(), 
                                                       FileList = new List<IFile>() };

            FakeFolder sourceFolder = new FakeFolder { FolderPath = source, 
                                                         FileList = new List<IFile>(), 
                                                         SubFolderList = new List<IFolder> { subFolderA, subFolderB } };

            
            //Queue up two target folder to assert against
            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            FakeFolder targetSubFolderA = new FakeFolder();
            folderContainer.Add(target + "a", targetSubFolderA);
            
            FakeFolder targetSubFolderB = new FakeFolder();
            folderContainer.Add(target + "b", targetSubFolderB);
                        
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);

            //
            //Act
            IDirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                                null);

            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert
            Assert.IsTrue(targetSubFolderA.CreateFolderCalled);
            Assert.IsTrue(targetSubFolderB.CreateFolderCalled);
        }

        [Test]
        public void CopyFolder_SubFoldersContainsFiles_CopiedCorrectly()
        {

            //
            //Arrange
            String source = @"c:\source\";
            String target = @"c:\target\";

            IFile file1 = Helpers.CreateStubHelpers.GetIFileStub("file1", @"c:\source\a\");
            IFile file2 = Helpers.CreateStubHelpers.GetIFileStub("file2", @"c:\source\b");

            
            
            
            List<IFile> fileList1 = new List<IFile>();
            fileList1.Add(file1);
            IFolder subFolder1 = MockRepository.GenerateStub<IFolder>();
            subFolder1.Stub(d => d.FolderPath).Return(source + "a");
            subFolder1.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            subFolder1.Stub(s => s.GetFileList()).Return(fileList1);

            List<IFile> fileList2 = new List<IFile>();
            fileList2.Add(file2);
            IFolder subFolder2 = MockRepository.GenerateStub<IFolder>();
            subFolder2.Stub(d => d.FolderPath).Return(source + "b");
            subFolder2.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            subFolder2.Stub(s => s.GetFileList()).Return(fileList2);

            StubDirManagerReturnSubFolderList sourceFolder = new StubDirManagerReturnSubFolderList();
            sourceFolder.FolderPath = source;
            sourceFolder.FolderList.Add(subFolder1);
            sourceFolder.FolderList.Add(subFolder2);
            sourceFolder.OnlyReturnSubFolderListFor = sourceFolder;

            
            Dictionary<String, IFile> targetFileContainer = new Dictionary<String, IFile>();
            IFile targetFileA = MockRepository.GenerateStub<IFile>();
            targetFileContainer.Add(target + @"a\file1" , targetFileA);

            IFile targetFileB = MockRepository.GenerateStub<IFile>();
            targetFileContainer.Add(target + @"b\file2" , targetFileB);


            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);

            //
            //Act
            IDirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(f => new FakeFolder(f), f => targetFileContainer[f]);
            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert
            file1.AssertWasCalled(f => f.CopyFile(targetFileA, true));
            file2.AssertWasCalled(f => f.CopyFile(targetFileB, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_TargetDoesNotExist_TargetCreated()
        {
            //Arrange
            String source = @"c:\source\";
            String target = @"c:\target";
           
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            
            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderExists()).Return(false);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(source)).Return(true);
            
            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();
            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);


            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => folderContainer[f],
                                                                               null);
            //Act
            dirLinker.CreateSymbolicLinkFolder(source, target,  false, true);

            //Assert
            targetFolder.AssertWasCalled(d => d.FolderExists());
            targetFolder.AssertWasCalled(d => d.CreateFolder());
        }

        [Test]
        public void CreateSymbolicLinkFolder_CompletedSuccessfully_UserNotified()
        {
            //Arrange
            String target = @"c:\target";
            String source = @"c:\source\";


            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderExists()).Return(true);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);

            Dictionary<String, IFolder> folderContainer = new Dictionary<String, IFolder>();

            folderContainer.Add(source, sourceFolder);
            folderContainer.Add(target, targetFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(f => GetFromMapOrDefault(folderContainer, f),
                                                                               null);

            Boolean delegateCalled = false;
            dirLinker.UserMessage += delegate(Object sender, UserMessageArgs args)
            {
                delegateCalled = true;
                Assert.IsTrue(args.Message.ToLower().Contains("success"));
                Assert.IsTrue(args.ResponseOptions == System.Windows.Forms.MessageBoxButtons.OK);
            };

            //Act
            dirLinker.CreateSymbolicLinkFolder(source, target, false, true);


            //Assert
            Assert.IsTrue(delegateCalled);
        }

        [Test]
        public void ValidatePath_ValidPath_Valid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();
           
            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(m_Path, out errorMessage);

            //Assert
            Assert.IsTrue(valid);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_EmptyString_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);
  
            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(String.Empty, out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_SpaceOnly_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();
            
            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath("  ", out errorMessage);

            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_GreaterThanMaxPath_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub(1);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            String longPath = @"c:\testPathThatIsLongerTheDirManagerAllows";


            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(longPath, out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_PathContainsIllegalCharacter_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub(new Char[] { '|', '?', ':' });

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                               null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(GetPathAddingChar('|'), out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_DriveHasColonPathDoesnContainAnyIllegalCharacters_Valid()
        {
            //MSDN states that : is illegal and is returned so need to check that when a drive is there it is still valid
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub(new Char[] { '|', '?', ':' });

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(m_Path, out errorMessage);


            //Assert
            Assert.IsTrue(valid);
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_DriveOnly_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(@"C:\", out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_NoDriveAtStart_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                               null);


            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(@"thisIsNotAPath", out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_NoDriveAtStartOneBackSlash_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);

            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(@"\test", out errorMessage);


            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_3CharAtStartInvalidDrive_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                                null);
            
            //Act
            String errorMessage;
            Boolean valid = controller.ValidDirectoryPath(@"abc\test", out errorMessage);

            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMessage);
        }

        [Test]
        public void ValidatePath_UNCPath_Invalid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();
            
            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(f => dirManager,
                                                                               null);


            //Act
            String errorMsg;
            Boolean valid = controller.ValidDirectoryPath(@"\\network\share", out errorMsg);

            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMsg);
        }
    }
}
