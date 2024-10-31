
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;

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
        public void RunConcurrentStackSample()
        {
            FillUpCoinFlip(1000);
            Task readerOne = Task.Run(() => GetFromStack());
            Task readerTwo = Task.Run(() => GetFromStack());
            Task readerThree = Task.Run(() => GetFromStack());
            Task readerFour = Task.Run(() => GetFromStack());
            Task.WaitAll(readerOne, readerTwo, readerThree, readerFour);
        }

        static void FillUpCoinFlip(int max)
        {

            //_coinstack.Clear();

            SerialPort port = new SerialPort("COM4");
            port.DtrEnable = true;
            Boolean good = false;
            uint trycount = 0;
            while (good == false)
            {
                try
                {
                    port.Open();
                    good = true;
                }
                catch { trycount++; }
            } //This prevents a crash if this code is ran simultaniously in another instance


            byte[] buffer = new byte[16];

            bool flip = false;
            for (int i = 0; i < max; i++)
            {
                int byread = port.Read(buffer, 0, 16);
                for (int j = 0; j < byread; j++)
                {
                    byte readDataByte = (byte)buffer[j];
                    flip = (readDataByte & 0x01) == 0x01;
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
            }

            port.Close();

        }
        static void FillUpDice(int die, int max)
        {

            //_coinstack.Clear();

            SerialPort port = new SerialPort("COM4");
            port.DtrEnable = true;
            Boolean good = false;
            uint trycount = 0;
            while (good == false)
            {
                try
                {
                    port.Open();
                    good = true;
                }
                catch { trycount++; }
            } //This prevents a crash if this code is ran simultaniously in another instance


            byte[] buffer = new byte[16];


            for (int i = 0; i < max; i++)
            {
                int byread = port.Read(buffer, 0, 16);
                for (int j = 0; j < byread; j++)
                    {
                        int readDataByte = (int)buffer[j] + 1;
                        if (die == 2)
                        {
                         bool   flip = (readDataByte & 0x01) == 0x01;
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
            }

            port.Close();

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
                        FillUpDice(2, 10000);
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
                        FillUpDice(4, 10000);
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
                        FillUpDice(6, 10000);
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
                        FillUpDice(8, 10000);
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
                        FillUpDice(10, 1000);
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
                        FillUpDice(12, 10000);
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
                        FillUpDice(20, 10000);
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
                        FillUpDice(100, 1000);
                        i--;
                    }
                }
            }

            return totalroll;
        }

        static void GetFromStack()
        {
            bool res;
            bool success = _coinstack.TryPop(out res);
            while (success)
            {
                Debug.WriteLine(res);
                success = _coinstack.TryPop(out res);
            }
        }

        static void Main(string[] args)
        {

            bool flip = false;
            var stopwatch = new Stopwatch();
            long[] results = new long[101];
            double[] perf = new double[101];
            long i;

            //int diefaces = 6;
            int numdie = 1;
            int maxChar = 0;
            int Char = 0;
            int[] supportedDie = { 2, 4, 6, 8, 10, 12, 20, 100 };

            foreach (int diefaces in supportedDie)
            {
                for (int j = 0; j <= 100; j++)
                    results[j] = 0;
                stopwatch.Reset();
                stopwatch.Start();
                Console.Write("Starting....");
                for ( i = 0; i < 10000000; i++)
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
                        double per = 1 / freq;
                        double rat = per * (double)(diefaces ^ numdie);

                        Console.WriteLine(j.ToString() + ": " + freq.ToString() +"   --  "+ var.ToString() );

                    }


                    //Console.WriteLine("Heads: " + h.ToString());
                    //Console.WriteLine("Tails: " + t.ToString());

                }

                Console.WriteLine("ms: " + elap.ToString());
                Console.WriteLine("chars: " + i.ToString());
                perf[diefaces] = ((i) / (elap / 1000.0));
                Console.WriteLine("rate: " + ((i) / (elap / 1000.0)).ToString() + " rolls per second");
            }
            foreach (int diefaces in supportedDie)
            {
                Console.WriteLine("die: " + diefaces.ToString()+ "    -  "+ perf[diefaces] + " rolls per second");
            }
                //Console.WriteLine("Heads: " + h.ToString());
                //Console.WriteLine("Tails: " + t.ToString());




            }

    }
}