using System;
using JunctionPointer.Helpers.Interfaces;
using JunctionPointer.Interfaces;
using System.Collections.Generic;
using JunctionPointer.Helpers.OCInject;

namespace DirLinker.Tests.Helpers
{
    public class QueueBasedClassFactory : ClassFactory
    {

        public QueueBasedClassFactory()
        {
            IFolderQueue = new Queue<IFolder>();
            IFileQueue = new Queue<IFile>();
        }
        
        public Queue<IFolder> IFolderQueue { get; set; }
    
        public Queue<IFile> IFileQueue { get; set; }
    
        public override ITypeOptions RegisterType<TContract, TImplementation>()
        {
            return base.RegisterType<TContract, TImplementation>();
        }

        public override T ManufactureType<T>()
        {
            return ManufactureType<T>(new Object[] { }); 
        }

        public override T ManufactureType<T>(params object[] args)
        {
            if (typeof(T).Equals(typeof(IFolder)) && IFolderQueue.Count > 0)
            {
                IFolder folder = IFolderQueue.Dequeue();
                //if (args.Length == 1)
                //{
                //    (folder as FakeFolder).FolderPath = args[0] as String;
                //}
                return (T)folder;
            }
            if (typeof(T).Equals(typeof(IFile)) && IFileQueue.Count > 0)
            {
                IFile file = IFileQueue.Dequeue();
                //if (args.Length == 1)
                //{
                //    (file as FakeFile).FullFilePath = args[0] as String;
                //}
                return (T)file;
            }
            else
            {
                return (T)base.ManufactureType<T>(args);
            }
        }

    }
}
