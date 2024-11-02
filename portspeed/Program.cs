
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.ComponentModel;  // for BackgroundWorker
using Console = System.Console;


namespace TrueRNGRanger
{

    class Program
    {


        public static ConcurrentStack<bool> _coinstack = new ConcurrentStack<bool>();
        public static ConcurrentStack<byte> _die4stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die6stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die8stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die10stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die12stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die20stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die100stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _randomBytes = new ConcurrentStack<byte>();



        static void FillUpDice(int die, int max)
        {
            // This method converts from the concurrant byte stack into rolls / flips of the relevant die type in as efficient a manner as possible. It pushes the results onto a concurrent queue of the relevant type.
            int readDataByte;
            byte readByte;
            bool success;
            for (int i = 0; i < max; i++)
            {

                success = _randomBytes.TryPop(out readByte);

                if (success)

                {
                    readDataByte = (int)readByte + 1;
                    if (die == 2)
                    {
                        bool flip = (readDataByte & 0x01) == 0x01;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x02) == 0x02;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x04) == 0x04;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x08) == 0x08;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x10) == 0x10;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x20) == 0x20;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x40) == 0x40;
                        _coinstack.Push(flip);
                        flip = (readDataByte & 0x80) == 0x80;
                        _coinstack.Push(flip);
                    }

                    if (die == 4)
                    {
                        _die4stack.Push((byte)((readDataByte % 4) + 1));
                        _die4stack.Push((byte)(((readDataByte / 4) % 4) + 1));
                        _die4stack.Push((byte)(((readDataByte / 64) % 4) + 1));
                    }
                    if (die == 6)
                    {
                        if (readDataByte <= 216)
                        {
                            _die6stack.Push((byte)((readDataByte % 6) + 1));
                            _die6stack.Push((byte)(((readDataByte / 6) % 6) + 1));
                            _die6stack.Push((byte)(((readDataByte / 36) % 6) + 1));
                        }
                        else if (readDataByte <= 252)
                        {
                            _die6stack.Push((byte)(((readDataByte - 216) % 6) + 1));
                            _die6stack.Push((byte)((((readDataByte - 216) / 6) % 6) + 1));
                        }

                    }
                    if (die == 8)
                    {
                        if (readDataByte <= 64)
                        {
                            _die8stack.Push((byte)((readDataByte % 8) + 1));
                            _die8stack.Push((byte)(((readDataByte / 8) % 8) + 1));
                        }
                        else if (readDataByte <= 128)
                        {
                            _die8stack.Push((byte)(((readDataByte - 64) % 8) + 1));
                            _die8stack.Push((byte)((((readDataByte - 64) / 8) % 8) + 1));
                        }
                        else if (readDataByte <= 192)
                        {
                            _die8stack.Push((byte)(((readDataByte - 128) % 8) + 1));
                            _die8stack.Push((byte)((((readDataByte - 128) / 8) % 8) + 1));
                        }
                        else if (readDataByte <= 256)
                        {
                            _die8stack.Push((byte)(((readDataByte - 192) % 8) + 1));
                            _die8stack.Push((byte)((((readDataByte - 192) / 8) % 8) + 1));
                        }

                    }
                    if (die == 10)
                    {
                        if (readDataByte <= 100)
                        {
                            _die10stack.Push((byte)((readDataByte % 10) + 1));
                            _die10stack.Push((byte)(((readDataByte / 10) % 10) + 1));
                        }
                        else if (readDataByte <= 200)
                        {
                            _die10stack.Push((byte)(((readDataByte - 100) % 10) + 1));
                            _die10stack.Push((byte)((((readDataByte - 100) / 10) % 10) + 1));
                        }
                        else if (readDataByte <= 210)
                        {
                            _die10stack.Push((byte)(((readDataByte - 200) % 10) + 1));
                        }
                        else if (readDataByte <= 220)
                        {
                            _die10stack.Push((byte)(((readDataByte - 210) % 10) + 1));
                        }
                        else if (readDataByte <= 230)
                        {
                            _die10stack.Push((byte)(((readDataByte - 220) % 10) + 1));
                        }
                        else if (readDataByte <= 240)
                        {
                            _die10stack.Push((byte)(((readDataByte - 230) % 10) + 1));
                        }
                        else if (readDataByte <= 250)
                        {
                            _die10stack.Push((byte)(((readDataByte - 240) % 10) + 1));
                        }

                    }
                    if (die == 12)
                    {
                        if (readDataByte <= 144)
                        {
                            _die12stack.Push((byte)((readDataByte % 12) + 1));
                            _die12stack.Push((byte)(((readDataByte / 12) % 12 + 1)));
                        }
                        else if (readDataByte <= 252)
                        {
                            _die12stack.Push((byte)(((readDataByte - 144) % 12) + 1));

                        }


                    }
                    if (die == 20)
                    {
                        if (readDataByte <= 240)
                        {
                            _die20stack.Push((byte)((readDataByte % 20) + 1));
                        }

                    }
                    if (die == 100)
                    {
                        if (readDataByte <= 100)
                            _die100stack.Push((byte)((readDataByte % 100) + 1));
                        else if (readDataByte <= 200)
                            _die100stack.Push((byte)(((readDataByte - 100) % 100) + 1));

                    }
                }
                else
                {
                    //just skip and wait for the random byte stack to be populated by the background process
                    i--;
                }
            }
        }

        static int RollDice(int die, int count)
        {
            //only d6 implemented for die....

            byte dieres;
            int totalroll = 0;
            bool success;

            if (die == 2)
            {
                for (int i = 0; i < count; i++)
                {
                    bool res;
                    success = _coinstack.TryPop(out res);
                    if (success)
                    {
                        if (res) totalroll++;
                    }
                    else
                    {
                        FillUpDice(2, 100);
                        i--;
                    }
                }
            }


            if (die == 4)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die4stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(4, 100);
                        i--;
                    }
                }
            }
            if (die == 6)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die6stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(6, 100);
                        i--;
                    }
                }
            }
            if (die == 8)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die8stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(8, 100);
                        i--;
                    }
                }
            }
            if (die == 10)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die10stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(10, 100);
                        i--;
                    }
                }
            }
            if (die == 12)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die12stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(12, 100);
                        i--;
                    }
                }
            }
            if (die == 20)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die20stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(20, 100);
                        i--;
                    }
                }
            }

            if (die == 100)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die100stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres;
                    }
                    else
                    {
                        FillUpDice(100, 100);
                        i--;
                    }
                }
            }

            return totalroll;
        }

        static void worker_ProgressChanged(object _, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Buffer size: {0:d}", e.ProgressPercentage);
        }

        static void worker_streamRandomBytes(object sender, DoWorkEventArgs e)
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
                        e.Result=(long)e.Result+1;
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





        static void worker_RunWorkerCompleted(object _, RunWorkerCompletedEventArgs e)
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


        static void Main(string[] args)
        {
            BackgroundWorker worker = new();
            worker.DoWork += worker_streamRandomBytes;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerAsync();

            //Console.ReadKey(true);  // event loop

            var stopwatch = new Stopwatch();
            long[] results = new long[101];
            double[] perf = new double[101];
            long i;

            //int diefaces = 6;
            int numdie = 1;
            int[] supportedDie = { 2, 4, 6, 8, 10, 12, 20, 100 };

            foreach (int diefaces in supportedDie)
            {
                for (int j = 0; j <= 100; j++)
                    results[j] = 0;
                stopwatch.Reset();
                stopwatch.Start();
                Console.Write("Starting....");

                for (i = 0; i < 1000000; i++)
                {

                    int rol = RollDice(diefaces, numdie);
                    results[rol]++;

                }
                var elap = stopwatch.ElapsedMilliseconds;
                for (int j = 0; j < 101; j++)
                {
                    if (results[j] > 0)
                    {
                        double freq = (double)i / (double)results[j];
                        double var = freq - (double)diefaces;

                        Console.WriteLine(j.ToString() + ": " + results[j].ToString() + " rolls. Frequency 1:" + freq.ToString() + "   --  Variance " + var.ToString());

                    }

                }

                Console.WriteLine("ms: " + elap.ToString());
                Console.WriteLine("chars: " + i.ToString());
                perf[diefaces] = ((i) / (elap / 1000.0));
                Console.WriteLine("rate: " + ((i) / (elap / 1000.0)).ToString() + " rolls per second");

            }
            foreach (int diefaces in supportedDie)
            {
                Console.WriteLine("die: " + diefaces.ToString() + "    -  " + perf[diefaces] + " rolls per second");
            }


            if (worker.IsBusy)
            {
                Console.WriteLine("Stopping the worker...");
                worker.CancelAsync();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (worker.IsBusy && sw.ElapsedMilliseconds < 5000)
                    System.Threading.Thread.Sleep(1);

            }


        }

    }
}