using System;

namespace KlotosLib.Angles
{
    
    /// <summary>
    /// Инкапсулирует один угол вращения, включая его величину, единицу измерения, количество оборотов и направление поворота 
    /// (определяемое положительной или отрицательной величиной угла). 
    /// Поддерживает конвертацию угловой величины в разные единицы измерений. Неизменяемый значимый тип.
    /// </summary>
    /// <remarks>Для сохранения точности величины угла экземпляр содержит её в той единице измерения, в которой экземпляр был создан. 
    /// Выполняет преобразования в необходимые единицы на лету, не изменяя изначально запомненную единицу измерения.</remarks>
    [Serializable()]
    public struct RotationAngle : IEquatable<RotationAngle>, IComparable<RotationAngle>, IComparable, ICloneable, IFormattable
    {
        #region Constants

        private const String _errorMesMuNotInit = "Specified measurement unit is not initialized";

        private const String _errorMesInvalidNumber = "Input angle value number is NaN or infinity";

        private const Double _threshold = 0.0000001;
        
        #endregion Constants

        #region Fields

        /// <summary>
        /// Величина угла. Положительные значение описывают вращение по часовой стрелке, отрицательные - против. 
        /// Может превышать величину одного оборота.
        /// </summary>
        private readonly Double _value;

        /// <summary>
        /// Единица измерения угла
        /// </summary>
        private readonly MeasurementUnit _measurementUnit;

        #endregion Fields

        #region Constructors

        private RotationAngle(RotationAngle other)
        {
            this._value = other._value;
            this._measurementUnit = other._measurementUnit;
        }

        private RotationAngle(Double value, MeasurementUnit measurementUnit)
        {
            this._value = value;
            this._measurementUnit = measurementUnit;
        }
        #endregion Constructors

