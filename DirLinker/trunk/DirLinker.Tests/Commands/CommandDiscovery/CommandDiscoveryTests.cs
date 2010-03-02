using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JunctionPointer.Interfaces;
using JunctionPointer.Commands;
using Rhino.Mocks;
using stupidNameClass = JunctionPointer.Commands.CommandDiscovery;

namespace DirLinker.Tests.Commands.CommandDiscovery
{

    public class MockCommand : ICommand
    {
        public Object[] ctorParams;
        public String CommandName { get; set; }

        public MockCommand(params Object[] constructorParams)
        {
            ctorParams = constructorParams;
        }

        #region ICommand Members

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string UserFeedback
        {
            get { throw new NotImplementedException(); }
        }

        public event RequestUserReponse AskUser;

        #endregion
    }

    public class MockCommandFactory : ICommandFactory
    {

        public ICommand CreateLinkCommand(IFolder linkTo, IFolder linkFrom)
        {
            return new MockCommand(linkTo, linkFrom) { CommandName = "CreateLinkCommand" };
        }

    }

    [TestFixture]
    public class CommandDiscoveryTests
    {

        [Test]
        public void GetCommandListForTask_target_does_not_exist_just_ICreateLinkCasommand_returned()
        {
            ICommandFactory factory = new MockCommandFactory();
            IFolder linkTo = MockRepository.GenerateMock<IFolder>();
            linkTo.Stub(l => l.FolderExists()).Return(false);

            IFolder linkFrom = MockRepository.GenerateMock<IFolder>();

            ICommandDiscovery discoverer = new stupidNameClass.CommandDiscovery(factory);
            List<ICommand> taskList = discoverer.GetCommandListForTask(linkTo, linkFrom, false, false);


        }
    }
}
