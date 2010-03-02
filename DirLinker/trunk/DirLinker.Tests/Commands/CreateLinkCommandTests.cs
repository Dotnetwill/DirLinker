using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JunctionPointer.Commands;
using JunctionPointer.Interfaces;
using Rhino.Mocks;
using JunctionPointer.Exceptions;

namespace DirLinker.Tests.Commands
{
    [TestFixture]
    public class CreateLinkCommandTests
    {
        [Test]
        public void Execute_link_is_created()
        {
            String linkToPath = @"testPath";
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(f => f.FolderExists()).Return(false);
            linkTo.Stub(f => f.FolderPath).Return(linkToPath);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(f => f.FolderExists()).Return(true);

            CreateLinkCommand command = new CreateLinkCommand(linkTo, linkFrom);

            command.Execute();

            linkFrom.AssertWasCalled(f => f.CreateLinkToFolderAt(linkToPath));                   
        }

        [Test]
        public void Execute_linkTo_folder_still_exists_exception_is_thrown()
        {
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(f => f.FolderExists()).Return(true);
            linkTo.Stub(f => f.FolderPath).Return(@"testPath");

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(f => f.FolderExists()).Return(true);

            CreateLinkCommand command = new CreateLinkCommand(linkTo, linkFrom);

            Assert.Throws<DirLinkerException>( () => command.Execute());
        }

        [Test]
        public void Undo_throws_not_supported_exception()
        {
            CreateLinkCommand command = new CreateLinkCommand(null, null);

            Assert.Throws<NotSupportedException>(() => command.Undo());
        }

        [Test]
        public void Status_returns_a_status_method()
        {
            CreateLinkCommand command = new CreateLinkCommand(null, null);

            Assert.IsNotEmpty(command.Status);
        }
    }
}
