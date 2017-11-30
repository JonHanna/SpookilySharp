// ExceptionHelper.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014–2017 Jon Hanna
//
// Licensed under the MIT license. See the LICENSE file in the repository root for more details.

using System;
using System.IO;

namespace SpookilySharp
{
    internal static class ExceptionHelper
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        public static void CheckNotNull(object arg, string name)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void CheckNotNullString(string arg) => CheckNotNull(arg, "s");

        public static Exception BadHashCode128Format() =>
            new FormatException("The string did not contain a 32-digit hexadecimal number.");

        private static Exception StartIndexOutOfRange() => new ArgumentOutOfRangeException("startIndex");

        private static Exception NegativeLength() => new ArgumentOutOfRangeException("length");

        private static Exception PastArrayBounds() => new ArgumentException("Attempt to read beyond the end of the array.");

        private static Exception PastStringBounds() => new ArgumentException("Attempt to read beyond the end of the string.");

        private static void CheckNotNegativeLength(int length)
        {
            if (length < 0)
            {
                throw NegativeLength();
            }
        }

        private static void CheckIndexInRange(int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)length)
            {
                throw StartIndexOutOfRange();
            }
        }

        public static void CheckArray<T>(T[] message, int startIndex, int length)
        {
            CheckNotNegativeLength(length);
            int len = message.Length;
            CheckIndexInRange(startIndex, len);
            if (startIndex + length > len)
            {
                throw PastArrayBounds();
            }
        }

        public static void CheckArrayIncNull<T>(T[] message, int startIndex, int length)
        {
            CheckMessageNotNull(message);
            CheckArray(message, startIndex, length);
        }

        public static void CheckBounds(string message, int startIndex, int length)
        {
            CheckNotNegativeLength(length);
            int len = message.Length;
            CheckIndexInRange(startIndex, len);
            if (startIndex + length > len)
            {
                throw PastStringBounds();
            }
        }

        public static void CheckString(string message, int startIndex, int length)
        {
            CheckMessageNotNull(message);
            CheckBounds(message, startIndex, length);
        }

        public static void CheckMessageNotNull(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
        }

        public static void CheckNotNull(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
        }
    }
}
