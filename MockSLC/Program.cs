using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;

namespace MockSLC
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string str in args)
            {
                Console.WriteLine(str);
            }

            NamedPipeClientStream client = new NamedPipeClientStream(".", "mu_slc_pipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);
            client.Connect();

            StreamString ss = new StreamString(client);
            if (ss.ReadString() == "I am the one True Server!")
            {
                ss.WriteString("Hello True Server.");

                Console.Write(ss.ReadString());
            }

            client.Close();

            Console.ReadLine();
        }
    }

    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
}
