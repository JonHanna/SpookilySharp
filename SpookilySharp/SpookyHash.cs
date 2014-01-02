// SpookyHash.cs
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

// Based on Bob Jenkins’ SpookyHash version 2. <http://burtleburtle.net/bob/hash/spooky.html>
// Described by Jenkins as “public domain” which may or may not be legally possible for the
// work of a living person in your jurisdiction. If not, it may be reasonably inferred that
// permission is given by him to port the algorithm into other languages, as per here.

//FIXME: Is this safe? Can we tell when it is? It gives a good performance boost, if it is...
#define ALLOW_UNALIGNED_READ

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpookilySharp
{
    public class SpookyHash
    {

        private const int NumVars = 12;
        private const long BlockSize = NumVars * 8;
        private const long BufSize = 2 * BlockSize;
        internal const ulong SpookyConst = 0xDEADBEEFDEADBEEF;
        
        [CLSCompliant(false)]
        public static unsafe void Hash128(void* message, long length, ref ulong hash1, ref ulong hash2)
        {
            if (length < BufSize)
            {
                Short(message, length, ref hash1, ref hash2);
                return;
            }
            ulong h0,h1,h2,h3,h4,h5,h6,h7,h8,h9,h10,h11;

            h0=h3=h6=h9  = hash1;
            h1=h4=h7=h10 = hash2;
            h2=h5=h8=h11 = SpookyConst;

            var p64 = (ulong*)message;

            ulong* end = p64 + (length / BlockSize) * NumVars;
            ulong* buf = stackalloc ulong[NumVars];
#if !ALLOW_UNALIGNED_READ
            if((((long)message) & 7) == 0)
                while (p64 < end)
                { 
                    h0 += p64[0];    h2 ^= h10;   h11 ^= h0;   h0 =   h0 << 11 | h0 >> -11;   h11 += h1;
                    h1 += p64[1];    h3 ^= h11;   h0 ^= h1;    h1 =   h1 << 32 | h1 >> 32;    h0 += h2;
                    h2 += p64[2];    h4 ^= h0;    h1 ^= h2;    h2 =   h2 << 43 | h2 >> -43;   h1 += h3;
                    h3 += p64[3];    h5 ^= h1;    h2 ^= h3;    h3 =   h3 << 31 | h3 >> -31;   h2 += h4;
                    h4 += p64[4];    h6 ^= h2;    h3 ^= h4;    h4 =   h4 << 17 | h4 >> -17;   h3 += h5;
                    h5 += p64[5];    h7 ^= h3;    h4 ^= h5;    h5 =   h5 << 28 | h5 >> -28;   h4 += h6;
                    h6 += p64[6];    h8 ^= h4;    h5 ^= h6;    h6 =   h6 << 39 | h6 >> -39;   h5 += h7;
                    h7 += p64[7];    h9 ^= h5;    h6 ^= h7;    h7 =   h7 << 57 | h7 >> -57;   h6 += h8;
                    h8 += p64[8];    h10 ^= h6;   h7 ^= h8;    h8 =    h8 <<55 | h8 >> -55;   h7 += h9;
                    h9 += p64[9];    h11 ^= h7;   h8 ^= h9;    h9 =   h9 << 54 | h9 >> -54;   h8 += h10;
                    h10 += p64[10];  h0 ^= h8;    h9 ^= h10;   h10 = h10 << 22 | h10 >> -22;  h9 += h11;
                    h11 += p64[11];  h1 ^= h9;    h10 ^= h11;  h11 = h11 << 46 | h11 >> -46;  h10 += h0;
                    p64 += NumVars;
                }
            else
#endif
                while (p64 < end)
                {
                    MemCpy(buf, p64, (int)BlockSize);
                    
                    h0 += buf[0];    h2 ^= h10;   h11 ^= h0;   h0 =   h0 << 11 | h0 >> -11;   h11 += h1;
                    h1 += buf[1];    h3 ^= h11;   h0 ^= h1;    h1 =   h1 << 32 | h1 >> 32;    h0 += h2;
                    h2 += buf[2];    h4 ^= h0;    h1 ^= h2;    h2 =   h2 << 43 | h2 >> -43;   h1 += h3;
                    h3 += buf[3];    h5 ^= h1;    h2 ^= h3;    h3 =   h3 << 31 | h3 >> -31;   h2 += h4;
                    h4 += buf[4];    h6 ^= h2;    h3 ^= h4;    h4 =   h4 << 17 | h4 >> -17;   h3 += h5;
                    h5 += buf[5];    h7 ^= h3;    h4 ^= h5;    h5 =   h5 << 28 | h5 >> -28;   h4 += h6;
                    h6 += buf[6];    h8 ^= h4;    h5 ^= h6;    h6 =   h6 << 39 | h6 >> -39;   h5 += h7;
                    h7 += buf[7];    h9 ^= h5;    h6 ^= h7;    h7 =   h7 << 57 | h7 >> -57;   h6 += h8;
                    h8 += buf[8];    h10 ^= h6;   h7 ^= h8;    h8 =    h8 <<55 | h8 >> -55;   h7 += h9;
                    h9 += buf[9];    h11 ^= h7;   h8 ^= h9;    h9 =   h9 << 54 | h9 >> -54;   h8 += h10;
                    h10 += buf[10];  h0 ^= h8;    h9 ^= h10;   h10 = h10 << 22 | h10 >> -22;  h9 += h11;
                    h11 += buf[11];  h1 ^= h9;    h10 ^= h11;  h11 = h11 << 46 | h11 >> -46;  h10 += h0;
                    p64 += NumVars;
                }
            long remainder = (length - ((byte *)end-(byte *)message));

            MemCpy(buf, end, remainder);
            MemZero(((byte*)buf) + remainder, BlockSize - remainder);
            ((byte *)buf)[BlockSize-1] = (byte)remainder;

            h0 += buf[0];   h1 += buf[1];   h2 += buf[2];   h3 += buf[3];
            h4 += buf[4];   h5 += buf[5];   h6 += buf[6];   h7 += buf[7];
            h8 += buf[8];   h9 += buf[9];   h10 += buf[10]; h11 += buf[11];
            h11+= h1;    h2 ^= h11;   h1 = h1  << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2  << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3  << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4  << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5  << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6  << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7  << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8  << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9  << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0  << 54 | h0 >> -54;
            h11+= h1;    h2 ^= h11;   h1 = h1  << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2  << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3  << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4  << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5  << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6  << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7  << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8  << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9  << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0  << 54 | h0 >> -54;
            h11+= h1;    h2 ^= h11;   h1 = h1  << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2  << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3  << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4  << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5  << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6  << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7  << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8  << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9  << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0  << 54 | h0 >> -54;
            hash1 = h0;
            hash2 = h1;
        }
        //FIXME: Can we improve this, but remain platform-independent?
        private static unsafe void MemCpy(void* dest, void* source, long length)
        {
            int firstNiggle = (int)(length & 7);
            if(firstNiggle != 0)
            {
                var db = (byte*)dest;
                var sb = (byte*)source;
                do
                {
                    *db++ = *sb++;
                    --length;
                }while(--firstNiggle != 0);
                dest = db;
                source = sb;
            }
            if(length > 8)
            {
                var dl = (long*)dest;
                var sl = (long*)source;
                do
                {
                    *dl++ = *sl++;
                }while((length -= 8) > 8);
                dest = dl;
                source = sl;
            }
            if(length != 0)
            {
                var db = (byte*)dest;
                var sb = (byte*)source;
                do
                {
                    *db++ = *sb++;
                }while(--length != 0);
            }
        }
        //FIXME: Can we improve this, but remain platform-independent?
        private static unsafe void MemZero(void* dest, long length)
        {
            int firstNiggle = (int)(length & 7);
            if(firstNiggle != 0)
            {
                var db = (byte*)dest;
                do
                {
                    *db++ = 0;
                    --length;
                }while(--firstNiggle != 0);
                dest = db;
            }
            if(length > 8)
            {
                var dl = (long*)dest;
                do
                {
                    *dl++ = 0;
                }while((length -= 8) > 8);
                dest = dl;
            }
            if(length != 0)
            {
                var db = (byte*)dest;
                do
                {
                    *db++ = 0;
                }while(--length != 0);
            }
        }
        [CLSCompliant(false)]
        public unsafe static ulong Hash64(void* message, long length, ulong seed)
        {
            ulong hash1 = seed;
            Hash128(message, length, ref hash1, ref seed);
            return hash1;
        }
        [CLSCompliant(false)]
        public unsafe static uint Hash32(void* message, long length, uint seed)
        {
            ulong hash1 = seed, hash2 = seed;
            Hash128(message, length, ref hash1, ref hash2);
            return (uint)hash1;
        }
        private unsafe static void Short(void *message, long length, ref ulong hash1, ref ulong hash2)
        {
            ulong* p64;
#if !ALLOW_UNALIGNED_READ
            ulong* buf = stackalloc ulong[2 * NumVars];
            if((((long)message) & 7) != 0)
            {
                MemCpy(buf, message, (int)length);
                p64 = buf;
            }
            else
#endif
                p64 = (ulong*)message;

            long remainder = length & 31;
            ulong a = hash1;
            ulong b = hash2;
            ulong c = SpookyConst;
            ulong d = SpookyConst;

            if (length > 15)
            {
                ulong *end = p64 + (length/32)*4;
                for (; p64 < end; p64 += 4)
                {
                    c += p64[0];
                    d += p64[1];
                    c = c << 50 | c >> -50;  c += d;  a ^= c;
                    d = d << 52 | d >> -52;  d += a;  b ^= d;
                    a = a << 30 | a >> -30;  a += b;  c ^= a;
                    b = b << 41 | b >> -41;  b += c;  d ^= b;
                    c = c << 54 | c >> -54;  c += d;  a ^= c;
                    d = d << 48 | d >> -48;  d += a;  b ^= d;
                    a = a << 38 | a >> -38;  a += b;  c ^= a;
                    b = b << 37 | b >> -37;  b += c;  d ^= b;
                    c = c << 62 | c >> -62;  c += d;  a ^= c;
                    d = d << 34 | d >> -34;  d += a;  b ^= d;
                    a = a << 5  | a >> -5;   a += b;  c ^= a;
                    b = b << 36 | b >> -36;  b += c;  d ^= b;
                    a += p64[2];
                    b += p64[3];
                }
                if (remainder >= 16)
                {
                    c += p64[0];
                    d += p64[1];
                    c = c << 50 | c >> -50;  c += d;  a ^= c;
                    d = d << 52 | d >> -52;  d += a;  b ^= d;
                    a = a << 30 | a >> -30;  a += b;  c ^= a;
                    b = b << 41 | b >> -41;  b += c;  d ^= b;
                    c = c << 54 | c >> -54;  c += d;  a ^= c;
                    d = d << 48 | d >> -48;  d += a;  b ^= d;
                    a = a << 38 | a >> -38;  a += b;  c ^= a;
                    b = b << 37 | b >> -37;  b += c;  d ^= b;
                    c = c << 62 | c >> -62;  c += d;  a ^= c;
                    d = d << 34 | d >> -34;  d += a;  b ^= d;
                    a = a << 5  | a >> -5;   a += b;  c ^= a;
                    b = b << 36 | b >> -36;  b += c;  d ^= b;
                    p64 += 2;
                    remainder -= 16;
                }
            }
            d += ((ulong)length) << 56;
            switch (remainder)
            {
                case 15:
                    d += ((ulong)((byte*)p64)[14]) << 48;
                    goto case 14;
                case 14:
                    d += ((ulong)((byte*)p64)[13]) << 40;
                    goto case 13;
                case 13:
                    d += ((ulong)((byte*)p64)[12]) << 32;
                    goto case 12;
                case 12:
                    d += ((uint*)p64)[2];
                    c += p64[0];
                    break;
                case 11:
                    d += ((ulong)((byte*)p64)[10]) << 16;
                    goto case 10;
                case 10:
                    d += ((ulong)((byte*)p64)[9]) << 8;
                    goto case 9;
                case 9:
                    d += (ulong)((byte*)p64)[8];
                    goto case 8;
                case 8:
                    c += p64[0];
                    break;
                case 7:
                    c += ((ulong)((byte*)p64)[6]) << 48;
                    goto case 6;
                case 6:
                    c += ((ulong)((byte*)p64)[5]) << 40;
                    goto case 5;
                case 5:
                    c += ((ulong)((byte*)p64)[4]) << 32;
                    goto case 4;
                case 4:
                    c += ((uint*)p64)[0];
                    break;
                case 3:
                    c += ((ulong)((byte*)p64)[2]) << 16;
                    goto case 2;
                case 2:
                    c += ((ulong)((byte*)p64)[1]) << 8;
                    goto case 1;
                case 1:
                    c += (ulong)((byte*)p64)[0];
                    break;
                case 0:
                    c += SpookyConst;
                    d += SpookyConst;
                    break;
            }
            d ^= c;  c = c << 15 | c >> -15;  d += c;
            a ^= d;  d = d << 52 | d >> -52;  a += d;
            b ^= a;  a = a << 26 | a >> -26;  b += a;
            c ^= b;  b = b << 51 | b >> -51;  c += b;
            d ^= c;  c = c << 28 | c >> -28;  d += c;
            a ^= d;  d = d << 9  | d >> -9;   a += d;
            b ^= a;  a = a << 47 | a >> -47;  b += a;
            c ^= b;  b = b << 54 | b >> -54;  c += b;
            d ^= c;  c = c << 32 | c >> -32;  d += c;
            a ^= d;  d = d << 25 | d >> -25;  a += d;
            b ^= a;  a = a << 63 | a >> -63;  b += a;
            hash1 = a;
            hash2 = b;
        }
        private readonly ulong[] _data = new ulong[2 * NumVars];
        private ulong _state0, _state1, _state2, _state3, _state4, _state5,
            _state6, _state7, _state8, _state9, _state10, _state11;
        private long _length;
        private byte _remainder;

        public SpookyHash(){}
        [CLSCompliant(false)]
        public SpookyHash(ulong seed1, ulong seed2)
        {
            Init(seed1, seed2);
        }
        public SpookyHash(long seed1, long seed2)
        {
            Init(seed1, seed2);
        }
        [CLSCompliant(false)]
        public void Init(ulong seed1, ulong seed2)
        {
            _length = _remainder = 0;
            _state0 = seed1;
            _state1 = seed2;
        }
        public void Init(long seed1, long seed2)
        {
            Init((ulong)seed1, (ulong)seed2);
        }
        public unsafe void Update(byte[] message, long startIndex, long length)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            fixed(byte* ptr = message)
                Update(ptr + startIndex, length);
        }
        public void Update(byte[] message)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            Update(message, 0, message.Length);
        }
        public unsafe void Update(char[] message, long startIndex, long length)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            fixed(char* ptr = message)
                Update(ptr + startIndex, length * 2);
        }
        public void Update(char[] message)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            Update(message, 0, message.Length);
        }
        public unsafe void Update(string message, int startIndex, int length)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            fixed(char* ptr = message + RuntimeHelpers.OffsetToStringData)
                Update(ptr + startIndex, length * 2);
        }
        public void Update(string message)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            Update(message, 0, message.Length);
        }
        [CLSCompliant(false)]
        public unsafe void Update(void* message, long length)
        {
            ulong h0, h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11;
            long newLength = length + _remainder;
            if (newLength < BufSize)
            {
                fixed(ulong* uptr = _data)
                {
                    MemCpy(((byte*)uptr) + _remainder, message, length);
                }
                _length = length + _length;
                _remainder = (byte)newLength;
                return;
            }
            if (_length < BufSize)
            {
                h0=h3=h6=h9  = _state0;
                h1=h4=h7=h10 = _state1;
                h2=h5=h8=h11 = SpookyConst;
            }
            else
            {
                h0 = _state0;
                h1 = _state1;
                h2 = _state2;
                h3 = _state3;
                h4 = _state4;
                h5 = _state5;
                h6 = _state6;
                h7 = _state7;
                h8 = _state8;
                h9 = _state9;
                h10 = _state10;
                h11 = _state11;
            }
            _length = length + _length;

            ulong* p64;
            fixed(ulong* p64Fixed = _data)
            {

                if (_remainder != 0)
                {
                    byte prefix = (byte)(BufSize - _remainder);
                    MemCpy((byte*)p64Fixed + _remainder, message, prefix);

                    h0 += p64Fixed[0];    h2 ^= h10;    h11 ^= h0;    h0 = h0 << 11 | h0 >> -11;    h11 += h1;
                    h1 += p64Fixed[1];    h3 ^= h11;    h0 ^= h1;    h1 = h1 << 32 | h1 >> 32;    h0 += h2;
                    h2 += p64Fixed[2];    h4 ^= h0;    h1 ^= h2;    h2 = h2 << 43 | h2 >> -43;    h1 += h3;
                    h3 += p64Fixed[3];    h5 ^= h1;    h2 ^= h3;    h3 = h3 << 31 | h3 >> -31;    h2 += h4;
                    h4 += p64Fixed[4];    h6 ^= h2;    h3 ^= h4;    h4 = h4 << 17 | h4 >> -17;    h3 += h5;
                    h5 += p64Fixed[5];    h7 ^= h3;    h4 ^= h5;    h5 = h5 << 28 | h5 >> -28;    h4 += h6;
                    h6 += p64Fixed[6];    h8 ^= h4;    h5 ^= h6;    h6 = h6 << 39 | h6 >> -39;    h5 += h7;
                    h7 += p64Fixed[7];    h9 ^= h5;    h6 ^= h7;    h7 = h7 << 57 | h7 >> -57;    h6 += h8;
                    h8 += p64Fixed[8];    h10 ^= h6;    h7 ^= h8;    h8 = h8 << 55 | h8 >> -55;    h7 += h9;
                    h9 += p64Fixed[9];    h11 ^= h7;    h8 ^= h9;    h9 = h9 << 54 | h9 >> -54;    h8 += h10;
                    h10 += p64Fixed[10];    h0 ^= h8;    h9 ^= h10;    h10 = h10 << 22 | h10 >> -22;    h9 += h11;
                    h11 += p64Fixed[11];    h1 ^= h9;    h10 ^= h11;    h11 = h11 << 46 | h11 >> -46;    h10 += h0;
                    p64 = p64Fixed + NumVars;
                    h0 += p64[0];    h2 ^= h10;    h11 ^= h0;    h0 = h0 << 11 | h0 >> -11;    h11 += h1;
                    h1 += p64[1];    h3 ^= h11;    h0 ^= h1;    h1 = h1 << 32 | h1 >> 32;    h0 += h2;
                    h2 += p64[2];    h4 ^= h0;    h1 ^= h2;    h2 = h2 << 43 | h2 >> -43;    h1 += h3;
                    h3 += p64[3];    h5 ^= h1;    h2 ^= h3;    h3 = h3 << 31 | h3 >> -31;    h2 += h4;
                    h4 += p64[4];    h6 ^= h2;    h3 ^= h4;    h4 = h4 << 17 | h4 >> -17;    h3 += h5;
                    h5 += p64[5];    h7 ^= h3;    h4 ^= h5;    h5 = h5 << 28 | h5 >> -28;    h4 += h6;
                    h6 += p64[6];    h8 ^= h4;    h5 ^= h6;    h6 = h6 << 39 | h6 >> -39;    h5 += h7;
                    h7 += p64[7];    h9 ^= h5;    h6 ^= h7;    h7 = h7 << 57 | h7 >> -57;    h6 += h8;
                    h8 += p64[8];    h10 ^= h6;    h7 ^= h8;    h8 = h8 << 55 | h8 >> -55;    h7 += h9;
                    h9 += p64[9];    h11 ^= h7;    h8 ^= h9;    h9 = h9 << 54 | h9 >> -54;    h8 += h10;
                    h10 += p64[10];    h0 ^= h8;    h9 ^= h10;    h10 = h10 << 22 | h10 >> -22;    h9 += h11;
                    h11 += p64[11];    h1 ^= h9;    h10 ^= h11;    h11 = h11 << 46 | h11 >> -46;    h10 += h0;
                    p64 = (ulong*)(((byte*)message) + prefix);
                    length -= prefix;
                }
                else
                    p64 = (ulong*)message;

                ulong* end = p64 + (length / BlockSize) * NumVars;
                byte remainder = (byte)(length - ((byte*)end - ((byte*)p64)));
#if !ALLOW_UNALIGNED_READ
                if((((long)message) & 7) == 0)
                    while (p64 < end)
                    { 
                        h0 += p64[0];    h2 ^= h10;   h11 ^= h0;   h0 =   h0 << 11 | h0 >> -11;   h11 += h1;
                        h1 += p64[1];    h3 ^= h11;   h0 ^= h1;    h1 =   h1 << 32 | h1 >> 32;    h0 += h2;
                        h2 += p64[2];    h4 ^= h0;    h1 ^= h2;    h2 =   h2 << 43 | h2 >> -43;   h1 += h3;
                        h3 += p64[3];    h5 ^= h1;    h2 ^= h3;    h3 =   h3 << 31 | h3 >> -31;   h2 += h4;
                        h4 += p64[4];    h6 ^= h2;    h3 ^= h4;    h4 =   h4 << 17 | h4 >> -17;   h3 += h5;
                        h5 += p64[5];    h7 ^= h3;    h4 ^= h5;    h5 =   h5 << 28 | h5 >> -28;   h4 += h6;
                        h6 += p64[6];    h8 ^= h4;    h5 ^= h6;    h6 =   h6 << 39 | h6 >> -39;   h5 += h7;
                        h7 += p64[7];    h9 ^= h5;    h6 ^= h7;    h7 =   h7 << 57 | h7 >> -57;   h6 += h8;
                        h8 += p64[8];    h10 ^= h6;   h7 ^= h8;    h8 =    h8 <<55 | h8 >> -55;   h7 += h9;
                        h9 += p64[9];    h11 ^= h7;   h8 ^= h9;    h9 =   h9 << 54 | h9 >> -54;   h8 += h10;
                        h10 += p64[10];  h0 ^= h8;    h9 ^= h10;   h10 = h10 << 22 | h10 >> -22;  h9 += h11;
                        h11 += p64[11];  h1 ^= h9;    h10 ^= h11;  h11 = h11 << 46 | h11 >> -46;  h10 += h0;
                        p64 += NumVars;
                    }
                else
#endif
                    fixed(ulong* dataPtr = _data)
                        while (p64 < end)
                        {
                            MemCpy(dataPtr, p64, BlockSize);
                            
                            h0 += _data[0];    h2 ^= h10;   h11 ^= h0;   h0 =   h0 << 11 | h0 >> -11;   h11 += h1;
                            h1 += _data[1];    h3 ^= h11;   h0 ^= h1;    h1 =   h1 << 32 | h1 >> 32;    h0 += h2;
                            h2 += _data[2];    h4 ^= h0;    h1 ^= h2;    h2 =   h2 << 43 | h2 >> -43;   h1 += h3;
                            h3 += _data[3];    h5 ^= h1;    h2 ^= h3;    h3 =   h3 << 31 | h3 >> -31;   h2 += h4;
                            h4 += _data[4];    h6 ^= h2;    h3 ^= h4;    h4 =   h4 << 17 | h4 >> -17;   h3 += h5;
                            h5 += _data[5];    h7 ^= h3;    h4 ^= h5;    h5 =   h5 << 28 | h5 >> -28;   h4 += h6;
                            h6 += _data[6];    h8 ^= h4;    h5 ^= h6;    h6 =   h6 << 39 | h6 >> -39;   h5 += h7;
                            h7 += _data[7];    h9 ^= h5;    h6 ^= h7;    h7 =   h7 << 57 | h7 >> -57;   h6 += h8;
                            h8 += _data[8];    h10 ^= h6;   h7 ^= h8;    h8 =    h8 <<55 | h8 >> -55;   h7 += h9;
                            h9 += _data[9];    h11 ^= h7;   h8 ^= h9;    h9 =   h9 << 54 | h9 >> -54;   h8 += h10;
                            h10 += _data[10];  h0 ^= h8;    h9 ^= h10;   h10 = h10 << 22 | h10 >> -22;  h9 += h11;
                            h11 += _data[11];  h1 ^= h9;    h10 ^= h11;  h11 = h11 << 46 | h11 >> -46;  h10 += h0;
                            p64 += NumVars;
                        }
                _remainder = remainder;
                MemCpy(p64Fixed, end, remainder);
            }
            _state0 = h0;
            _state1 = h1;
            _state2 = h2;
            _state3 = h3;
            _state4 = h4;
            _state5 = h5;
            _state6 = h6;
            _state7 = h7;
            _state8 = h8;
            _state9 = h9;
            _state10 = h10;
            _state11 = h11;
        }
        public void Final(out long hash1, out long hash2)
        {
            ulong uhash1, uhash2;
            Final(out uhash1, out uhash2);
            hash1 = (long)uhash1;
            hash2 = (long)uhash2;
        }
        [CLSCompliant(false)]
        public unsafe void Final(out ulong hash1, out ulong hash2)
        {
            if (_length < BufSize)
            {
                hash1 = _state0;
                hash2 = _state1;
                fixed(void* ptr = _data)
                    Short(ptr, _length, ref hash1, ref hash2);
                return;
            }
            ulong h0 = _state0;
            ulong h1 = _state1;
            ulong h2 = _state2;
            ulong h3 = _state3;
            ulong h4 = _state4;
            ulong h5 = _state5;
            ulong h6 = _state6;
            ulong h7 = _state7;
            ulong h8 = _state8;
            ulong h9 = _state9;
            ulong h10 = _state10;
            ulong h11 = _state11;
            fixed(ulong* dataFixed = _data)
            {
                ulong* data = dataFixed;
                byte remainder = _remainder;
                if (remainder >= BlockSize)
                {
                    h0 += data[0];    h2 ^= h10;    h11 ^= h0;    h0 = h0 << 11 | h0 >> -11;    h11 += h1;
                    h1 += data[1];    h3 ^= h11;    h0 ^= h1;    h1 = h1 << 32 | h1 >> 32;    h0 += h2;
                    h2 += data[2];    h4 ^= h0;    h1 ^= h2;    h2 = h2 << 43 | h2 >> -43;    h1 += h3;
                    h3 += data[3];    h5 ^= h1;    h2 ^= h3;    h3 = h3 << 31 | h3 >> -31;    h2 += h4;
                    h4 += data[4];    h6 ^= h2;    h3 ^= h4;    h4 = h4 << 17 | h4 >> -17;    h3 += h5;
                    h5 += data[5];    h7 ^= h3;    h4 ^= h5;    h5 = h5 << 28 | h5 >> -28;    h4 += h6;
                    h6 += data[6];    h8 ^= h4;    h5 ^= h6;    h6 = h6 << 39 | h6 >> -39;    h5 += h7;
                    h7 += data[7];    h9 ^= h5;    h6 ^= h7;    h7 = h7 << 57 | h7 >> -57;    h6 += h8;
                    h8 += data[8];    h10 ^= h6;    h7 ^= h8;    h8 = h8 << 55 | h8 >> -55;    h7 += h9;
                    h9 += data[9];    h11 ^= h7;    h8 ^= h9;    h9 = h9 << 54 | h9 >> -54;    h8 += h10;
                    h10 += data[10];    h0 ^= h8;    h9 ^= h10;    h10 = h10 << 22 | h10 >> -22;    h9 += h11;
                    h11 += data[11];    h1 ^= h9;    h10 ^= h11;    h11 = h11 << 46 | h11 >> -46;    h10 += h0;
                    data += NumVars;
                    remainder = (byte)(remainder - BlockSize);
                }
                MemZero(((byte*)data) + remainder, BlockSize - remainder);
                *((byte*)data + BlockSize - 1) = remainder;
                h0 += data[0];   h1 += data[1];   h2 += data[2];   h3 += data[3];
                h4 += data[4];   h5 += data[5];   h6 += data[6];   h7 += data[7];
                h8 += data[8];   h9 += data[9];   h10 += data[10]; h11 += data[11];
            }
            h11+= h1;    h2 ^= h11;   h1 = h1 << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2 << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3 << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4 << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5 << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6 << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7 << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8 << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9 << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0 << 54 | h0 >> -54;
            h11+= h1;    h2 ^= h11;   h1 = h1 << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2 << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3 << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4 << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5 << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6 << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7 << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8 << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9 << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0 << 54 | h0 >> -54;
            h11+= h1;    h2 ^= h11;   h1 = h1 << 44 | h1 >> -44;
            h0 += h2;    h3 ^= h0;    h2 = h2 << 15 | h2 >> -15;
            h1 += h3;    h4 ^= h1;    h3 = h3 << 34 | h3 >> -34;
            h2 += h4;    h5 ^= h2;    h4 = h4 << 21 | h4 >> -21;
            h3 += h5;    h6 ^= h3;    h5 = h5 << 38 | h5 >> -38;
            h4 += h6;    h7 ^= h4;    h6 = h6 << 33 | h6 >> -33;
            h5 += h7;    h8 ^= h5;    h7 = h7 << 10 | h7 >> -10;
            h6 += h8;    h9 ^= h6;    h8 = h8 << 13 | h8 >> -13;
            h7 += h9;    h10^= h7;    h9 = h9 << 38 | h9 >> -38;
            h8 += h10;   h11^= h8;    h10= h10 << 53 | h10 >> -53;
            h9 += h11;   h0 ^= h9;    h11= h11 << 42 | h11 >> -42;
            h10+= h0;    h1 ^= h10;   h0 = h0 << 54 | h0 >> -54;
            hash1 = h0;
            hash2 = h1;
        }
    }
    public static class SpookyHasher
    {
        public unsafe static Tuple<long, long> SpookyHash128(this string str, long seed0, long seed1)
        {
            if(str == null)
                return Tuple.Create(0L, 0L);
            ulong hash1 = (ulong)seed0;
            ulong hash2 = (ulong)seed1;
            fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
                SpookyHash.Hash128(ptr, str.Length, ref hash1, ref hash2);
            return Tuple.Create((long)hash1, (long)hash2);
        }
        public static Tuple<long, long> SpookyHash128(this string str)
        {
            return unchecked(SpookyHash128(str, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public unsafe static long SpookyHash64(this string str, long seed)
        {
            if(str == null)
                return 0L;
            ulong hash = (ulong)seed;
            fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
                return (long)SpookyHash.Hash64(ptr, str.Length, hash);
        }
        public static long SpookyHash64(this string str)
        {
            return unchecked(SpookyHash64(str, (long)SpookyHash.SpookyConst));
        }
        public unsafe static int SpookyHash32(this string str, int seed)
        {
            if(str == null)
                return 0;
            uint hash = (uint)seed;
            fixed(char* ptr = str + RuntimeHelpers.OffsetToStringData)
                return (int)SpookyHash.Hash32(ptr, str.Length, hash);
        }
        public static int SpookyHash32(this string str)
        {
            return unchecked(SpookyHash32(str, (int)SpookyHash.SpookyConst));
        }
        public unsafe static Tuple<long, long> SpookyHash128(this char[] message, int startIndex, int length, long seed0, long seed1)
        {
            if(message == null)
                return Tuple.Create(0L, 0L);
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            ulong hash1 = (ulong)seed0;
            ulong hash2 = (ulong)seed1;
            fixed(char* ptr = message)
                SpookyHash.Hash128(ptr + startIndex, length * 2, ref hash1, ref hash2);
            return Tuple.Create((long)hash1, (long)hash2);
        }
        public static Tuple<long, long> SpookyHash128(this char[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash128(message, startIndex, length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public unsafe static long SpookyHash64(this char[] message, int startIndex, int length, long seed)
        {
            if(message == null)
                return 0;
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            ulong hash = (ulong)seed;
            fixed(char* ptr = message)
                return (long)SpookyHash.Hash64(ptr + startIndex, length * 2, hash);
        }
        public static long SpookyHash64(this char[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash64(message, startIndex, length, (long)SpookyHash.SpookyConst));
        }
        public unsafe static int SpookyHash32(this char[] message, int startIndex, int length, int seed)
        {
            if(message == null)
                return 0;
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            uint hash = (uint)seed;
            fixed(char* ptr = message)
                return (int)SpookyHash.Hash32(ptr + startIndex, length * 2, hash);
        }
        public static int SpookyHash32(this char[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash32(message, startIndex, length, (int)SpookyHash.SpookyConst));
        }
        public static Tuple<long, long> SpookyHash128(this char[] message, long seed0, long seed1)
        {
            return SpookyHash128(message, 0, message.Length, seed0, seed1);
        }
        public static Tuple<long, long> SpookyHash128(this char[] message)
        {
            return unchecked(SpookyHash128(message, 0, message.Length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public unsafe static long SpookyHash64(this char[] message, long seed)
        {
            return SpookyHash64(message, 0, 0, seed);
        }
        public static long SpookyHash64(this char[] message)
        {
            return unchecked(SpookyHash64(message, 0, message.Length, (long)SpookyHash.SpookyConst));
        }
        public unsafe static int SpookyHash32(this char[] message, int seed)
        {
            return SpookyHash32(message, 0, message.Length, seed);
        }
        public static int SpookyHash32(this char[] message)
        {
            return unchecked(SpookyHash32(message, 0, message.Length, (int)SpookyHash.SpookyConst));
        }
        public unsafe static Tuple<long, long> SpookyHash128(this byte[] message, int startIndex, int length, long seed0, long seed1)
        {
            if(message == null)
                return Tuple.Create(0L, 0L);
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            ulong hash1 = (ulong)seed0;
            ulong hash2 = (ulong)seed1;
            fixed(byte* ptr = message)
                SpookyHash.Hash128(ptr + startIndex, length, ref hash1, ref hash2);
            return Tuple.Create((long)hash1, (long)hash2);
        }
        public static Tuple<long, long> SpookyHash128(this byte[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash128(message, startIndex, length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public unsafe static long SpookyHash64(this byte[] message, int startIndex, int length, long seed)
        {
            if(message == null)
                return 0;
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            ulong hash = (ulong)seed;
            fixed(byte* ptr = message)
                return (long)SpookyHash.Hash64(ptr + startIndex, length, hash);
        }
        public static long SpookyHash64(this byte[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash64(message, startIndex, length, (long)SpookyHash.SpookyConst));
        }
        public unsafe static int SpookyHash32(this byte[] message, int startIndex, int length, int seed)
        {
            if(message == null)
                return 0;
            if(startIndex < 0 || startIndex > message.Length)
                throw new ArgumentOutOfRangeException("startIndex");
            if(startIndex + length > message.Length)
                throw new ArgumentException();
            uint hash = (uint)seed;
            fixed(byte* ptr = message)
                return (int)SpookyHash.Hash32(ptr + startIndex, length, hash);
        }
        public static int SpookyHash32(this byte[] message, int startIndex, int length)
        {
            return unchecked(SpookyHash32(message, startIndex, length, (int)SpookyHash.SpookyConst));
        }
        public static Tuple<long, long> SpookyHash128(this byte[] message, long seed0, long seed1)
        {
            return SpookyHash128(message, 0, message.Length, seed0, seed1);
        }
        public static Tuple<long, long> SpookyHash128(this byte[] message)
        {
            return unchecked(SpookyHash128(message, 0, message.Length, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public unsafe static long SpookyHash64(this byte[] message, long seed)
        {
            return SpookyHash64(message, 0, 0, seed);
        }
        public static long SpookyHash64(this byte[] message)
        {
            return unchecked(SpookyHash64(message, 0, message.Length, (int)SpookyHash.SpookyConst));
        }
        public unsafe static int SpookyHash32(this byte[] message, int seed)
        {
            return SpookyHash32(message, 0, message.Length, seed);
        }
        public static int SpookyHash32(this byte[] message)
        {
            return unchecked(SpookyHash32(message, 0, message.Length, (int)SpookyHash.SpookyConst));
        }
        public unsafe static Tuple<long, long> SpookyHash128(this Stream stream, long seed0, long seed1)
        {
            if(stream == null)
                throw new ArgumentNullException("stream");
            var hash = new SpookyHash(seed0, seed1);
            var buffer = new byte[4096];
            fixed(void* ptr = buffer)
                for(int len = stream.Read(buffer, 0, 4096); len != 0; len = stream.Read(buffer, 0, 4096))
                    hash.Update(ptr, len);
            long hash0, hash1;
            hash.Final(out hash0, out hash1);
            return Tuple.Create(hash0, hash1);
        }
        public static Tuple<long, long> SpookyHash128(this Stream stream)
        {
            return unchecked(SpookyHash128(stream, (long)SpookyHash.SpookyConst, (long)SpookyHash.SpookyConst));
        }
        public static long SpookyHash64(this Stream stream, long seed)
        {
            return SpookyHash128(stream, seed, seed).Item1;
        }
        public static long SpookyHash64(this Stream stream)
        {
            return unchecked(SpookyHash64(stream, (long)SpookyHash.SpookyConst));
        }
        public static int Hash32(this Stream stream, int seed)
        {
            return unchecked((int)SpookyHash128(stream, seed, seed).Item1);
        }
        public static int Hash32(this Stream stream)
        {
            return unchecked(Hash32(stream, (int)SpookyHash.SpookyConst));
        }
    }
}