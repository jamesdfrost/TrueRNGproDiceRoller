
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.ComponentModel;  // for BackgroundWorker
using Console = System.Console;
using System.Timers;


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
            //Console.WriteLine("Buffer size: {0:d}", e.ProgressPercentage);
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
            long elap;

            //Console.ReadKey(true);  // event loop



            double[] perf = new double[101];

            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            //int diefaces = 6;
            int numdie = 1;
            int[] supportedDie = { 2, 4, 6, 8, 10, 12, 20, 100 };
            Task[] rollDiceTask = new Task[supportedDie.Length];
            int i= 0;
            foreach (int diefaces in supportedDie)
            {
                int numRolls = 10000000;
                // Code to test parallel running rollDiceTask[i] = Task.Run(() => TestDie(numdie, diefaces, numRolls));

                elap = TestDie(numdie, diefaces, numRolls);
                perf[diefaces] = ((numRolls) / (elap / 1000.0));
                i++;
            }
            //Task.WaitAll(rollDiceTask);
           
            elap = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("\n\nElapsed: " + elap.ToString()+ "\n\n\n");


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

        private static long TestDie(int numdie, int diefaces, int rolls)
        {
            long i,elap;
            int pc10 = rolls / 10;

            Console.Write("Starting....");
            long[] results = new long[101];
            for (int j = 0; j <= 100; j++)
                results[j] = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            for (int pc = 10; pc <= 100; pc = pc + 10)
            {
                for (i = 0; i < pc10; i++)
                {

                    int rol = RollDice(diefaces, numdie);
                    results[rol]++;

                }
                Console.WriteLine("Worker: D{0:D} - {1:D}%", diefaces,pc);
            }
            elap = stopwatch.ElapsedMilliseconds;
            for (int j = 0; j < 101; j++)
            {
                if (results[j] > 0)
                {
                    double freq = (double)rolls / (double)results[j];
                    double var = freq - (double)diefaces;

                    Console.WriteLine(j.ToString() + ": " + results[j].ToString() + " rolls. Frequency 1:" + freq.ToString() + "   --  Variance " + var.ToString());

                }

            }

            Console.WriteLine("ms: " + elap.ToString());
            Console.WriteLine("rolls: " + rolls.ToString());
            Console.WriteLine("rate: " + ((rolls) / (elap / 1000.0)).ToString() + " rolls per second");
            return elap;
        }

        public static double ChiSquarePval(double x, int df)
        {
            // x = a computed chi-square value.
            // df = degrees of freedom.
            // output = prob. x value occurred by chance.
            // ACM 299.
            if (x <= 0.0 || df < 1)
                throw new Exception("Bad arg in ChiSquarePval()");
            double a = 0.0; // 299 variable names
            double y = 0.0;
            double s = 0.0;
            double z = 0.0;
            double ee = 0.0; // change from e
            double c;
            bool even; // Is df even?
            a = 0.5 * x;
            if (df % 2 == 0) even = true; else even = false;
            if (df > 1) y = Exp(-a); // ACM update remark (4)
            if (even == true) s = y;
            else s = 2.0 * Gauss(-Math.Sqrt(x));
            if (df > 2)
            {
                x = 0.5 * (df - 1.0);
                if (even == true) z = 1.0; else z = 0.5;
                if (a > 40.0) // ACM remark (5)
                {
                    if (even == true) ee = 0.0;
                    else ee = 0.5723649429247000870717135;
                    c = Math.Log(a); // log base e
                    while (z <= x)
                    {
                        ee = Math.Log(z) + ee;
                        s = s + Exp(c * z - a - ee); // ACM update remark (6)
                        z = z + 1.0;
                    }
                    return s;
                } // a > 40.0
                else
                {
                    if (even == true) ee = 1.0;
                    else
                        ee = 0.5641895835477562869480795 / Math.Sqrt(a);
                    c = 0.0;
                    while (z <= x)
                    {
                        ee = ee * (a / z); // ACM update remark (7)
                        c = c + ee;
                        z = z + 1.0;
                    }
                    return c * y + s;
                }
            } // df > 2
            else
            {
                return s;
            }
        } // ChiSquarePval()
        private static double Exp(double x)
        {
            if (x < -40.0) // ACM update remark (8)
                return 0.0;
            else
                return Math.Exp(x);
        }
        public static double Gauss(double z)
        {
            // input = z-value (-inf to +inf)
            // output = p under Normal curve from -inf to z
            // ACM Algorithm #209
            double y; // 209 scratch variable
            double p; // result. called ‘z’ in 209
            double w; // 209 scratch variable
            if (z == 0.0)
                p = 0.0;
            else
            {
                y = Math.Abs(z) / 2;
                if (y >= 3.0)
                {
                    p = 1.0;
                }
                else if (y < 1.0)
                {
                    w = y * y;
                    p = ((((((((0.000124818987 * w
                      - 0.001075204047) * w + 0.005198775019) * w
                      - 0.019198292004) * w + 0.059054035642) * w
                      - 0.151968751364) * w + 0.319152932694) * w
                      - 0.531923007300) * w + 0.797884560593) * y
                      * 2.0;
                }
                else
                {
                    y = y - 2.0;
                    p = (((((((((((((-0.000045255659 * y
                      + 0.000152529290) * y - 0.000019538132) * y
                      - 0.000676904986) * y + 0.001390604284) * y
                      - 0.000794620820) * y - 0.002034254874) * y
                     + 0.006549791214) * y - 0.010557625006) * y
                     + 0.011630447319) * y - 0.009279453341) * y
                     + 0.005353579108) * y - 0.002141268741) * y
                     + 0.000535310849) * y + 0.999936657524;
                }
            }
            if (z > 0.0)
                return (p + 1.0) / 2;
            else
                return (1.0 - p) / 2;
        } // Gauss()
        public static double ChiFromProbs(int[] observed, double[] probs)
        {
            int n = observed.Length;
            int sumObs = 0;
            for (int i = 0; i < n; ++i)
                sumObs += observed[i];
            double[] expected = ExpectedFromProbs(probs, sumObs);
            return ChiFromFreqs(observed, expected);
        }

        public static double[] ExpectedFromProbs(double[] probs,
  int N)
        {
            double[] expected = new double[probs.Length];
            for (int i = 0; i < probs.Length; ++i)
                expected[i] = probs[i] * N;
            return expected;
        }
        public static double ChiFromFreqs(int[] observed,  double[] expected)
        {
            double sum = 0.0;
            for (int i = 0; i < observed.Length; ++i)
            {
                sum += ((observed[i] - expected[i]) *
                  (observed[i] - expected[i])) / expected[i];
            }
            return sum;
        }
    }
}