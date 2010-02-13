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

        private IFolderFactoryForPath CreateIFolderFactoryFromContainer(IClassFactory container)
        {
            return f => container.ManufactureType<IFolder>();
        }

        private IFileFactoryForPath CreateIFileFactoryFromContainer(IClassFactory container)
        {
            return f => container.ManufactureType<IFile>();
        }

        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_User_Cancel_File_Not_Copied()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");


            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return("file1");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.UserMessage += (sender, e) => e.Response = DialogResult.Cancel;

            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");


            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return("file1");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");


            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return("file1");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.UserMessage += (sender, e) => e.Response = DialogResult.Yes;

            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");


            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            targetFile.Stub(f => f.FullFilePath).Return("file1");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
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

            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);
        }
        
        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Readonly_Readonly_Cleared()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");

            
            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.ReadOnly);
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.UserMessage += (sender, e) => e.Response = System.Windows.Forms.DialogResult.Yes;
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

            //Assert
            targetFile.AssertWasCalled(f => f.SetAttributes(FileAttributes.Normal));
            sourceFile.AssertWasCalled(f => f.CopyFile(targetFile, true));
        }


        [Test]
        public void CreateSymbolicLinkFolder_Target_File_Exists_When_Overwriting_Check_If_Its_Readonly()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");

            
            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            targetFile.Stub(f => f.GetAttributes()).Return(FileAttributes.Normal);
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);



            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

            //Assert
            targetFile.AssertWasCalled(f => f.GetAttributes());
        }


        [Test]
        public void CreateSymbolicLinkFolder_Clear_File_ReadOnly_Flag_Before_Deleting_When_Copying_Files()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");


            IFile targetFile = MockRepository.GenerateStub<IFile>();
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);



            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, false);

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile sourceFile = MockRepository.GenerateStub<IFile>();
            sourceFile.Stub(s => s.Folder).Return(source);
            sourceFile.Stub(s => s.FileName).Return("file1");
    

            IFile targetFile = MockRepository.GenerateStub<IFile>();
            targetFile.Stub(f => f.Exists()).Return(true);
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { sourceFile });
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

           

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, false);

            //Assert
            sourceFile.AssertWasNotCalled(f => f.CopyFile(Arg<IFile>.Is.Anything, Arg<Boolean>.Is.Anything));
        }

        [Test]
        public void CreateSymbolicLinkFolder_ClearReadOnlyFlagBeforeDeleting()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();

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
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            mockRepo.ReplayAll();

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", false, true);

            //Assert
            sourceFolder.VerifyAllExpectations();
        }


        [Test]
        public void CreateSymbolicLinkFolder_Clear_ReadOnly_Flag_Before_Deleting_SubFolder_Only_When_Copying_Files()
        {
            //
            //Arrange   
            String source = @"c:\source";
            QueueBasedClassFactory container = new QueueBasedClassFactory();
            
            MockRepository mockRepo = new MockRepository();
        
            IFolder subFolder = mockRepo.DynamicMock<IFolder>();
            subFolder.Stub(d => d.FolderPath).Return(Path.Combine(source, "subfolder"));
            subFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            subFolder.Stub(d => d.FolderExists()).Return(true);
            subFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());

            IFolder sourceFolder = mockRepo.DynamicMock<IFolder>();
            sourceFolder.Stub(d => d.FolderPath).Return(source);
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder> { subFolder });
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            container.IFolderQueue.Enqueue(sourceFolder);

            using (mockRepo.Ordered())
            {
                Expect.Call(() => subFolder.SetAttributes(FileAttributes.Normal));
                Expect.Call(() => subFolder.DeleteFolder());

                Expect.Call(() => sourceFolder.SetAttributes(FileAttributes.Normal));
                Expect.Call(() => sourceFolder.DeleteFolder());

            }

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //add another IFolder for target sub folder that is created
            container.IFolderQueue.Enqueue(MockRepository.GenerateStub<IFolder>());

            //Act
            mockRepo.ReplayAll();
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", true, true);

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            //Act
            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            dirLinker.CreateSymbolicLinkFolder(source, @"c:\target", false, true);

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

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));
            
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

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>(); 
            sourceFolder.Stub(d => d.GetFileList()).Throw(new Exception());
            container.IFolderQueue.Enqueue(sourceFolder);

            container.IFolderQueue.Enqueue(MockRepository.GenerateStub<IFolder>());

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));

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

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(false);
            container.IFolderQueue.Enqueue(targetFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                               CreateIFileFactoryFromContainer(container));

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();
            
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Throw(new UnauthorizedAccessException());
            
            container.IFolderQueue.Enqueue(sourceFolder);
            container.IFolderQueue.Enqueue(MockRepository.GenerateStub<IFolder>());

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                               CreateIFileFactoryFromContainer(container));

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
            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(s => s.FolderExists()).Return(true);
            sourceFolder.Stub(s => s.DeleteFolder()).Throw(new UnauthorizedAccessException());

            container.IFolderQueue.Enqueue(sourceFolder);
            container.IFolderQueue.Enqueue(MockRepository.GenerateStub<IFolder>());

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                               CreateIFileFactoryFromContainer(container));

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

            QueueBasedClassFactory container = new QueueBasedClassFactory();
            container.IFolderQueue.Enqueue(sourceFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));

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
            
            QueueBasedClassFactory container = new QueueBasedClassFactory();
            container.IFolderQueue.Enqueue(sourceFolder);

            JunctionPointer.Implemenation.DirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                            CreateIFileFactoryFromContainer(container));

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

            QueueBasedClassFactory container = new QueueBasedClassFactory();
           
            IFile fileStub = Helpers.CreateStubHelpers.GetIFileStub("file1", @"c:\testDir\");
           
            IFile targetFile = Helpers.CreateStubHelpers.GetIFileStub("", "");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { fileStub });
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);

            
            container.IFolderQueue.Enqueue(sourceFolder);
            container.IFolderQueue.Enqueue(new StubDirManagerReturnSubFolderList());

            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                             CreateIFileFactoryFromContainer(container));
            testLinker.CreateSymbolicLinkFolder(pathLink, pathTo, true, true);
            
            //Assert
            targetFile.AssertWasCalled(f => f.SetFile(pathTo + "file1"));
            fileStub.AssertWasCalled(f => f.CopyFile(targetFile, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_DoNotOverwriteTargetFile_CopyFileCalledCorrectly()
        {
            //
            //Arrange
            String pathLink = @"c:\testDir\";
            String pathTo = @"c:\testTarget\";

            QueueBasedClassFactory container = new QueueBasedClassFactory();
           
            IFile fileStub = Helpers.CreateStubHelpers.GetIFileStub("file1", @"c:\testDir\");

            IFile targetFile = Helpers.CreateStubHelpers.GetIFileStub("", "");
            container.IFileQueue.Enqueue(targetFile);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile> { fileStub });
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            sourceFolder.Stub(d => d.FolderExists()).Return(true);

            container.IFolderQueue.Enqueue(sourceFolder);
            container.IFolderQueue.Enqueue(MockRepository.GenerateStub<IFolder>());

            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                             CreateIFileFactoryFromContainer(container));
            testLinker.CreateSymbolicLinkFolder(pathLink, pathTo, true, false);

            //Assert
            targetFile.AssertWasCalled(f => f.SetFile(pathTo + "file1"));
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

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFile targetFileA = Helpers.CreateStubHelpers.GetIFileStub("", "");
            container.IFileQueue.Enqueue(targetFileA);

            IFile targetFileB = Helpers.CreateStubHelpers.GetIFileStub("", "");
            container.IFileQueue.Enqueue(targetFileB);

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(s => s.GetFileList()).Return(newList);
            sourceFolder.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());

            String target = @"c:\target\";

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(t => t.FolderPath).Return(target);
            
            //
            //Act
            JunctionPointer.Implemenation.DirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                                             CreateIFileFactoryFromContainer(container));
            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert

            targetFileA.AssertWasCalled(f => f.SetFile(target + "a"));
            targetFileB.AssertWasCalled(f => f.SetFile(target + "b"));
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
            
            IFolder subFolderA = MockRepository.GenerateStub<IFolder>();
            subFolderA.Stub(d => d.FolderPath).Return(source + "a");
            subFolderA.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            subFolderA.Stub(d => d.GetFileList()).Return(new List<IFile>());
            
            IFolder subFolderB = MockRepository.GenerateStub<IFolder>();
            subFolderB.Stub(d => d.FolderPath).Return(source + "b");
            subFolderB.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            subFolderB.Stub(d => d.GetFileList()).Return(new List<IFile>());
            
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.FolderPath).Return(source);
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder> { subFolderA, subFolderB });
            
            //Queue up two target folder to assert against
            QueueBasedClassFactory container = new QueueBasedClassFactory();
            
            IFolder targetSubFolderA = MockRepository.GenerateStub<IFolder>();
            IFolder targetSubFolderB = MockRepository.GenerateStub<IFolder>();

            container.IFolderQueue.Enqueue(targetSubFolderA);
            container.IFolderQueue.Enqueue(targetSubFolderB);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);

            //
            //Act
            IDirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert
            Assert.AreEqual(targetSubFolderA.FolderPath, target + "a"); 
            targetSubFolderA.AssertWasCalled(d => d.CreateFolder());

            Assert.AreEqual(targetSubFolderB.FolderPath, target + "b"); 
            targetSubFolderB.AssertWasCalled(d => d.CreateFolder());    
        }

        [Test]
        public void CopyFolder_SubFoldersContainsFiles_CopiedCorrectly()
        {

            //
            //Arrange
            String source = @"c:\source\";
            String target = @"c:\target\";

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            container.IFolderQueue.Enqueue(new StubDirManagerReturnSubFolderList());
            container.IFolderQueue.Enqueue(new StubDirManagerReturnSubFolderList());

            IFile file1 = Helpers.CreateStubHelpers.GetIFileStub("file1", @"c:\source\a\");
            IFile file2 = Helpers.CreateStubHelpers.GetIFileStub("file2", @"c:\source\b");

            List<IFile> fileList1 = new List<IFile>();
            fileList1.Add(file1);

            List<IFile> fileList2 = new List<IFile>();
            fileList2.Add(file2);

            IFile targetFileA = MockRepository.GenerateStub<IFile>();
            container.IFileQueue.Enqueue(targetFileA);

            IFile targetFileB = MockRepository.GenerateStub<IFile>();
            container.IFileQueue.Enqueue(targetFileB);


            IFolder subFolder1 = MockRepository.GenerateStub<IFolder>();
            subFolder1.Stub(d => d.FolderPath).Return(source + "a");
            subFolder1.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            subFolder1.Stub(s => s.GetFileList()).Return(fileList1);

            IFolder subFolder2 = MockRepository.GenerateStub<IFolder>();
            subFolder2.Stub(d => d.FolderPath).Return(source + "b");
            subFolder2.Stub(s => s.GetSubFolderList()).Return(new List<IFolder>());
            subFolder2.Stub(s => s.GetFileList()).Return(fileList2);

            StubDirManagerReturnSubFolderList sourceFolder = new StubDirManagerReturnSubFolderList();
            sourceFolder.FolderPath = source;
            sourceFolder.FolderList.Add(subFolder1);
            sourceFolder.FolderList.Add(subFolder2);

            sourceFolder.OnlyReturnSubFolderListFor = sourceFolder;

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderPath).Return(target);

            //
            //Act
            IDirLinker testLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));
            testLinker.CopyFolder(sourceFolder, targetFolder, true);

            //
            //Assert
            targetFileA.AssertWasCalled(f => f.SetFile(target + @"a\file1"));
            targetFileB.AssertWasCalled(f => f.SetFile(target + @"b\file2"));

            file1.AssertWasCalled(f => f.CopyFile(targetFileA, true));
            file2.AssertWasCalled(f => f.CopyFile(targetFileB, true));
        }

        [Test]
        public void CreateSymbolicLinkFolder_TargetDoesNotExist_TargetCreated()
        {
            //Arrange
            String source = @"c:\source\";
            String target = @"c:\target";
           
            QueueBasedClassFactory container = new QueueBasedClassFactory();
            
            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderExists()).Return(false);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(source)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                               CreateIFileFactoryFromContainer(container));
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
            IFolder dirManager = MockRepository.GenerateStub<IFolder>();

            QueueBasedClassFactory container = new QueueBasedClassFactory();

            IFolder sourceFolder = MockRepository.GenerateStub<IFolder>();
            sourceFolder.Stub(d => d.GetFileList()).Return(new List<IFile>());
            sourceFolder.Stub(d => d.GetSubFolderList()).Return(new List<IFolder>());
            container.IFolderQueue.Enqueue(sourceFolder);

            IFolder targetFolder = MockRepository.GenerateStub<IFolder>();
            targetFolder.Stub(d => d.FolderExists()).Return(true);
            targetFolder.Stub(d => d.CreateLinkToFolderAt(Arg<String>.Is.Anything)).Return(true);
            container.IFolderQueue.Enqueue(targetFolder);

            IDirLinker dirLinker = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                               CreateIFileFactoryFromContainer(container));

            Boolean delegateCalled = false;
            dirLinker.UserMessage += delegate(Object sender, UserMessageArgs args)
            {
                delegateCalled = true;
                Assert.IsTrue(args.Message.ToLower().Contains("success"));
                Assert.IsTrue(args.ResponseOptions == System.Windows.Forms.MessageBoxButtons.OK);
            };

            //Act
            dirLinker.CreateSymbolicLinkFolder(@"c:\source\", target, false, true);


            //Assert
            Assert.IsTrue(delegateCalled);
        }

        [Test]
        public void ValidatePath_ValidPath_Valid()
        {
            //Arrange
            IFolder dirManager = Helpers.CreateStubHelpers.GetIDirectoryManagerStub();
            
            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));
  
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
            
            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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
            
            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));


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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));

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

            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));
            
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
            
            UnitTestClassFactory container = new UnitTestClassFactory();
            container.ReturnObjectForType<IFolder>(dirManager);

            IDirLinker controller = new JunctionPointer.Implemenation.DirLinker(CreateIFolderFactoryFromContainer(container),
                                                                                CreateIFileFactoryFromContainer(container));


            //Act
            String errorMsg;
            Boolean valid = controller.ValidDirectoryPath(@"\\network\share", out errorMsg);

            //Assert
            Assert.IsFalse(valid);
            Assert.IsNotEmpty(errorMsg);
        }
    }
}
