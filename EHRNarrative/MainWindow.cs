using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace EHRNarrative
{
    public partial class EHRNarrative : Form
    {
        private ArrayList keyword_list = null;
        private Thread slc_thread = null;

        private volatile bool stopping = false;

        public EHRNarrative()
        {
            InitializeComponent();

            keyword_list = new ArrayList();

            slc_thread = new Thread(SLCServerThread);
            slc_thread.Start();

            System.Diagnostics.Process.Start("SLC.exe");

            HealthRecordText.SelectAll();
        }

        private void EHRNarrative_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopping = true;
        }

        private void HealthRecordText_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"\[([^]]*)\]";
            Regex rgx = new Regex(pattern);

            ArrayList new_keyword_list = new ArrayList();
            foreach (Match match in rgx.Matches(HealthRecordText.Text))
            {
                string match_str = match.Value;

                new_keyword_list.Add(match_str);
            }

            IEnumerable removed_keywords = keyword_list.ToArray().Except(new_keyword_list.ToArray());
            IEnumerable added_keywords = new_keyword_list.ToArray().Except(keyword_list.ToArray());

            last_action_label.Text = "Last Action:\n";
            string command_string = "";
            foreach (string keyword in added_keywords)
            {
                last_action_label.Text += "Added " + keyword + "\n";
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "add " + keyword);
                //command_string += "add " + keyword;
            }
            foreach (string keyword in removed_keywords)
            {
                last_action_label.Text += "Removed " + keyword + "\n";
                if (command_string != "")
                {
                    command_string += " ! ";
                }
                //System.Diagnostics.Process.Start("SLC.exe", "del " + keyword);
                command_string += "data " + keyword;
            }
            if (command_string != "")
            {
                System.Diagnostics.Process.Start("SLC.exe", command_string);
            }

            keyword_list = new_keyword_list;
            current_label.Text = "Current Keywords:\n";
            foreach (string keyword in keyword_list)
            {
                current_label.Text += keyword + "\n";
            }
        }

        private static void SLCServerThread(object data)
        {
            NamedPipeServerStream slc_server = new NamedPipeServerStream("mu_slc_pipe", PipeDirection.InOut, 2);

            int threadId = Thread.CurrentThread.ManagedThreadId;

            // Wait for a client to connect
            slc_server.WaitForConnection();

            Console.WriteLine("Client connected.");
            try
            {
                StreamString ss = new StreamString(slc_server);

                ss.WriteString("I am the one True Server!");

                string response = ss.ReadString();

                if (response == "Hello True Server.")
                {
                    ss.WriteString("Greetings, young SLC.");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            slc_server.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    // Defines the data protocol for reading and writing strings on our stream 
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
            int len = 0;

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
