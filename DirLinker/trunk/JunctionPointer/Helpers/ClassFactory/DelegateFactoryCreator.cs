using System;
using System.Linq;
using System.Collections.Generic;
using JunctionPointer.Helpers.Interfaces;
using System.Reflection;
using System.Linq.Expressions;

namespace JunctionPointer.Helpers.ClassFactory
{
    public class DelegateFactoryCreator
    {
        IClassFactory _ClassFactory;

        public DelegateFactoryCreator(IClassFactory classFactory)
        {
            _ClassFactory = classFactory;
        }

        public virtual void RegisterDelegateFactoryForType<TResult, TFactoryDelegateType>()
        {
            MethodInfo delegateInvoker = typeof(TFactoryDelegateType).GetMethod("Invoke");
            ParameterExpression[] factoryParams = GetParamsAsExpressions(delegateInvoker);

            //Build the factory from the template
            MethodInfo mi = typeof(DelegateFactoryCreator).GetMethod("FactoryTemplate");
            mi = mi.MakeGenericMethod(typeof(TResult));

            Expression call = Expression.Call(mi, new Expression[] {Expression.Constant(_ClassFactory), 
                Expression.NewArrayInit(typeof(Object), factoryParams)});

            TFactoryDelegateType factory = Expression.Lambda<TFactoryDelegateType>(call, factoryParams).Compile();

            _ClassFactory.AddFactory(typeof(TFactoryDelegateType), factory as Delegate);
        }

        private ParameterExpression[] GetParamsAsExpressions(MethodInfo mi)
        {
            List<ParameterExpression> paramsAsExpression = new List<ParameterExpression>();

            Array.ForEach<ParameterInfo>(mi.GetParameters(),
                p => paramsAsExpression.Add(Expression.Parameter(p.ParameterType, p.Name)));

            return paramsAsExpression.ToArray();
        }

        public static T FactoryTemplate<T>(ClassFactory factory, params Object[] args)
        {
            return factory.ManufactureType<T>(args);
        }

        //public static TResult FactoryTemplate<T1, TResult>(ClassFactory factory, T1 param1)
        //{
        //    return factory.ManufactureType<TResult>();
        //
        //}

        //public static TResult FactoryTemplate<T1, T2, TResult>(ClassFactory factory, T1 param1, T2 param2)
        //{
        //    return factory.ManufactureType<TResult>();
        //}

        //public static TResult FactoryTemplate<T1, T2, T3, TResult>(ClassFactory factory, T1 param1, T2 param2, T3 param3)
        //{
        //    return factory.ManufactureType<TResult>();
        //
        //}

    }
}
