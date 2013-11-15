using System;
using System.Linq.Expressions;

namespace KlotosLib
{
    /// <summary>
    /// Содержит различные утилитаные методы, использующие рефлексию и/или деревья выражений
    /// </summary>
    public static class LambdaExtensions
    {
        /// <summary>
        /// Возвращает текстовое название указанного члена данного объекта
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Expr"></param>
        /// <returns></returns>
        public static string MemberName<T, R>(this T Source, Expression<Func<T, R>> Expr)
        {
            var node = Expr.Body as MemberExpression;
            if (Object.ReferenceEquals(null, node))
                throw new InvalidOperationException("Expression must be of member access");
            return node.Member.Name;
        }
    }
}
