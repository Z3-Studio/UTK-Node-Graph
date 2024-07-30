using System;

namespace Z3.NodeGraph.Core
{
    public interface ITypeConverter { }

    /// <summary>
    /// Used to create custom convertions for different types
    /// </summary>
    /// <remarks> Example:
    /// <para> parameter.get => (parameterType)variable </para>
    /// <para> parameter.set => variable = (variableType)value </para>
    /// </remarks>
    /// <typeparam name="TOut"> To new Type </typeparam>
    /// <typeparam name="TIn"> From old Type </typeparam>
    public interface ITypeConverter<TOut, TIn> : ITypeConverter
    {
        TOut Convert(TIn source);
    }

    public interface ITypeConveterCreator<TOut, TIn> : ITypeConverter
    {
        // Ideas
        //void CreateCustomSet(TVariable variable, TValue newValue);

        Func<TOut, TIn> CreateConverter(Type fromType, Type toType);
    }
}