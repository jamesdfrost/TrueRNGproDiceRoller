using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO.Ports;
using Console = System.Console;

namespace TrueRNGRanger
{
    internal static class HardwareRNGinterface
    {
        public static ConcurrentStack<byte> _randomBytes = new ConcurrentStack<byte>();
        static internal void worker_ProgressChanged(object _, ProgressChangedEventArgs e)
        {
            //Console.WriteLine("Buffer size: {0:d}", e.ProgressPercentage);
        }





        static internal void worker_RunWorkerCompleted(object _, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("Worker: Shutting Down!");
                Console.WriteLine("Worker: I worked {0:D} times.", e.Result);
                return;
            }

            Console.WriteLine("Worker: I worked {0:D} times.", e.Result);
            Console.WriteLine("Worker: Done now!");
        }

        static internal void worker_streamRandomBytes(object sender, DoWorkEventArgs e)
        {
            string strPort = "COM4"; //COM port your RNG is connected to. Should really move this to a config setting. Set to NONE if you want to use internal Pseudo Random number generator for testing purpose.
            int numBytesToRead = 16; //Number of bytes to try and read at once from the RNG. Depending on your RNG, this setting could be worth tweaking (up/down) to see if it impacts performance.
            int bufferSize = 10000000; //Size of buffer needed. Higher values will use more memory, but the buffer is generally way faster than reading off the RNG so if your workload is peaky having a large buffer being constantly updated can have a big (positive) impact on performance
            Random rand = new Random(); //only needed if using NONE above.

            Console.WriteLine("Worker: Starting to connect to " + strPort + "...\n");
            BackgroundWorker? worker = sender as BackgroundWorker;
            if (worker != null)
            {
                SerialPort port = new SerialPort(strPort);
                port.DtrEnable = true;
                Boolean portOpen = false;
                uint trycount = 0;
                byte[] buffer = new byte[16];
                int byread = numBytesToRead;

                if (strPort != "NONE")
                {
                    while (portOpen == false)
                    {
                        try
                        {
                            port.Open();
                            portOpen = true;
                        }
                        catch
                        {
                            Console.WriteLine("Worker: Failed to connect to " + strPort + "...\n");
                            trycount++;
                        }
                    } //This prevents a crash if this port doesnt open.
                }
                e.Result = (long)0;

                while (!worker.CancellationPending)
                {
                    if (_randomBytes.Count < bufferSize)
                    {
                        e.Result = (long)e.Result + 1;
                        for (int i = 0; i < 100000; i++)
                        {
                            if (strPort == "NONE")
                            {
                                rand.NextBytes(buffer);
                            }
                            else
                            {
                                byread = port.Read(buffer, 0, 16);
                            }
                            for (int j = 0; j < byread; j++)
                            {
                                byte readDataByte = buffer[j];
                                //if (readDataByte == 0 && j <=2) readDataByte = 1; //introduce a small bias....
                                _randomBytes.Push(readDataByte);
                            }

                            if (worker.CancellationPending)
                                break;
                        }
                        worker.ReportProgress((int)_randomBytes.Count);
                    }
                }

                port.Close();
                e.Cancel = worker.CancellationPending;
            }
        }
    }
}