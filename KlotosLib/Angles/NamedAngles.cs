namespace KlotosLib.Angles
{
    /// <summary>
    /// Представляет распространённые именованные угловые величины
    /// </summary>
    public enum NamedAngles : byte
    {
        /// <summary>
        /// Нулевой угол (0°)
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Половинно-четвертной угол (45°)
        /// </summary>
        HalfQuarter = 1,

        /// <summary>
        /// Прямой (четвертной) угол (90°)
        /// </summary>
        Right = 2,

        /// <summary>
        /// Тройной половинно-четвертной угол (135°)
        /// </summary>
        TripleHalfQuarter = 3,

        /// <summary>
        /// Развёрнутый (половинный) угол (180°)
        /// </summary>
        Straight = 4,

        /// <summary>
        /// Трёх-четвертной угол (270°)
        /// </summary>
        TripleQuarters = 5,

        /// <summary>
        /// Полный угол (360°)
        /// </summary>
        Full = 6
    }
}