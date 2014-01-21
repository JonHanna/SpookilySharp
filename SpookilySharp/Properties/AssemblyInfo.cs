// AssemblyInfo.cs
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("Spookily Sharp")]
[assembly: AssemblyDescription(".NET/Mono implementation of SpookyHash")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyProduct("Spookily Sharp")]
[assembly: AssemblyCopyright("© 2014 Jon Hanna")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
#if NET_20
[assembly: AssemblyVersion("1.1.5126.40902")]
#elif NET_30
[assembly: AssemblyVersion("1.1.5126.40903")]
#elif NET_35
[assembly: AssemblyVersion("1.1.5126.40907")]
#elif NET_40
[assembly: AssemblyVersion("1.1.5126.40904")]
#elif NET_45
[assembly: AssemblyVersion("1.1.5126.40900")]
#elif NET_451
[assembly: AssemblyVersion("1.1.5126.40896")]
#endif
[assembly: AssemblyInformationalVersionAttribute("1.1.5127")]
[assembly: CLSCompliant(true)]
[assembly: AllowPartiallyTrustedCallers]
[assembly: ComVisible(false)]