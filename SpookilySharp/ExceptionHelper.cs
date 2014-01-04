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
    }
}

