namespace KlotosLib.Angles
{
    /// <summary>
    /// Представляет диапазоны угловых величин
    /// </summary>
    public enum AngleRanges : byte
    {
        /// <summary>
        /// Нулевой угол [0°]
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Острый угол (0°-90°)
        /// </summary>
        Acute = 1,

        /// <summary>
        /// Прямой (четвертной) угол [90°]
        /// </summary>
        Right = 2,

        /// <summary>
        /// Тупой угол (90°-180°)
        /// </summary>
        Obtuse = 3,

        /// <summary>
        /// Развёрнутый (половинный) угол [180°]
        /// </summary>
        Straight = 4,

        /// <summary>
        /// Невыпуклый угол (180°-360°)
        /// </summary>
        Reflex = 5,

        /// <summary>
        /// Полный угол [360°]
        /// </summary>
        Full = 6
    }
}