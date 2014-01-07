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
using System.Runtime.Remoting.Messaging;

namespace SpookilySharp
{
    internal static class ExceptionHelper
    {
        public static void CheckNotNull<T>(T arg, string name) where T : class
        {
            if(arg == null)
                throw new ArgumentNullException(name);
        }

        public static void CheckNotNullS(string arg)
        {
            CheckNotNull(arg, "s");
        }

        public static void BadHashCode128Format()
        {
            throw new FormatException("The string did not contain a 16-digit hexadecimal number.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
            "CA2208:InstantiateArgumentExceptionsCorrectly",
            Justification = "Helper method called by methods with such a parameter")]
        private static void StartIndexOutOfRange()
        {
            // Analysis disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("startIndex");
        }
        
        private static void PastArrayBounds()
        {
            throw new ArgumentException("Attempt to read beyond the end of the array.");
        }
        
        private static void PastStringBounds()
        {
            throw new ArgumentException("Attempt to read beyond the end of the string.");
        }
        
        public static void CheckArray<T>(T[] message, int startIndex, int length)
        {
            if(startIndex < 0 || startIndex > message.Length)
                StartIndexOutOfRange();
            if(startIndex + length > message.Length)
                PastArrayBounds();
        }
        
        public static void CheckArrayIncNull<T>(T[] message, int startIndex, int length)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            CheckArray(message, startIndex, length);
        }
        public static void CheckString(string message, int startIndex, int length)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            if(startIndex < 0 || startIndex > message.Length)
                StartIndexOutOfRange();
            if(startIndex + length > message.Length)
                PastStringBounds();
        }
        public static void CheckMessageNotNull<T>(T message) where T : class
        {
            if(message == null)
                throw new ArgumentNullException("message");
        }
    }
}