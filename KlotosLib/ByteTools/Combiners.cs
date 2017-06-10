using System;
using System.ComponentModel;

namespace KlotosLib.ByteTools
{
    /// <summary>
    /// Contains pure methods, that combine complex numbers from bytes
    /// </summary>
    public static class Combiners
    {
        #region Basic methods
        //2-byte
        
        /// <summary>
        /// Combines two bytes into Int16 using little endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <returns></returns>
        public static Int16 GetInt16LE(Byte first, Byte second)
        {
            return (Int16) (first | second << 8);
        }

        /// <summary>
        /// Combines two bytes into Int16 using big endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <returns></returns>
        public static Int16 GetInt16BE(Byte first, Byte second)
        {
            return (Int16)(first << 8 | second);
        }

        /// <summary>
        /// Combines two bytes into UInt16 using little endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <returns></returns>
        public static UInt16 GetUInt16LE(Byte first, Byte second)
        {
            return unchecked((UInt16)(first | second << 8));
        }

        /// <summary>
        /// Combines two bytes into UInt16 using big endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <returns></returns>
        public static UInt16 GetUInt16BE(Byte first, Byte second)
        {
            return unchecked((UInt16)(first << 8 | second));
        }

        //4-byte

        /// <summary>
        /// Combines four bytes into Int32 using little endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <param name="third">Third byte</param>
        /// <param name="fourth">Fourth byte</param>
        /// <returns></returns>
        public static Int32 GetInt32LE(Byte first, Byte second, Byte third, Byte fourth)
        {
            return (Int32)(first | second << 8 | third << 16 | fourth << 24);
        }

        /// <summary>
        /// Combines four bytes into Int32 using big endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <param name="third">Third byte</param>
        /// <param name="fourth">Fourth byte</param>
        /// <returns></returns>
        public static Int32 GetInt32BE(Byte first, Byte second, Byte third, Byte fourth)
        {
            return (Int32)(first << 24 | second << 16 | third << 8 | fourth);
        }

        /// <summary>
        /// Combines four bytes into UInt32 using little endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <param name="third">Third byte</param>
        /// <param name="fourth">Fourth byte</param>
        /// <returns></returns>
        public static UInt32 GetUInt32LE(Byte first, Byte second, Byte third, Byte fourth)
        {
            return unchecked((UInt32)(first | second << 8 | third << 16 | fourth << 24));
        }
        
        /// <summary>
        /// Combines four bytes into UInt32 using big endianess
        /// </summary>
        /// <param name="first">First byte</param>
        /// <param name="second">Second byte</param>
        /// <param name="third">Third byte</param>
        /// <param name="fourth">Fourth byte</param>
        /// <returns></returns>
        public static UInt32 GetUInt32BE(Byte first, Byte second, Byte third, Byte fourth)
        {
            return unchecked((UInt32)(first << 24 | second << 16 | third << 8 | fourth));
        }

        //8-byte

        /// <summary>
        /// Combines eight bytes into Int64 using little endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static Int64 GetInt64LE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            Int32 low = GetInt32LE(first, second, third, fourth);
            Int32 high = GetInt32LE(fifth, sixth, seventh, eighth);
            return (UInt32)low | ((Int64)high << 32);
        }

