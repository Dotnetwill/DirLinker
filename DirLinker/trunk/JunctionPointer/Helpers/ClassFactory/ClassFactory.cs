using System;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

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
        public class TypeOptions<TClass> : ITypeOptions 
        {
            private IClassFactory _currentFactory;

            public TypeOptions(IClassFactory classFactory)
            {
                _currentFactory = classFactory;    
            }
      
            public ITypeOptions WithFactory<T>()
            {
                _currentFactory.RegisterDelegateFactoryForType<TClass, T>();
                return this;
            }
        }

        public static IClassFactory CurrentFactory { get; set; }
 
        public static T CreateInstance<T>()
        {
            if (CurrentFactory == null)
            {
                throw new NoCurrentFactoryException();
            }
            return CurrentFactory.ManufactureType<T>();
        }

        private readonly IDictionary<Type, Type> _types = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, Delegate> _typeFactories = new Dictionary<Type, Delegate>();

        public virtual ITypeOptions RegisterType<TContract, TImplementation>()
        {
            _types[typeof(TContract)] = typeof(TImplementation);

            return new TypeOptions<TContract>(this);
        }

        public virtual T ManufactureType<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public virtual object Resolve(Type contract)
        {
            if (_types.ContainsKey(contract))
            {
                Type implementation = _types[contract];

                ConstructorInfo constructor = implementation.GetConstructors()[0];

                ParameterInfo[] constructorParameters = constructor.GetParameters();

                if (constructorParameters.Length == 0)
                    return Activator.CreateInstance(implementation);

                List<object> parameters = new List<object>(constructorParameters.Length);

                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    if (_typeFactories.ContainsKey(parameterInfo.ParameterType))
                    {
                        parameters.Add(_typeFactories[parameterInfo.ParameterType]);
                    }
                    else
                    {
                        parameters.Add(Resolve(parameterInfo.ParameterType));
                    }
                }

                return constructor.Invoke(parameters.ToArray());
            }
            throw new ArgumentException("contract is not a known type");
        }

        public virtual void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>()
        {
           //var expressParam = new Expression[]
           //{
           //    Expression.co
           //};
           // Expression.Lambda(typeof(TFactoryDelegateType), 
           //   Expression.Call(typeof(ClassFactory).GetMethod("GenericFactoryTemplate"), )
           //     null);

           // MethodInfo.
           // 

            //Expression<Func<TResult>> exp = () => ManufactureType<TResult>();
            //
            //LambdaExpression lamba = exp as LambdaExpression;
            //Delegate del = lamba.Compile();
            //MethodInfo mi = del.Method;
            ////MethodInfo mi = (lamba.Body as MethodCallExpression).Method;

            MethodInfo mi = typeof(ClassFactory).GetMethod("ManufactureType", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(typeof(TResult));
            
            Delegate factory = Delegate.CreateDelegate(typeof(TFactoryDelegateType), mi);

            _typeFactories.Add(typeof(TFactoryDelegateType), factory);
        }
           
  
    }
}
