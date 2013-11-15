using System;

namespace KlotosLib
{
    /// <summary>
    /// Содержит статические методы и методы расширения по работе с экземплярами делегатов
    /// </summary>
    public static class DelegateTools
    {
        /// <summary>
        /// Возвращает количество простых делегатов в списке вызова данного экземпляра делегата.
        /// </summary>
        /// <param name="Del"></param>
        /// <returns></returns>
        public static Int32 SimpleDelegatesCount(this System.Delegate Del)
        {
            if (Del == null) { return 0; }
            return Del.GetInvocationList().Length;
        }

        /// <summary>
        /// Определяет, является ли метод, на который указывает данный экземпляр делегата, статическим или экземплярным. 
        /// Если данный экземпляр делегата является комбинированным, метод выполняется по отношению к последнему простому делегаты в списке вызова.
        /// </summary>
        /// <param name="Del"></param>
        /// <param name="ThrowExIfNull">Определяет, выбрасывать ли исключение (true) или нет (false), если данный экземпляр делегата является null. 
        /// Если не выбрасывать и экземпляр делегата является null, метод возвратит false.</param>
        /// <returns></returns>
        public static Boolean IsStaticDelegateMethod(this System.Delegate Del, Boolean ThrowExIfNull)
        {
            if (Del == null)
            {
                if (ThrowExIfNull == true) { throw new ArgumentNullException("Del", "Список вызовов переданного экземпляра делегата пуст"); }
                else { return false; }
            }
            return Del.Target == null;
        }
    }
}
