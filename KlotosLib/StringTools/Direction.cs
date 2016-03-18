using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlotosLib.StringTools
{
    /// <summary>
    /// Определяет направление поиска или выдачи подстроки в строке
    /// </summary>
    public enum Direction : byte
    {
        /// <summary>
        /// Искать/выдавать в направлении от начала до конца строки
        /// </summary>
        FromStartToEnd,

        /// <summary>
        /// Искать/выдавать в направлении с конца к началу строки
        /// </summary>
        FromEndToStart
    }
}
