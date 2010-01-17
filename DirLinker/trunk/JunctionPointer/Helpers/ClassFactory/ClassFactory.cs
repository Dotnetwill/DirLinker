using System;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;

namespace JunctionPointer.Helpers.ClassFactory
{
    public class NoCurrentFactoryException : Exception
    {
        public NoCurrentFactoryException()
            : base("No class factory is specified")
        { }
    }

    public class ClassFactory : IClassFactory
    {
        public static IClassFactory CurrentFactory { get; set; }
 
        public static T CreateInstance<T>()
        {
            if (CurrentFactory == null)
            {
                throw new NoCurrentFactoryException();
            }
            return CurrentFactory.ManufactureType<T>();
        }

        private readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();

        public virtual void RegisterType<TContract, TImplementation>()
        {
            types[typeof(TContract)] = typeof(TImplementation);
        }

        public virtual T ManufactureType<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public virtual object Resolve(Type contract)
        {
            if (types.ContainsKey(contract))
            {
                Type implementation = types[contract];

                ConstructorInfo constructor = implementation.GetConstructors()[0];

                ParameterInfo[] constructorParameters = constructor.GetParameters();

                if (constructorParameters.Length == 0)
                    return Activator.CreateInstance(implementation);

                List<object> parameters = new List<object>(constructorParameters.Length);

                foreach (ParameterInfo parameterInfo in constructorParameters)
                    parameters.Add(Resolve(parameterInfo.ParameterType));

                return constructor.Invoke(parameters.ToArray());
            }
            throw new ArgumentException("contract is not a known type");
        }
    }
}
