// ExcludeFromCodeCoverageAttribute.cs
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

#if NET_35
using System;

namespace SpookilySharp
{
    // Act as local replacement for System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute when compiling with
    // the .NET 3.5 framework.
    [AttributeUsageAttribute(
        AttributeTargets.Class |
        AttributeTargets.Struct |
        AttributeTargets.Constructor |
        AttributeTargets.Method |
        AttributeTargets.Property |
        AttributeTargets.Event,
        Inherited = false,
        AllowMultiple = false)]
    internal sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }
}
#endif