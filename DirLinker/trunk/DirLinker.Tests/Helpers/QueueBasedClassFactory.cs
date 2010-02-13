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
            throw new NotImplementedException();
        }

        public override T ManufactureType<T>()
        {
            if (typeof(T).Equals(typeof(IFolder)))
            {
                return (T)IFolderQueue.Dequeue();
            }
            if (typeof(T).Equals(typeof(IFile)))
            {
                return (T)IFileQueue.Dequeue();
            }

            else
            {
                return default(T);
            }
        }

        public override T ManufactureType<T>(params object[] args)
        {
            return ManufactureType<T>();
        }

    }
}
