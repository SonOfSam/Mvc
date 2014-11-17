// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Rendering;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// A <see cref="TextWriter"/> that is backed by a unbuffered writer (over the Response stream) and a buffered
    /// <see cref="StringCollectionTextWriter"/>. When <c>Flush</c> or <c>FlushAsync</c> is invoked, the writer
    /// copies all content from the buffered writer to the unbuffered one and switches to writing to the unbuffered
    /// writer for all further write operations.
    /// </summary>
    /// <remarks>
    /// This type is designed to avoid creating large in-memory strings when buffering and supporting the contract that
    /// <see cref="RazorPage.FlushAsync"/> expects.
    /// </remarks>
    public class RazorTextWriter : TextWriter, IBufferedTextWriter
    {
        private readonly FlushPointManager _flushPointManager;
        private StringCollectionTextWriter _bufferedWriter;

        /// <summary>
        /// Creates a new instance of <see cref="RazorTextWriter"/>.
        /// </summary>
        /// <param name="unbufferedWriter">The <see cref="TextWriter"/> to write output to when this instance
        /// is no longer buffering.</param>
        /// <param name="encoding">The character <see cref="Encoding"/> in which the output is written.</param>
        public RazorTextWriter(TextWriter unbufferedWriter, Encoding encoding)
        {
            UnbufferedWriter = unbufferedWriter;
            Encoding = encoding;
            _flushPointManager = new FlushPointManager(unbufferedWriter);
            _bufferedWriter = new StringCollectionTextWriter(encoding);
        }

        /// <inheritdoc />
        public override Encoding Encoding { get; }

        /// <inheritdoc />
        public bool IsBuffering { get; private set; } = true;

        // Internal for unit testing
        internal StringCollectionTextWriter BufferedWriter
        {
            get { return _bufferedWriter; }
        }

        private TextWriter UnbufferedWriter { get; }

        /// <inheritdoc />
        public override void Write(char value)
        {
            BufferedWriter.Write(value);
        }

        /// <inheritdoc />
        public override void Write(object value)
        {
            var htmlString = value as HtmlString;
            if (htmlString != null)
            {
                htmlString.WriteTo(BufferedWriter);
                return;
            }

            base.Write(value);
        }

        /// <inheritdoc />
        public override void Write([NotNull] char[] buffer, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0 || (buffer.Length - index < count))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            BufferedWriter.Write(buffer, index, count);
        }

        /// <inheritdoc />
        public override void Write(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                BufferedWriter.Write(value);
            }
        }

        /// <inheritdoc />
        public override Task WriteAsync(char value)
        {
            return BufferedWriter.WriteAsync(value);
        }

        /// <inheritdoc />
        public override Task WriteAsync([NotNull] char[] buffer, int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0 || (buffer.Length - index < count))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return BufferedWriter.WriteAsync(buffer, index, count);
        }

        /// <inheritdoc />
        public override Task WriteAsync(string value)
        {
            return BufferedWriter.WriteAsync(value);
        }

        /// <inheritdoc />
        public override void WriteLine()
        {
            BufferedWriter.WriteLine();
        }

        /// <inheritdoc />
        public override void WriteLine(string value)
        {
            BufferedWriter.WriteLine(value);
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(char value)
        {
            return BufferedWriter.WriteLineAsync(value);
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(char[] value, int start, int offset)
        {
            return BufferedWriter.WriteLineAsync(value, start, offset);
        }

        /// <inheritdoc />
        public override Task WriteLineAsync(string value)
        {
            return BufferedWriter.WriteLineAsync(value);
        }

        /// <inheritdoc />
        public override Task WriteLineAsync()
        {
            return BufferedWriter.WriteLineAsync();
        }

        /// <summary>
        /// Copies the buffered content to the unbuffered writer and invokes flush on it.
        /// Additionally causes this instance to no longer buffer and direct all write operations
        /// to the unbuffered writer.
        /// </summary>
        public override void Flush()
        {
            IsBuffering = false;
            var oldWriter = Interlocked.Exchange(ref _bufferedWriter, new StringCollectionTextWriter(Encoding));
            _flushPointManager.QueueForFlush(oldWriter);
        }

        /// <summary>
        /// Copies the buffered content to the unbuffered writer and invokes flush on it.
        /// Additionally causes this instance to no longer buffer and direct all write operations
        /// to the unbuffered writer.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous copy and flush operations.</returns>
        public override Task FlushAsync()
        {
            IsBuffering = false;
            return _flushPointManager.FlushAsync(_bufferedWriter);
        }

        /// <inheritdoc />
        public void CopyTo(TextWriter writer)
        {
            if (!IsBuffering)
            {
                throw new InvalidOperationException();
            }

            _bufferedWriter.CopyTo(writer);
        }

        /// <inheritdoc />
        public Task CopyToAsync(TextWriter writer)
        {
            if (!IsBuffering)
            {
                throw new InvalidOperationException();
            }

            return _bufferedWriter.CopyToAsync(writer);
        }

        private sealed class FlushPointManager
        {
            private readonly object _lock = new object();
            private readonly TextWriter _targetWriter;
            private Task _flushTask;

            public FlushPointManager(TextWriter targetWriter)
            {
                _targetWriter = targetWriter;
            }

            public Task QueueForFlush(StringCollectionTextWriter writer)
            {
                lock (_lock)
                {
                    _flushTask = FlushAsync(writer);
                    return _flushTask;
                }
            }

            public async Task FlushAsync(StringCollectionTextWriter writer)
            {
                var previousFlushTask = _flushTask;
                if (previousFlushTask != null)
                {
                    await previousFlushTask;
                }

                await writer.CopyToAsync(_targetWriter);
                await _targetWriter.FlushAsync();
            }
        }
    }
}