        /// <summary>
        /// Combines eight bytes into Int64 using big endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static Int64 GetInt64BE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            Int32 low = GetInt32BE(first, second, third, fourth);
            Int32 high = GetInt32BE(fifth, sixth, seventh, eighth);
            return (UInt32)high | ((Int64)low << 32);
        }

        /// <summary>
        /// Combines eight bytes into UInt64 using little endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static UInt64 GetUInt64LE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            return unchecked((UInt64)GetInt64LE(first, second, third, fourth, fifth, sixth, seventh, eighth));
        }

        /// <summary>
        /// Combines eight bytes into UInt64 using big endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static UInt64 GetUInt64BE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            return unchecked((UInt64)GetInt64BE(first, second, third, fourth, fifth, sixth, seventh, eighth));
        }

        //floats

        private static readonly Byte[] _buffer = new Byte[8];

        /// <summary>
        /// Combines four bytes into Single using little endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <returns></returns>
        public static Single ReadSingleLE(Byte first, Byte second, Byte third, Byte fourth)
        {
            _buffer[0] = first;
            _buffer[1] = second;
            _buffer[2] = third;
            _buffer[3] = fourth;
            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Combines four bytes into Single using big endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <returns></returns>
        public static Single ReadSingleBE(Byte first, Byte second, Byte third, Byte fourth)
        {
            _buffer[0] = fourth;
            _buffer[1] = third;
            _buffer[2] = second;
            _buffer[3] = first;
            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Combines eight bytes into Single using little endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static Double ReadDoubleLE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            _buffer[0] = first;
            _buffer[1] = second;
            _buffer[2] = third;
            _buffer[3] = fourth;
            _buffer[4] = fifth;
            _buffer[5] = sixth;
            _buffer[6] = seventh;
            _buffer[7] = eighth;
            return BitConverter.ToDouble(_buffer, 0);
        }

        /// <summary>
        /// Combines eight bytes into Single using big endianess
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="third"></param>
        /// <param name="fourth"></param>
        /// <param name="fifth"></param>
        /// <param name="sixth"></param>
        /// <param name="seventh"></param>
        /// <param name="eighth"></param>
        /// <returns></returns>
        public static Double ReadDoubleBE(Byte first, Byte second, Byte third, Byte fourth, Byte fifth, Byte sixth, Byte seventh, Byte eighth)
        {
            _buffer[0] = eighth;
            _buffer[1] = seventh;
            _buffer[2] = sixth;
            _buffer[3] = fifth;
            _buffer[4] = fourth;
            _buffer[5] = third;
            _buffer[6] = second;
            _buffer[7] = first;
            return BitConverter.ToDouble(_buffer, 0);
        }

        #endregion Basic methods

        #region Array wrappers

        private const string _NotEnoughBytesTemplate = "Specified offset and array allow to obtain lesser then {0} bytes, which are required for this method";
        private const byte _2b = 2;
        private const byte _4b = 4;
        private const byte _8b = 8;

        //2-byte

        /// <summary>
        /// Returns Int16 by reading two bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int16 ReadInt16LE(Byte[] input, Int32 offset)
        {
            if(input == null) {throw new ArgumentNullException("input");}
            if(offset < 0) {throw new ArgumentOutOfRangeException("offset");}
            if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            return GetInt16LE(first, second);
        }

        /// <summary>
        /// Returns Int16 by reading two bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int16 ReadInt16BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            return GetInt16BE(first, second);
        }

        /// <summary>
        /// Returns UInt16 by reading two bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt16 ReadUInt16LE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            return GetUInt16LE(first, second);
        }

        /// <summary>
        /// Returns UInt16 by reading two bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt16 ReadUInt16BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            return GetUInt16BE(first, second);
        }

        //4-byte

        /// <summary>
        /// Returns Int32 by reading four bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int32 ReadInt32LE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            return GetInt32LE(first, second, third, fourth);
        }

        /// <summary>
        /// Returns Int32 by reading four bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int32 ReadInt32BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            return GetInt32BE(first, second, third, fourth);
        }

        /// <summary>
        /// Returns UInt32 by reading four bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt32 ReadUInt32LE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            return GetUInt32LE(first, second, third, fourth);
        }

        /// <summary>
        /// Returns UInt32 by reading four bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt32 ReadUInt32BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            return GetUInt32BE(first, second, third, fourth);
        }

        //8-byte

        /// <summary>
        /// Returns Int64 by reading eight bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int64 ReadInt64LE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            Byte fifth = input[offset + 4];
            Byte sixth = input[offset + 5];
            Byte seventh = input[offset + 6];
            Byte eighth = input[offset + 7];
            return GetInt64LE(first, second, third, fourth, fifth, sixth, seventh, eighth);
        }

        /// <summary>
        /// Returns Int64 by reading eight bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int64 ReadInt64BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            Byte fifth = input[offset + 4];
            Byte sixth = input[offset + 5];
            Byte seventh = input[offset + 6];
            Byte eighth = input[offset + 7];
            return GetInt64BE(first, second, third, fourth, fifth, sixth, seventh, eighth);
        }

        /// <summary>
        /// Returns UInt64 by reading eight bytes from specified array, starting from specified position (offset), and using little endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt64 ReadUInt64LE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            Byte fifth = input[offset + 4];
            Byte sixth = input[offset + 5];
            Byte seventh = input[offset + 6];
            Byte eighth = input[offset + 7];
            return GetUInt64LE(first, second, third, fourth, fifth, sixth, seventh, eighth);
        }

        /// <summary>
        /// Returns UInt64 by reading eight bytes from specified array, starting from specified position (offset), and using big endianess
        /// </summary>
        /// <param name="input"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt64 ReadUInt64BE(Byte[] input, Int32 offset)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
            Byte first = input[offset];
            Byte second = input[offset + 1];
            Byte third = input[offset + 2];
            Byte fourth = input[offset + 3];
            Byte fifth = input[offset + 4];
            Byte sixth = input[offset + 5];
            Byte seventh = input[offset + 6];
            Byte eighth = input[offset + 7];
            return GetUInt64BE(first, second, third, fourth, fifth, sixth, seventh, eighth);
        }

        #endregion Array wrappers

        #region Generic wrapper

        /// <summary>
        /// Represents endianess (order of the bytes that are combined into multi-byte structure)
        /// </summary>
        public enum Endianess : byte
        {
            /// <summary>
            /// Little endian representation (from lower to higher)
            /// </summary>
            LittleEndian = 0,

            /// <summary>
            /// Big endian representation (from higher to lower)
            /// </summary>
            BigEndian = 1
        }

        private const String _UnknownEndianessTemplate = "Specified endianess value '{0}' is not supported";

        /// <summary>
        /// Returns a number of specified type, which is extracted from specified array and offset, and using specified endianess
        /// </summary>
        /// <typeparam name="T">Type of the output number, which should be returned. Only 2-, 4-, and 8-bytes signed and unsigned integers are supported.</typeparam>
        /// <param name="input">Input byte array. Cannot be NULL.</param>
        /// <param name="offset">0-based position in the specified array, starting from which the bytes should be taken</param>
        /// <param name="endianess">Endianess of bytes in the specified array</param>
        /// <returns></returns>
        public static T Read<T>(Byte[] input, Int32 offset, Endianess endianess)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (offset < 0) { throw new ArgumentOutOfRangeException("offset"); }
            if (Enum.IsDefined(typeof (Endianess), endianess) == false)
            {
                throw new InvalidEnumArgumentException("endianess", (Int32)endianess, endianess.GetType());
            }
            Type inputType = typeof (T);
            if (inputType == typeof (Int16))
            {
                if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetInt16LE(input[offset], input[offset + 1]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetInt16BE(input[offset], input[offset + 1]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            if (inputType == typeof(UInt16))
            {
                if (input.Length - offset < _2b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _2b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetUInt16LE(input[offset], input[offset + 1]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetUInt16BE(input[offset], input[offset + 1]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            if (inputType == typeof (Int32))
            {
                if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetInt32LE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetInt32BE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            if (inputType == typeof(UInt32))
            {
                if (input.Length - offset < _4b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _4b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetUInt32LE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetUInt32BE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            if (inputType == typeof(Int64))
            {
                if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetInt64LE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3],
                        input[offset + 4], input[offset + 5], input[offset + 6], input[offset + 7]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetInt64BE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3],
                        input[offset + 4], input[offset + 5], input[offset + 6], input[offset + 7]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            if (inputType == typeof(UInt64))
            {
                if (input.Length - offset < _8b) { throw new ArgumentException(String.Format(_NotEnoughBytesTemplate, _8b)); }
                switch (endianess)
                {
                    case Endianess.LittleEndian:
                        return (T)(Object)GetUInt64LE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3],
                        input[offset + 4], input[offset + 5], input[offset + 6], input[offset + 7]);
                    case Endianess.BigEndian:
                        return (T)(Object)GetUInt64BE(input[offset], input[offset + 1], input[offset + 2], input[offset + 3],
                        input[offset + 4], input[offset + 5], input[offset + 6], input[offset + 7]);
                    default:
                        throw new ArgumentException(String.Format(_UnknownEndianessTemplate, endianess));
                }
            }
            throw new ArgumentException("Specified type '" + inputType.FullName + "' is not supported");
        }
        #endregion Generic wrapper
    }
}