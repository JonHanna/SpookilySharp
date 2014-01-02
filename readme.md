# Spookily Sharp

A .NET/Mono implementation of Bob Jenkins’ [SpookyHash version 2](http://burtleburtle.net/bob/hash/spooky.html). Offers 32- 64- and 128-bit hashes of strings, char and byte arrays, streams and any type of object represented by an array of simple types.  

# Usage

The `SpookyHash` class allows for both incremental and one-shot use.  

To build up a hash incrementally, create an instance of the class, and call `Update()` with strings, arrays of bytes or chars, or a pointer to arbitary data, then call `Final()` to obtain the hash. You may continue to `Update()` further after the call to `Final()`.  

To obtain a hash in a single operation, use the appropriate static method of `SpookyHash` or the extension methods provided by `SpookyHasher`.  

The overrides which most directly mirror the original C++ implemenation use pointers and unsigned types, which are not CLS-compliant and will not be available to all .NET languages. Other CLS-compliant overrides are also available.

# Performance

SpookyHash is optimised for fast processing on 64-bit systems, but does operate on 32-bit systems and 32-bit builds on 64-bit systems. It is instructive that one of the test cases compares with the Microsoft implementation of `string.GetHashCode()` as follows:

1. The fastest is SpookyHash on a 64-bit build.
2. Next fastest is `string.GetHashCode()` on a 32-bit build.
3. Next fastest is `string.GetHashCode()` on a 64-bit build.
4. Slowest is SpookyHash on a 32-bit build. (Though still fast enough for many uses).

# Quality

SpookyHash has very good “avalance” tendencies as examined in the link above, [here](http://blog.aggregateknowledge.com/2012/02/02/choosing-a-good-hash-function-part-3/) and elsewhere. It’s quality is such that even on a 32-bit build the overall performance of some hash-based structures (particulary open-addressing with power-two sizes, being particularly vulnerable to lower-bit collisions) will be better than with most .NET implementations of `GetHashCode()`.
