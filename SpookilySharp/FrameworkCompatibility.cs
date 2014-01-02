// FrameworkCompatibility.cs
//
// Author:
//     Jon Hanna <jon@hackcraft.net>
//
// © 2014 Jon Hanna
//
// Licensed under the EUPL, Version 1.1 only (the “Licence”).
// You may not use, modify or distribute this work except in compliance with the Licence.
// You may obtain a copy of the Licence at:
// <http://joinup.ec.europa.eu/software/page/eupl/licence-eupl>
// A copy is also distributed with this source code.
// Unless required by applicable law or agreed to in writing, software distributed under the
// Licence is distributed on an “AS IS” basis, without warranties or conditions of any kind.

using System;
using System.Runtime.CompilerServices;

namespace SpookyHash
{
    internal static class FrameworkCompatibility
    {
#if NET_4_5
        public const MethodImplOptions AggressiveInlining = MethodImplOptions.AggressiveInlining;
#else
        public const MethodImplOptions AggressiveInlining = (MethodImplOptions)0;
#endif
    }
}