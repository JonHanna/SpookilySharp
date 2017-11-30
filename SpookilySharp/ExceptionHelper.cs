// ExceptionHelper.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014 Jon Hanna
//
// This source code is licensed under the EUPL, Version 1.1 only (the “Licence”).
// You may not use, modify or distribute this work except in compliance with the Licence.
// You may obtain a copy of the Licence at:
// <http://joinup.ec.europa.eu/software/page/eupl/licence-eupl>
// A copy is also distributed with this source code.
// Unless required by applicable law or agreed to in writing, software distributed under the
// Licence is distributed on an “AS IS” basis, without warranties or conditions of any kind.

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

        public static void BadHashCode128Format() =>
            throw new FormatException("The string did not contain a 32-digit hexadecimal number.");

        private static void StartIndexOutOfRange() => throw new ArgumentOutOfRangeException("startIndex");

        private static void NegativeLength() => throw new ArgumentOutOfRangeException("length");

        private static void PastArrayBounds() => throw new ArgumentException("Attempt to read beyond the end of the array.");

        private static void PastStringBounds() => throw new ArgumentException("Attempt to read beyond the end of the string.");

        private static void CheckNotNegativeLength(int length)
        {
            if (length < 0)
            {
                NegativeLength();
            }
        }

        private static void CheckIndexInRange(int startIndex, int length)
        {
            if ((uint)startIndex >= (uint)length)
            {
                StartIndexOutOfRange();
            }
        }

        public static void CheckArray<T>(T[] message, int startIndex, int length)
        {
            CheckNotNegativeLength(length);
            int len = message.Length;
            CheckIndexInRange(startIndex, len);
            if (startIndex + length > len)
            {
                PastArrayBounds();
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
                PastStringBounds();
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