        #region Static factories

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в указанной единице измерения углов
        /// </summary>
        /// <param name="inputValue">Величина угла. Если отрицательная или превышает один полный оборот в заданных единицах измерения углов — 
        /// будет нормализирована.</param>
        /// <param name="measurementUnit">Единица измерения углов, в которой указана величина <paramref name="inputValue"/>.</param>
        /// <returns></returns>
        public static RotationAngle FromSpecifiedUnit(Double inputValue, MeasurementUnit measurementUnit)
        {
            if (Double.IsNaN(inputValue) || Double.IsInfinity(inputValue))
            {
                throw new ArgumentOutOfRangeException("inputValue", inputValue, _errorMesInvalidNumber);
            }
            if (NumericTools.IsZero(inputValue) && measurementUnit.IsInitialized == false)
            {
                return new RotationAngle(0.0, measurementUnit);
            }
            if (measurementUnit.IsInitialized == false)
            {
                throw new ArgumentException(_errorMesMuNotInit, "measurementUnit");
            }
            return new RotationAngle(inputValue, measurementUnit);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в оборотах
        /// </summary>
        /// <param name="turns"></param>
        /// <returns></returns>
        public static RotationAngle FromTurns(Double turns)
        {
            if (Double.IsNaN(turns) || Double.IsInfinity(turns))
            {
                throw new ArgumentOutOfRangeException("turns", turns, _errorMesInvalidNumber);
            }
            return new RotationAngle(turns, MeasurementUnit.Turns);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в радианах
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static RotationAngle FromRadians(Double radians)
        {
            if (Double.IsNaN(radians) || Double.IsInfinity(radians))
            {
                throw new ArgumentOutOfRangeException("radians", radians, _errorMesInvalidNumber);
            }
            return new RotationAngle(radians, MeasurementUnit.Radians);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градусах
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static RotationAngle FromDegrees(Double degrees)
        {
            if (Double.IsNaN(degrees) || Double.IsInfinity(degrees))
            {
                throw new ArgumentOutOfRangeException("degrees", degrees, _errorMesInvalidNumber);
            }
            return new RotationAngle(degrees, MeasurementUnit.Degrees);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в двоичных градусах
        /// </summary>
        /// <param name="binaryDegrees"></param>
        /// <returns></returns>
        public static RotationAngle FromBinaryDegrees(Double binaryDegrees)
        {
            if (Double.IsNaN(binaryDegrees) || Double.IsInfinity(binaryDegrees))
            {
                throw new ArgumentOutOfRangeException("binaryDegrees", binaryDegrees, _errorMesInvalidNumber);
            }
            return new RotationAngle(binaryDegrees, MeasurementUnit.BinaryDegrees);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в квадрантах
        /// </summary>
        /// <param name="quadrants"></param>
        /// <returns></returns>
        public static RotationAngle FromQuadrants(Double quadrants)
        {
            if (Double.IsNaN(quadrants) || Double.IsInfinity(quadrants))
            {
                throw new ArgumentOutOfRangeException("quadrants", quadrants, _errorMesInvalidNumber);
            }
            return new RotationAngle(quadrants, MeasurementUnit.Quadrants);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в секстантах
        /// </summary>
        /// <param name="sextants"></param>
        /// <returns></returns>
        public static RotationAngle FromSextants(Double sextants)
        {
            if (Double.IsNaN(sextants) || Double.IsInfinity(sextants))
            {
                throw new ArgumentOutOfRangeException("sextants", sextants, _errorMesInvalidNumber);
            }
            return new RotationAngle(sextants, MeasurementUnit.Sextants);
        }

        /// <summary>
        /// Возвращает новый экземпляр угла, заданный указанным значением в градах (градианах, гонах)
        /// </summary>
        /// <param name="grads"></param>
        /// <returns></returns>
        public static RotationAngle FromGrads(Double grads)
        {
            if (Double.IsNaN(grads) || Double.IsInfinity(grads))
            {
                throw new ArgumentOutOfRangeException("grads", grads, _errorMesInvalidNumber);
            }
            return new RotationAngle(grads, MeasurementUnit.Grads);
        }

        /// <summary>
        /// Возвращает нулевой угол без величины (угол по умолчанию)
        /// </summary>
        public static readonly RotationAngle Zero = RotationAngle.FromSpecifiedUnit(0.0, MeasurementUnit.Uninitialized);
        #endregion Static factories

        #region Instance properties

        /// <summary>
        /// Показывает, содержит ли данный угол вращения безразмерный ноль, т.е. является нулевым и при этом без определённой единицы измерения. 
        /// Получить подобный экземпляр угла можно при помощи поля <see cref="Zero"/> или вызвав пустой конструктор по-умолчанию.
        /// </summary>
        public Boolean IsUnitlessZero { get { return NumericTools.IsZero(this._value) && this._measurementUnit.IsInitialized == false; } }

        /// <summary>
        /// Определяет, является ли данный угол вращения нулевым
        /// </summary>
        public Boolean IsZero
        {
            get { return NumericTools.IsZero(this._value); }
        }

        /// <summary>
        /// Возвращает единицу измерения данного угла
        /// </summary>
        public MeasurementUnit Unit { get { return this._measurementUnit; } }

        /// <summary>
        /// Возвращает величину данного угла
        /// </summary>
        public Double Value { get { return this._value; } }

        /// <summary>
        /// Определяет, является ли угол положительным, т.е. с направлением вращения по часовой стрелке
        /// </summary>
        public Boolean IsClockwise
        {
            get { return this._value > 0; }
        }

        /// <summary>
        /// Определяет, является ли угол отрицательным, т.е. с направлением вращения против часовой стрелки
        /// </summary>
        public Boolean IsCounterClockwise
        {
            get { return this._value < 0; }
        }

        /// <summary>
        /// Вовзращает количество полных оборотов для данного угла. Если величина угла меньше одного оборота, возвращает 0.
        /// </summary>
        public UInt16 NumberOfFullTurns
        {
            get
            {
                UInt16 output;
                Double modulus = Math.Abs(this._value);
                if (this.IsZero || modulus < this._measurementUnit.PositionsInOneTurn)
                {
                    output = 0;
                }
                else
                {
                    output = (UInt16)(modulus / this._measurementUnit.PositionsInOneTurn);
                }
                return output;
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в оборотах
        /// </summary>
        public Double InTurns
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Turns, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в радианах
        /// </summary>
        public Double InRadians
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Radians, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градусах
        /// </summary>
        public Double InDegrees
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Degrees, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в двоичных градусах
        /// </summary>
        public Double InBinaryDegrees
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.BinaryDegrees, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в квадрантах
        /// </summary>
        public Double InQuadrants
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Quadrants, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в секстантах
        /// </summary>
        public Double InSextants
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Sextants, this._value);
                }
            }
        }

        /// <summary>
        /// Возвращает величину данного угла в градах (градианах, гонах)
        /// </summary>
        public Double InGrads
        {
            get
            {
                if (this.IsZero)
                {
                    return this._value;
                }
                else
                {
                    return MeasurementUnit.ConvertFromTo(this._measurementUnit, MeasurementUnit.Grads, this._value);
                }
            }
        }
        #endregion Instance properties

        #region Instance methods
        /// <summary>
        /// Возвращает величину данного угла в указанных единицах измерения
        /// </summary>
        /// <param name="measurementUnit"></param>
        /// <returns></returns>
        public Double GetValueInUnit(MeasurementUnit measurementUnit)
        {
            if (this.IsZero)
            {
                return this._value;
            }
            else if (measurementUnit == MeasurementUnit.Turns)
            {
                return this.InTurns;
            }
            else if (measurementUnit == MeasurementUnit.Radians)
            {
                return this.InRadians;
            }
            else if (measurementUnit == MeasurementUnit.Degrees)
            {
                return this.InDegrees;
            }
            else if (measurementUnit == MeasurementUnit.BinaryDegrees)
            {
                return this.InBinaryDegrees;
            }
            else if (measurementUnit == MeasurementUnit.Quadrants)
            {
                return this.InQuadrants;
            }
            else if (measurementUnit == MeasurementUnit.Sextants)
            {
                return this.InSextants;
            }
            else if (measurementUnit == MeasurementUnit.Grads)
            {
                return this.InGrads;
            }
            else
            {
                throw new ArgumentException(_errorMesMuNotInit, "measurementUnit");
            }
        }

        /// <summary>
        /// Извлекает геометрический угол из данного угла вращения согласно указанному параметру и возвращает его. 
        /// Сохраняет текущую единицу измерения в возвращаемом угле.
        /// </summary>
        /// <param name="useModulus">Определяет, как обрабатывать величину отрицательного угла: преобразовывать по модулю (true) или дополнять (false). 
        /// Также см. примеры</param>
        /// <example>Примеры преобразования:
        /// 45°.GetGeometricAngle(true) -> 45°
        /// 45°.GetGeometricAngle(false) -> 45°
        /// 405°.GetGeometricAngle(true) -> 405° - 360° -> 45°
        /// 405°.GetGeometricAngle(false) -> 405° - 360° -> 45°
        /// -45°.GetGeometricAngle(true) -> |-45|° -> 45°
        /// -45°.GetGeometricAngle(false) -> 360° - 45° -> 315°
        /// -405°.GetGeometricAngle(true) -> -405° + 360° -> |-45|° -> 45°
        /// -405°.GetGeometricAngle(false) -> -405° + 360° -> 360° - 45° -> 315°
        /// </example>
        /// <returns></returns>
        public GeometricAngle GetGeometricAngle(Boolean useModulus)
        {
            if (this.IsZero)
            {
                return GeometricAngle.FromSpecifiedUnit(0.0, this._measurementUnit);
            }
            if (useModulus)
            {
                return GeometricAngle.FromSpecifiedUnit(Math.Abs(this._value), this._measurementUnit);
            }
            else
            {
                return GeometricAngle.FromSpecifiedUnit(this._value, this._measurementUnit);
            }
        }
        #endregion Instance methods

        #region Comparable and Equatable

        /// <summary>
        /// Определяет, равны ли два указанных угла вращения, вне зависимости от того, в каких единицах измерения углов представлены их величины. 
        /// Два угла считаются равны, если равны их величины (вне зависимости от единицы измерения), количество и направление оборотов.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean AreEqual(RotationAngle first, RotationAngle second)
        {
            Boolean firstIsZero = first.IsZero;
            Boolean secondIsZero = second.IsZero;
            if (firstIsZero == true && secondIsZero == true)
            {
                return true;
            }
            else if (firstIsZero == true || secondIsZero == true)
            {
                return false;
            }

            if (first._measurementUnit == second._measurementUnit)
            {
                return NumericTools.AreEqual(_threshold, first._value, second._value);
            }
            else
            {
                return NumericTools.AreEqual(_threshold, first._value, second.GetValueInUnit(first._measurementUnit));
            }
        }

        /// <summary>
        /// Определяет, равны ли между собой все указанные углы, вне зависимости от того, в каких единицах измерения углов представлены их величины
        /// </summary>
        /// <param name="rotationAngles">Набор углов для оценки равенства. Не может быть NULL, пустым или содержать только один угол.</param>
        /// <returns></returns>
        public static Boolean AreEqual(params RotationAngle[] rotationAngles)
        {
            if (rotationAngles == null) { throw new ArgumentNullException("rotationAngles"); }
            if (rotationAngles.Length == 0) { throw new ArgumentException("Набор углов не может быть пуст", "rotationAngles"); }
            if (rotationAngles.Length == 1) { throw new ArgumentException("Набор углов не может содержать только один угол", "rotationAngles"); }
            if (rotationAngles.Length == 2) { return RotationAngle.AreEqual(rotationAngles[0], rotationAngles[1]); }

            for (Int32 firstIndex = 0, secondIndex = firstIndex + 1; secondIndex < rotationAngles.Length; firstIndex++, secondIndex++)
            {
                if (RotationAngle.AreEqual(rotationAngles[firstIndex], rotationAngles[secondIndex]) == false)
                { return false; }
            }
            return true;
        }

        /// <summary>
        /// Сравнивает между собой два угла вращения вне зависимости от того, в каких единицах измерения углов представлены их величины, 
        /// и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="first">Первый сравниваемый угол</param>
        /// <param name="second">Второй сравниваемый угол</param>
        /// <returns>'1' - первый угол больше второго; '-1' - первый угол меньше второго; '0' - оба угла равны между собой</returns>
        public static Int32 Compare(RotationAngle first, RotationAngle second)
        {
            Boolean firstIsZero = first.IsZero;
            Boolean secondIsZero = second.IsZero;
            if (firstIsZero == true && secondIsZero == true)
            {
                return 0;
            }
            else if (firstIsZero == true && secondIsZero == false)
            {
                return -1;
            }
            else if (firstIsZero == false && secondIsZero == true)
            {
                return 1;
            }
            else if (first._measurementUnit == second._measurementUnit)
            {
                return first._value.CompareTo(second._value);
            }
            else
            {
                return first._value.CompareTo(second.GetValueInUnit(first._measurementUnit));
            }
        }

        /// <summary>
        /// Возвращает хэш-код данного угла вращения
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hashCode = this._value.GetHashCode();
                hashCode = (hashCode * 397) ^ this._measurementUnit.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Определяет равенство данного экземпляра угла вращения с указанным, предварительно пытаясь привести его к данному типу
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object other)
        {
            if (Object.ReferenceEquals(null, other)) { return false; }
            if (other.GetType() != typeof(RotationAngle)) { return false; }
            return RotationAngle.AreEqual(this, (RotationAngle)other);
        }

        /// <summary>
        /// Определяет равенство данного экземпляра угла вращения с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(RotationAngle other)
        {
            return RotationAngle.AreEqual(this, other);
        }

        /// <summary>
        /// Сравнивает данный угол вращения с указанным и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(RotationAngle other)
        {
            return RotationAngle.Compare(this, other);
        }

        /// <summary>
        /// Пытается привести указанный экземпляр неизвестного типа Object к данному типу, а затем 
        /// сравнивает данный угол вращения с указанным и определяет, какой из них больше, меньше, или же они равны.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Int32 CompareTo(Object other)
        {
            if (Object.ReferenceEquals(other, null) == true)
            {
                throw new ArgumentNullException("other", "Нельзя сравнить угол с NULL");
            }
            if (other.GetType() != this.GetType())
            {
                throw new InvalidOperationException("Нельзя сравнить угол с другим типом");
            }
            return RotationAngle.Compare(this, (RotationAngle)other);
        }
        #endregion Comparable and Equatable

        #region Operators

        /// <summary>
        /// Определяет, равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator ==(RotationAngle first, RotationAngle second)
        {
            return RotationAngle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, не равны ли два указанных экземпляра углов между собой
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator !=(RotationAngle first, RotationAngle second)
        {
            return !RotationAngle.AreEqual(first, second);
        }

        /// <summary>
        /// Определяет, строго больше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >(RotationAngle first, RotationAngle second)
        {
            return RotationAngle.Compare(first, second) == 1;
        }

        /// <summary>
        /// Определяет, строго меньше ли величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <(RotationAngle first, RotationAngle second)
        {
            return RotationAngle.Compare(first, second) == -1;
        }

        /// <summary>
        /// Определяет, больше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator >=(RotationAngle first, RotationAngle second)
        {
            return RotationAngle.Compare(first, second) >= 0;
        }

        /// <summary>
        /// Определяет, меньше или равна величина первого угла по сравнению с величиной второго
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Boolean operator <=(RotationAngle first, RotationAngle second)
        {
            return RotationAngle.Compare(first, second) <= 0;
        }

        /// <summary>
        /// Возвращает угол вращения, являющийся суммой указанных двух углов. Если указанные углы имеют разные направления вращения, 
        /// направление результирующего угла будет определяться углом с большей угловой величиной. 
        /// Единица измерения результирующего угла будет такой, как у первого угла.
        /// </summary>
        /// <param name="first">Первый слагаемый угол вращения</param>
        /// <param name="second">Второй слагаемый угол вращения</param>
        /// <example>30° + 30° = 60°; 270° + 180° = 450°; (-30)° + (-40)° = (-70)°; (-800)° + 270° = (-530)°</example>
        /// <returns></returns>
        public static RotationAngle operator +(RotationAngle first, RotationAngle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return RotationAngle.FromSpecifiedUnit(first._value + second._value, first._measurementUnit);
            }
            return RotationAngle.FromSpecifiedUnit(first._value + second.GetValueInUnit(first._measurementUnit), first._measurementUnit);
        }

        /// <summary>
        /// Возвращает угол вращения, являющийся разностью уменьшаемого и вычитаемого углов. Если указанные углы имеют разные направления вращения, 
        /// направление результирующего угла будет определяться углом с большей угловой величиной. 
        /// Единица измерения результирующего угла будет такой, как у первого угла. 
        /// Не-ассоциативная и антикоммуникативная операция.
        /// </summary>
        /// <param name="first">Первый уменьшаемый угол</param>
        /// <param name="second">Второй вычитаемый угол</param>
        /// <example>540° - 360° = 180°; 180° - 270° = (-90)°; (-90)° - 270° = (-360)°; 380° - (-340)° = 720°</example>
        /// <returns></returns>
        public static RotationAngle operator -(RotationAngle first, RotationAngle second)
        {
            if (first._measurementUnit == second._measurementUnit)
            {
                return RotationAngle.FromSpecifiedUnit(first._value - second._value, first._measurementUnit);
            }
            return RotationAngle.FromSpecifiedUnit(first._value - second.GetValueInUnit(first._measurementUnit), first._measurementUnit);
        }

        /// <summary>
        /// Неявно преобразовывает указанный геометрический угол в угол вращения с той же величиной и единицей измерения
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static implicit operator RotationAngle(GeometricAngle input)
        {
            if (input.IsZero && input.Unit.IsInitialized == false)
            {
                return RotationAngle.Zero;
            }
            return RotationAngle.FromSpecifiedUnit(input.Value, input.Unit);
        }
        #endregion Operators

        #region Cloning
        /// <summary>
        /// Возвращает глубокую копию текущего экземпляра угла
        /// </summary>
        /// <returns></returns>
        public RotationAngle Clone()
        {
            return new RotationAngle(this);
        }
        Object ICloneable.Clone()
        {
            return this.Clone();
        }
        #endregion Cloning

        #region ToString
        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return String.Format("{0} {1}", this._value.ToString(System.Globalization.CultureInfo.InvariantCulture), this._measurementUnit.ToString());
        }

        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла 
        /// с использованием указанного формата и сведений об особенностях форматирования 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public String ToString(String format, IFormatProvider formatProvider)
        {
            return String.Format("{0} {1}", this._value.ToString(format, formatProvider), this._measurementUnit.ToString());
        }

        /// <summary>
        /// Возвращает строковое представление величины и единицы измерения данного угла, совместимое со спецификацией CSS. 
        /// Выбрасывает исключение, если единица измерения данного угла не поддерживается. 
        /// </summary>
        /// <remarks>https://developer.mozilla.org/en-US/docs/Web/CSS/angle</remarks>
        /// <returns></returns>
        ///<exception cref="NotSupportedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public string ToCssString()
        {
            return String.Concat(this._value.ToString(System.Globalization.CultureInfo.InvariantCulture), this._measurementUnit.CssName);
        }
        #endregion ToString
    }
}