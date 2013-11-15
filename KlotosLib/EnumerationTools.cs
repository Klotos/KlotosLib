using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlotosLib
{
    /// <summary>
    /// Extension methods to make working with Enum values easier
    /// </summary>
    /// <remarks>Copyright: Hugo Bonacci. http://hugoware.net/blog/enumeration-extensions-2-0 </remarks>
    public static class EnumerationTools
    {
        #region Extension Methods
        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        public static T IncludeTo<T>(this Enum value, T append)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(append, type);
            if (parsed.Signed is Int64)
            {
                result = Convert.ToInt64(value) | (Int64)parsed.Signed;
            }
            else if (parsed.Unsigned is UInt64)
            {
                result = Convert.ToUInt64(value) | (UInt64)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        public static T DeleteFrom<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(remove, type);
            if (parsed.Signed is Int64)
            {
                result = Convert.ToInt64(value) & ~(Int64)parsed.Signed;
            }
            else if (parsed.Unsigned is UInt64)
            {
                result = Convert.ToUInt64(value) & ~(UInt64)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        public static Boolean Contains<T>(this Enum value, T check)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(check, type);
            if (parsed.Signed is Int64)
            {
                return (Convert.ToInt64(value) & (Int64)parsed.Signed) == (Int64)parsed.Signed;
            }
            else if (parsed.Unsigned is UInt64)
            {
                return (Convert.ToUInt64(value) & (UInt64)parsed.Unsigned) == (UInt64)parsed.Unsigned;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        public static Boolean Misses<T>(this Enum obj, T value)
        {
            return !EnumerationTools.Contains<T>(obj, value);
        }

        #region Helper Classes
        //class to simplfy narrowing values between 
        //a ulong and long since either value should
        //cover any lesser value
        private class _Value
        {

            //cached comparisons for tye to use
            private static readonly Type _UInt64 = typeof(UInt64);
            private static readonly Type _UInt32 = typeof(Int64);

            public readonly Int64? Signed;
            public readonly UInt64? Unsigned;

            public _Value(object value, Type type)
            {

                //make sure it is even an enum to work with
                if (!type.IsEnum)
                {
                    throw new ArgumentException("Value provided is not an enumerated type!", "type");
                }

                //then check for the enumerated value
                Type compare = Enum.GetUnderlyingType(type);

                //if this is an unsigned long then the only
                //value that can hold it would be a ulong
                if (compare.Equals(_Value._UInt32) || compare.Equals(_Value._UInt64))
                {
                    this.Unsigned = Convert.ToUInt64(value);
                }
                //otherwise, a long should cover anything else
                else
                {
                    this.Signed = Convert.ToInt64(value);
                }
            }
        }
        #endregion
        #endregion
    }
}
