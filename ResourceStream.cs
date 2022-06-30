using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streams.Resources
{
    public class ResourceReaderStream : Stream
    {
        private readonly Stream stream;
        private string key;

        private long pointer = -1;
        byte[] buffer = new byte[Constants.BufferSize];

        private string readKeyDecoded = null;
        private byte[] readValue = null;
        private long readValuePointer = 0;

        public ResourceReaderStream(Stream stream, string key)
        {
            this.stream = stream;
            this.key = key;
        }

        // if key not found yet: SeekValue();
        // if value is not read yet: ReadFieldValue(...)
        // else return 0;
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (readKeyDecoded == null)
                SeekValue();

            if (readValue == null)
            {
                readValue = ReadSection().ToArray();
            }

            if(readValuePointer < readValue.Length)
            {
                long start = readValuePointer;
                for (int i = 0; i < 100; i++)
                {
                    if (readValuePointer < readValue.Length)
                    {
                        buffer[offset + i] = readValue[readValuePointer];
                        readValuePointer++;
                    }
                    else break;
                }

                return (int)(readValuePointer - start);
            }

            else return 0;
        }

        // while not end of stream read next section key, compare with required key
        // and skip value if read key is not equal to required key
        private void SeekValue()
        {
            while (true)
            {
                List<byte> readKey = ReadSection();
               
                string s = Encoding.ASCII.GetString(readKey.ToArray());
                if (s == key)
                {
                    readKeyDecoded = s;
                    break;
                }
            }
        }

        private void CheckBufferAndRead()
        {
            int count = -1;
            if (pointer == -1 || pointer == 1024)
            {
                count = stream.Read(buffer, 0, Constants.BufferSize);
                pointer = 0;

                if (count == 0) throw new EndOfStreamException();
            }
        }

        private List<byte> ReadSection()
        {
            CheckBufferAndRead();

            List<byte> section = new List<byte>();
            byte prevprev = 255;
            byte prev = 255;
            byte next = buffer[pointer];
            pointer++;

            while (true)
            {
                CheckBufferAndRead();

                prevprev = prev;
                prev = next;
                next = buffer[pointer];
                if (prev == 0 && next == 1 && prevprev != 0)
                    break;

                if (!(prev == 0 && prevprev == 0))
                    section.Add(prev);

                pointer++;
            }

            pointer++;
            return section;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
        public override void Flush()
        {
            // nothing to do
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
