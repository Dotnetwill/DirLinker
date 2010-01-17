using System;

namespace JunctionPointer.Helpers.Interfaces
{
    public interface IClassFactory
    {
        void RegisterType<TContract, TImplementation>();
        T ManufactureType<T>();
    }
}
