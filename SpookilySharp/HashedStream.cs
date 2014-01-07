// HashedStream.cs
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
    /// <summary>Wraps a stream, and maintains a hash of the bytes written to and from it.</summary>
    public class HashedStream : Stream
    {
        private readonly Stream _backing;
        private readonly SpookyHash _read;
        private readonly SpookyHash _written;
        private bool _moved;

        /// <summary>Initialises a new instance of the <see cref="SpookilySharp.HashedStream"/> class.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="readSeed0">The first 64 bits of the seed for the hash of contents read.</param>
        /// <param name="readSeed1">The second 64 bits of the seed for the hash of contents read.</param>
        /// <param name="writeSeed0">The first 64 bits of the seed for the hash of contents written.</param>
        /// <param name="writeSeed1">The second 64 bits of the seed for the hash of contents written.</param>
        [CLSCompliant(false)]
        public HashedStream(Stream stream, ulong readSeed0, ulong readSeed1, ulong writeSeed0, ulong writeSeed1)
        {
            stream.CheckNotNull();
            _backing = stream;
            _read = new SpookyHash(readSeed0, readSeed1);
            _written = new SpookyHash(writeSeed0, writeSeed1);
        }

        /// <summary>Initialises a new instance of the <see cref="SpookilySharp.HashedStream"/> class.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="readSeed0">The first 64 bits of the seed for the hash of contents read.</param>
        /// <param name="readSeed1">The second 64 bits of the seed for the hash of contents read.</param>
        /// <param name="writeSeed0">The first 64 bits of the seed for the hash of contents written.</param>
        /// <param name="writeSeed1">The second 64 bits of the seed for the hash of contents written.</param>
        public HashedStream(Stream stream, long readSeed0, long readSeed1, long writeSeed0, long writeSeed1)
            : this(stream, (ulong)readSeed0, (ulong)readSeed1, (ulong)writeSeed0, (ulong)writeSeed1)
        {
        }

        /// <summary>Initialises a new instance of the <see cref="SpookilySharp.HashedStream"/> class.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="seed0">The first 64 bits of the seed for both the hash of contents read and the hash of the contents written.</param>
        /// <param name="seed1">The second 64 bits of the seed for both the hash of contents read and the hash of the contents written.</param>
        [CLSCompliant(false)]
        public HashedStream(Stream stream, ulong seed0, ulong seed1)
            : this(stream, seed0, seed1, seed0, seed1)
        {
        }

        /// <summary>Initialises a new instance of the <see cref="SpookilySharp.HashedStream"/> class.</summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="seed0">The first 64 bits of the seed for both the hash of contents read and the hash of the contents written.</param>
        /// <param name="seed1">The second 64 bits of the seed for both the hash of contents read and the hash of the contents written.</param>
        public HashedStream(Stream stream, long seed0, long seed1)
            : this(stream, (ulong)seed0, (ulong)seed1)
        {
        }

        /// <summary>Initialises a new instance of the <see cref="SpookilySharp.HashedStream"/> class.</summary>
        /// <param name="stream">The stream to read.</param>
        public HashedStream(Stream stream)
        {
            stream.CheckNotNull();
            _backing = stream;
            _read = new SpookyHash();
            _written = new SpookyHash();
        }

        /// <summary>Gets a value indicating whether this instance can read.</summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
        public override bool CanRead
        {
            get { return _backing.CanRead; }
        }

        /// <summary>Gets a value indicating whether this instance can seek.</summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
        public override bool CanSeek
        {
            get { return _backing.CanSeek; }
        }

        /// <summary>Gets a value indicating whether this instance can timeout.</summary>
        /// <value><c>true</c> if this instance can timeout; otherwise, <c>false</c>.</value>
        public override bool CanTimeout
        {
            get { return _backing.CanTimeout; }
        }

        /// <summary>Gets a value indicating whether this instance can write.</summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
        public override bool CanWrite
        {
            get { return _backing.CanWrite; }
        }

        /// <summary>Closes the stream.</summary>
        public override void Close()
        {
            base.Close();
            _backing.Close();
        }

        /// <summary>Dispose the specified disposing.</summary>
        /// <param name="disposing">If set to <c>true</c> disposing, otherwise finalising.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
                _backing.Dispose();
        }

        /// <summary>Flush the stream.</summary>
        public override void Flush()
        {
            _backing.Flush();
        }

        /// <summary>Gets the length of the stream.</summary>
        /// <value>The length.</value>
        public override long Length
        {
            get { return _backing.Length; }
        }

        /// <summary>Gets or sets the position within the stream.</summary>
        /// <value>The position.</value>
        public override long Position
        {
            get
            {
                return _backing.Position;
            }
            set
            {
                _backing.Position = value;
                _moved = true;
            }
        }

        /// <summary>Read the specified <paramref name="count"/> number of bytes, into <paramref name="buffer"/>, starting at <paramref name="offset"/>.</summary>
        /// <param name="buffer">The array to read into.</param>
        /// <param name="offset">The position within <paramref name="buffer"/> to copy bytes into.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            count = _backing.Read(buffer, offset, count);
            _read.Update(buffer, offset, count);
            return count;
        }

        /// <summary>Reads a single byte.</summary>
        /// <returns>The byte's value, or <c>-1</c> if at the end of the stream.</returns>
        public override int ReadByte()
        {
            int ret = _backing.ReadByte();
            if(ret != -1)
                _read.Update((byte)ret);
            return ret;
        }

        /// <summary>Gets or sets the read timeout.</summary>
        /// <value>The read timeout.</value>
        public override int ReadTimeout
        {
            get { return _backing.ReadTimeout; }
            set { _backing.ReadTimeout = value; }
        }

        /// <summary>Seek to a position within the stream.</summary>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="origin">Position from which to seek.</param>
        /// <returns>The new position within the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            long ret = _backing.Seek(offset, origin);
            _moved = true;
            return ret;
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The new length.</param>
        public override void SetLength(long value)
        {
            _backing.SetLength(value);
            _moved = true;
        }

        /// <summary>
        /// Write the specified <paramref name="count"/> of bytes from <paramref name="buffer"/>, starting from <paramref name="offset"/>, into the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write from.</param>
        /// <param name="offset">Offset within buffer, to write from.</param>
        /// <param name="count">Number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _backing.Write(buffer, offset, count);
            _written.Update(buffer, offset, count);
        }

        /// <summary>Write a single byte into the stream.</summary>
        /// <param name="value">The byte to write.</param>
        public override void WriteByte(byte value)
        {
            _backing.WriteByte(value);
            _written.Update(value);
        }

        /// <summary>Gets or sets the write timeout.</summary>
        /// <value>The write timeout.</value>
        public override int WriteTimeout
        {
            get { return _backing.WriteTimeout; }
            set { _backing.WriteTimeout = value; }
        }

        /// <summary>Gets the current 128-bite hash of what has been written so far.</summary>
        /// <value>The hash, so far.</value>
        public HashCode128 WriteHash
        {
            get { return _written.Final(); }
        }

        /// <summary>Gets the current 64-bite hash of what has been written so far.</summary>
        /// <value>The hash, so far.</value>
        public long WriteHash64
        {
            get { return WriteHash.Hash1; }
        }

        /// <summary>Gets the current 32-bite hash of what has been written so far.</summary>
        /// <value>The hash, so far.</value>
        public int WriteHash32
        {
            get { return (int)WriteHash.Hash1; }
        }

        /// <summary>Gets the current 128-bite hash of what has been read so far.</summary>
        /// <value>The hash, so far.</value>
        public HashCode128 ReadHash
        {
            get { return _read.Final(); }
        }

        /// <summary>Gets the current 64-bite hash of what has been read so far.</summary>
        /// <value>The hash, so far.</value>
        public long ReadHash64
        {
            get { return ReadHash.Hash1; }
        }

        /// <summary>Gets the current 32-bite hash of what has been read so far.</summary>
        /// <value>The hash, so far.</value>
        public int ReadHash32
        {
            get { return (int)ReadHash.Hash1; }
        }

        /// <summary>Gets a value indicating whether there had been an operation that moved the point being read from or written to.</summary>
        /// <value><c>true</c> if was moved; otherwise, <c>false</c>.</value>
        /// <remarks>Operations such as <see cref="SetLength"/> or setting properties such as <see cref="Position"/> will mean that while the hashes will remain correct hashes of the values written and read, they may not correspond with e.g. the hash obtained by hashing the contents of a file the stream is backed by, etc.</remarks>
        public bool WasMoved
        {
            get { return _moved; }
        }
    }
}