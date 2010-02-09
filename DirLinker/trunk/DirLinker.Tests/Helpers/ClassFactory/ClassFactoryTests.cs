using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using JunctionPointer.Helpers.Interfaces;

namespace DirLinker.Tests.Helpers.ClassFactory
{
    [TestFixture]
    public class ClassFactoryTests
    {
        interface ITestClass { }
        class TestClass : ITestClass { }

        interface IDepend { }
        class Depend : IDepend { }

        interface ITestClassWithDepend { IDepend Depend {get; set;} }
        class TestClassWithDepend : ITestClassWithDepend
        {
            public IDepend Depend {get; set;}

            public TestClassWithDepend(IDepend idepend)
            {
                Depend = idepend;
            }
        }

        interface ITestClassWithDelegateFactory { ITestClassFactory Factory { get; set; } }
        delegate ITestClass ITestClassFactory();

        class TestClassWithDelegateFactory : ITestClassWithDelegateFactory
        {
            public ITestClassFactory Factory { get; set; }

            public TestClassWithDelegateFactory(ITestClassFactory delegateFactory)
            {
                Factory = delegateFactory;
            }
        }

        delegate ITestClass TestClassMultiParamFactory(String paramOne, String param2);
        interface ITestClassMultiParamFactory { TestClassMultiParamFactory Factory { get; set; } }

        class TestClassMultiParam : ITestClassMultiParamFactory
        {
            public TestClassMultiParamFactory Factory { get; set; }

            public TestClassMultiParam(TestClassMultiParamFactory delegateFactory)
            {
                Factory = delegateFactory;
            }
        }

        [Test]
        public void ManufactureType_ClassNoDependecies_CreatedSuccessfully()
        {

            IClassFactory testClssFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();
            testClssFactory.RegisterType<ITestClass, TestClass>();

            ITestClass manufacturedType = testClssFactory.ManufactureType<ITestClass>();


            Assert.IsTrue(manufacturedType is TestClass);
        }

        [Test]
        public void ManufactureType_ClassWithDespendecies_CreatedSuccessfully()
        {
            IClassFactory testClassFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();

            testClassFactory.RegisterType<IDepend, Depend>();
            testClassFactory.RegisterType<ITestClassWithDepend, TestClassWithDepend>();

            ITestClassWithDepend manufacturedType = testClassFactory.ManufactureType<ITestClassWithDepend>();

            Assert.IsTrue(manufacturedType is TestClassWithDepend);
            Assert.IsTrue(manufacturedType.Depend is Depend);
        }

        [Test]
        public void ManufactureType_Type_accepts_delegate_factory_past_correctly()
        {
            IClassFactory testClassFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();

            testClassFactory.RegisterType<ITestClass, TestClass>()
                .WithFactory<ITestClassFactory>();
            testClassFactory.RegisterType<ITestClassWithDelegateFactory, TestClassWithDelegateFactory>();

            ITestClassWithDelegateFactory manufacturedType = testClassFactory.ManufactureType<ITestClassWithDelegateFactory>();

            Assert.That(manufacturedType.Factory != null);

        }

        [Test]
        public void ManufactureType_Type_delegate_factory_manufactures_correct_type()
        {
            IClassFactory testClassFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();

            testClassFactory.RegisterType<ITestClass, TestClass>()
                .WithFactory<ITestClassFactory>();
            testClassFactory.RegisterType<ITestClassWithDelegateFactory, TestClassWithDelegateFactory>();

            ITestClassWithDelegateFactory manufacturedType = testClassFactory.ManufactureType<ITestClassWithDelegateFactory>();
            ITestClass instance = manufacturedType.Factory();

            Assert.IsInstanceOf(typeof(TestClass), instance);

        }

        [Test]
        public void ManfactureType_Type_with_delegate_factory_that_accepts_two_params()
        {
            IClassFactory testClassFactory = new JunctionPointer.Helpers.ClassFactory.ClassFactory();

            testClassFactory.RegisterType<ITestClass, TestClass>()
                .WithFactory<TestClassMultiParamFactory>();
            testClassFactory.RegisterType<ITestClassMultiParamFactory, TestClassMultiParam>();

            ITestClassMultiParamFactory manufacturedType = testClassFactory.ManufactureType<ITestClassMultiParamFactory>();

            Assert.That(manufacturedType.Factory != null);
        }
    }
}
