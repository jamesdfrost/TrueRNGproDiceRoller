using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Console = System.Console;

namespace TrueRNGRanger
{
    internal static class DiceClass
    {

        public static ConcurrentStack<bool> _coinstack = new ConcurrentStack<bool>();
        public static ConcurrentStack<byte> _die4stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die6stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die8stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die10stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die12stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die20stack = new ConcurrentStack<byte>();
        public static ConcurrentStack<byte> _die100stack = new ConcurrentStack<byte>();

        static void FillUpDice(int die, int max)
        {
            // This method converts from the concurrant byte stack into rolls / flips of the relevant die type in as efficient a manner as possible. It pushes the results onto a concurrent queue of the relevant type.
            int readDataByte;
            byte readByte;
            bool success;
            for (int i = 0; i < max; i++)
            {

                success = HardwareRNGinterface._randomBytes.TryPop(out readByte);

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
                        if (res) totalroll = totalroll + 2; //Heads = 2, Tails = 1
                        else totalroll++;
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

        public static long TestDie(int numdie, int diefaces, int rolls)
        {
            long i,elap;
            int pc10 = rolls / 10;

            Console.Write("Starting....");
            long[] results = new long[diefaces];
            double[] probs = new double[diefaces];

            for (int j = 0; j < diefaces; j++)
            {
                results[j] = 0;
                probs[j] = 1 / (double)diefaces;
                    }
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            for (int pc = 10; pc <= 100; pc = pc + 10)
            {
                for (i = 0; i < pc10; i++)
                {

                    int rol = DiceClass.RollDice(diefaces, numdie);
                    results[rol-1]++;

                }
                Console.WriteLine("Worker: D{0:D} - {1:D}%", diefaces,pc);
            }
            Console.WriteLine("Is dice fair?");


            StatisticsTests.ShowVector(results);

            
            Console.WriteLine("Probabilities if fair:");
            StatisticsTests.ShowVector(probs, 4);
            double[] expected = StatisticsTests.ExpectedFromProbs(probs, rolls);
            Console.WriteLine("Expected counts if fair:");
            StatisticsTests.ShowVector(expected, 1);
            double chi = StatisticsTests.ChiFromProbs(results, probs);
            Console.WriteLine("Calculated chi-squared = " +
              chi.ToString("F2"));  // 3.66

            
           // int df = results.Length - 1;
            double pval = StatisticsTests.ChiSquarePval(chi, diefaces-1);
            Console.WriteLine("The pval with df of " + diefaces +
              " = " + pval.ToString("F4"));
            Console.WriteLine("Pval is probability, if wheel fair,");

            


            elap = stopwatch.ElapsedMilliseconds;
            for (int j = 0; j <diefaces; j++)
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
    }
}