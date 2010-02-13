using System;
using System.Collections.Generic;

namespace DirLinker.Tests.Helpers
{
    public class UnitTestClassFactory : JunctionPointer.Helpers.OCInject.ClassFactory
    {
        protected Dictionary<Type, Object> m_ObjectList = new Dictionary<Type,Object>();

        public void ReturnObjectForType<T>(Object returnedObject)
        {
            m_ObjectList.Add(typeof(T), returnedObject);
        }

        public override T ManufactureType<T>()
        {
            if (m_ObjectList.ContainsKey(typeof(T)))
            {
                return (T)m_ObjectList[typeof(T)];
            }
            else
            {
                T returnedObject = default(T);
                try
                {
                    returnedObject = base.ManufactureType<T>();
                }
                catch (ArgumentException) 
                { 
                    //This means we don't know about the type so just return the default of true
                }
                return returnedObject;
            }
        }

    }
}
