# Spookily Sharp [<img src="https://ci.appveyor.com/api/projects/status/y00kmlox0in8o8kj/branch/master?svg=true&passingText=passing%20%F0%9F%8E%89&pendingText=pending%20%E2%8F%B3&failingText=failing%20%F0%9F%94%A5" align="right">](https://ci.appveyor.com/project/JonHanna/spookilysharp/branch/master)

A .NET/Mono implementation of Bob Jenkins’ [SpookyHash version 2](http://burtleburtle.net/bob/hash/spooky.html). Offers 32- 64- and 128-bit hashes of strings, char and byte arrays, streams and any type of object represented by an array of simple types.  

Development is only active for .NET Standard 2.0 (.NET Core 2.0, .NET Framework 4.6.1, Mono 5.4, etc.). Packages for previous frameworks down to .NET Framework 2.0 are available but will only be updated if serious issues are discovered.

# License

> Licensed under the MIT license. See the LICENSE file in the repository root for more details.

# NuGet Package

Spookily Sharp is available as a [NuGet package](https://www.nuget.org/packages/SpookilySharp)

Run `Install-Package SpookilySharp` in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) or search for “SpookilySharp” in your IDE’s package management plug-in.

# Usage

The `SpookyHash` class allows for both incremental and one-shot use.  

To build up a hash incrementally, create an instance of the class, and call `Update()` with strings, arrays of bytes or chars, or a pointer to arbitrary data, then call `Final()` to obtain the hash. You may continue to `Update()` further after the call to `Final()`.  

To obtain a hash in a single operation, use the appropriate static method of `SpookyHash` or the extension methods provided by `SpookyHasher`.  

The overrides which most directly mirror the original C++ implementation use pointers and unsigned types, which are not CLS-compliant and will not be available to all .NET languages. Other CLS-compliant overrides are also available.

Extension methods offer quick access to seeded and default-seeded hashes of all three sizes on strings and arrays of bytes and characters.

Rehash operations redistribute the bits of 32- and 64-bit integer types. This can be useful in improving the distribution of existing hash methods. While this cannot improve the overall risk of collision (indeed, it makes it slightly worse), it improves the risk of collision in the lower-bits, which is the real danger with most uses of hash codes. (Indeed, improving such cases was the first reason for porting SpookyHash to C♯, but having done that, finishing the job seemed fruitful). Extension methods exist to provide wrappers on `IEquatable<T>`, with an attribute available to mark implementations of `IEquatable<T>.GetHashCode(T)` or overrides of `object.GetHashCode()` that are of such quality (e.g. if already from SpookyHash or a similarly high-quality hash method) as to not benefit from such wrappers. (Hint: If your `GetHashCode` method was written in less than half an hour, or is less than a page of code, it should **not** have this attribute applied. Even the relatively good `string.GetHashCode()` benefits from such wrappers when used with power-of-two sized tables).

# Performance

SpookyHash is optimised for fast processing on 64-bit systems, but does operate on 32-bit systems and 32-bit builds on 64-bit systems. It is instructive that one of the test cases compares with the Microsoft implementation of `string.GetHashCode()` as follows:

1. The fastest is SpookyHash on a 64-bit build.
2. Next fastest is `string.GetHashCode()` on a 32-bit build.
3. Next fastest is `string.GetHashCode()` on a 64-bit build.
4. Slowest is SpookyHash on a 32-bit build. (Though still fast enough for many uses).

This implementation has a lot of hand-inlining, wherever profiling showed it to give an improvement over depending upon the JITter to decide when to inline.

# Quality

SpookyHash has very good “avalance” tendencies as examined in the link above, [here](http://blog.aggregateknowledge.com/2012/02/02/choosing-a-good-hash-function-part-3/) and elsewhere. Its quality is such that even on a 32-bit build the overall performance of some hash-based structures (particularly open-addressing with power-two sizes, being particularly vulnerable to lower-bit collisions) will be better than with most .NET implementations of `GetHashCode()`.

The TL/DR version of the above links: If you give SpookyHash two different inputs that differ only in one bit, each bit in the outputs has a roughly 50% chance of being different between the two.

# Security Considerations

SpookyHash’s 128-bit form is of comparable size to some older cryptographic hashes, such as MD5. Aside from such small cryptographic hashes being deprecated for most uses, SpookyHash is **not** a cryptographic hash, and should not be used as such.

That said, SpookyHash is likely to be reasonably resilient in the face of hash DoS attacks, as its strong distribution makes the task of producing multiple inputs with the same hash codes more difficult. However, if you will be hashing items from untrusted input, such as web requests, you should use the seeded forms of the hash methods, and the seeded constructors to the equality comparers. This will make the output unpredictable without knowledge of the seed. (Uptime serves as a reasonably good seed value).

# Platform Independence

If run on a big-endian system, the code would produce different hashes, but of equal quality.
