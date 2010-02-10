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
            return ManufactureType<T>(new Object[] {});
        }
        public virtual T ManufactureType<T>(params Object[] args)
        {
            return (T)Resolve(typeof(T), args);
        }

        public virtual object Resolve(Type contract, params Object[] args)
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
                    Boolean found = false;
                    foreach (Object o in args)
                    {
                        if (parameterInfo.ParameterType.IsAssignableFrom(o.GetType()))
                        {
                            parameters.Add(o);
                            found = true;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }
                    else if (_typeFactories.ContainsKey(parameterInfo.ParameterType))
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
            MethodInfo delegateInvoker = typeof(TFactoryDelegateType).GetMethod("Invoke");
            List<ParameterExpression> factoryParams = GetParamsAsExpressions(delegateInvoker);

            //Build the factory from the template
            MethodInfo mi = typeof(ClassFactory).GetMethod("FactoryTemplate");
            mi = mi.MakeGenericMethod(typeof(TResult));

            Expression call = Expression.Call(mi, new Expression[] {Expression.Constant(this), 
                Expression.NewArrayInit(typeof(Object), factoryParams.ToArray())} );

            TFactoryDelegateType factory = Expression.Lambda<TFactoryDelegateType>(call, factoryParams.ToArray()).Compile();

            _typeFactories.Add(typeof(TFactoryDelegateType), factory as Delegate);
        }

        private List<ParameterExpression> GetParamsAsExpressions(MethodInfo mi)
        {
            List<ParameterExpression> paramsAsExpression = new List<ParameterExpression>();

            Array.ForEach<ParameterInfo>(mi.GetParameters(),
                p => paramsAsExpression.Add(Expression.Parameter(p.ParameterType, p.Name)));

            return paramsAsExpression;
        }

        public static T FactoryTemplate<T>(ClassFactory factory, params Object[] args)
        {
            return factory.ManufactureType<T>(args);
        }
    }
}
