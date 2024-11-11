using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Console = System.Console;
using Accord.Statistics;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.ComponentModel.Design;
namespace TrueRNGRanger
{
    internal static class DiceClass
    {

        private static ConcurrentStack<bool> _coinstack = new ConcurrentStack<bool>();
        private static ConcurrentStack<byte> _die4stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die6stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die8stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die10stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die12stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die20stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die100stack = new ConcurrentStack<byte>();
        private static ConcurrentStack<byte> _die256stack = new ConcurrentStack<byte>();
        /*
         * 
         * 
         *             Console.WriteLine("Dice: D{0:N}", diefaces);
            Console.WriteLine("stDev: {0:N}", stdDev);
            Console.WriteLine("AvgP: {0:N}", runningP/repetitions);
            Console.WriteLine("ms: " + elap.ToString());
            Console.WriteLine("rolls: " + (rolls*repetitions).ToString());
            Console.WriteLine("rate: " + ((rolls * repetitions) / (elap / 1000.0)).ToString() + " rolls per second");
*/

        public class dieEval
        {
            public int diefaces { get; set; }
            public long rolls { get; set; }
            public double seconds { get; set; }
            public double avgP { get; set; }
            public double stdDev { get; set; }
            public bool fair { get; set; }



        }
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
                        if (readDataByte <= 256)
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
                    /*if (die == 20)
                    {
                        byte[] readBytes = new byte[5];
                        int x = 0;
                        while (x < 5)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        //long bigVal=readByte*

                            if (readDataByte <= 240)
                        {
                            _die20stack.Push((byte)((readDataByte % 20) + 1));
                        }

                    }*/
                    if (die == 100)
                    {
                        if (readDataByte <= 100)
                            _die100stack.Push((byte)((readDataByte % 100) + 1));
                        else if (readDataByte <= 200)
                            _die100stack.Push((byte)(((readDataByte - 100) % 100) + 1));

                    }
                    if (die == 256)
                    {

                            _die256stack.Push((byte)readDataByte);


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

            if (die == 256)
            {
                for (int i = 0; i < count; i++)
                {
                    success = _die256stack.TryPop(out dieres);
                    if (success)
                    {
                        totalroll += dieres+1;
                    }
                    else
                    {
                        FillUpDice(256, 100);
                        i--;
                    }
                }
            }

            return totalroll;
        }

        public static dieEval TestDie(int diefaces, int rolls ,int repetitions,bool showProgress,string logFilePath)
        {

            var csv = new StringBuilder();
            string newLine;

            //after your loop


            dieEval dieFeedback = new dieEval();
            dieFeedback.diefaces = diefaces;
            long i,elap;
            int pc10 = rolls / 10;
            double stPeter;
            double[] pvals= new double[repetitions];
            if (showProgress) Console.Write("Starting....");
            long[] results = new long[diefaces];
            double[] probs = new double[diefaces];
            var stopwatch = new Stopwatch();
            int maxRoll = diefaces ;
            long[] inarow = new long[100];
            double[] pDist = new double[10];
            int currentInARow = 0;
            stopwatch.Reset();
            stopwatch.Start();
            double runningP = 0;

            for (int x = 0; x < repetitions; x++)
            {
                int maxBatchInARow = 0;
                for (int j = 0; j < diefaces; j++)
                {
                    results[j] = 0;
                    probs[j] = 1 / (double)diefaces;
                }

                for (int pc = 10; pc <= 100; pc = pc + 10)
                {
                    for (i = 0; i < pc10; i++)
                    {
                        int rol = DiceClass.RollDice(diefaces, 1);
                        if (rol ==maxRoll ) 
                            currentInARow++;
                        else
                        {
                            inarow[currentInARow]++;
                            if (currentInARow>maxBatchInARow) maxBatchInARow = currentInARow;
                            currentInARow = 0;

                        }
                        results[rol - 1]++;

                    }
                    if (showProgress) Console.WriteLine("Worker: D{0:D} - {1:D}%", diefaces, pc);
                }
                //Console.WriteLine("Is dice fair?");


                if (showProgress) Console.WriteLine("Run,Result,Rolls,Expected,Frequency,Variance");

                for (int j = 0; j < diefaces; j++)
                {
                    if (results[j] > 0)
                    {
                        double freq = (double)rolls / (double)results[j];
                        double var = freq - (double)diefaces;
                        if (showProgress) Console.WriteLine("{0:D},{1:D},{2:D},{3:D},{4:E},{5:E}", x,j, results[j], rolls / diefaces, freq, freq - (double)diefaces);

                    }

                }



                //double[] expected = StatisticsTests.ExpectedFromProbs(probs, rolls);
                double chi = StatisticsTests.ChiFromProbs(results, probs);
                //Console.WriteLine("Calculated chi-squared, " + chi.ToString("F2"));  // 3.66


                // int df = results.Length - 1;
                double pval = StatisticsTests.ChiSquarePval(chi, diefaces - 1);
                pvals[x] = pval;
                runningP += pval;
                //Console.WriteLine("The pval with df of " + diefaces +               "," + pval.ToString("F4"));
                if (pval >= 0 && pval <= 1)
                {
                    int intPval = (int)Math.Truncate(pval * 10);
                    if (intPval == 10) intPval = 9;
                    pDist[intPval]++;
                        }
                string rollResults = "";
                string inaRowResults = "";
                for (int j = 0;j<diefaces;j++)
                {
                    rollResults = rollResults + results[j].ToString() + ",";
                }
                stPeter = 0;
                for (int j = 1; j <= maxBatchInARow; j++)
                {
                    inaRowResults = inaRowResults + inarow[j].ToString() + ",";
                    stPeter += Math.Pow(diefaces, j+1);
                    inarow[j] = 0;
                }
                double maxStPeter = Math.Pow(diefaces, maxBatchInARow+1);
                newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", x.ToString(),chi.ToString(),pval.ToString(), maxStPeter.ToString(),(stPeter/(double)rolls).ToString(),rollResults, inaRowResults);
                csv.AppendLine(newLine);
                Console.WriteLine("Completed batch {0:D}", x);

            }
            elap = stopwatch.ElapsedMilliseconds;
            if (showProgress)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (inarow[j] > 0)
                    {
                        Console.WriteLine("{0:D},{1:D}", j, inarow[j]);

                    }
                    Console.WriteLine("{0:N}", pDist[j]);

                }
            }


            
            double[] probs2 = new double[100];
            probs2[0] = ((double)(diefaces - 1) / (double)diefaces);
            int maxInarow = 0;
            for (int j = 1; j < 100; j++)
            {
                probs2[j] = (double)(probs2[j - 1] / (double)diefaces);
                if (inarow[j] > 0 || (probs2[j]*rolls*repetitions)>0.5) maxInarow = j;
            }
            double chi2 = StatisticsTests.ChiFromProbs(inarow, probs2, maxInarow);
            double pval2 = StatisticsTests.ChiSquarePval(chi2, maxInarow);
                        
            


            var stdDev=Measures.StandardDeviation(pDist);
            if (showProgress)
            {
                Console.WriteLine("AvgP2: {0:N}", pval2); 
                Console.WriteLine("Dice: D{0:N}", diefaces);
                Console.WriteLine("stDev: {0:N}", stdDev);
                Console.WriteLine("AvgP: {0:N}", runningP / repetitions);
                Console.WriteLine("ms: " + elap.ToString());
                Console.WriteLine("rolls: " + (rolls * repetitions).ToString());
                Console.WriteLine("rate: " + ((rolls * repetitions) / (elap / 1000.0)).ToString() + " rolls per second");
            }

            if (stdDev / (float)repetitions < 0.1) //&& pval2>0.01)
            {
                if (showProgress) Console.WriteLine("Die Seems Fair");
                dieFeedback.fair = true;
            }
            else
            {
                if (showProgress) Console.WriteLine("WARNING - Die has potential issues");
                dieFeedback.fair = false;
            }
            dieFeedback.seconds = (elap / 1000.0);
            dieFeedback.avgP = runningP / repetitions;
            dieFeedback.stdDev = stdDev;
            dieFeedback.rolls = rolls * repetitions;

            string csvRollheader = "";
            if (diefaces == 2)
            {
                csvRollheader = "tails,heads";
            }
            else
            {
                for (int j = 0; j < diefaces; j++)
                {
                    csvRollheader = csvRollheader + "roll " + (j + 1).ToString();
                    if (j < diefaces - 1)
                        csvRollheader = csvRollheader + ",";
                }
            }
            string csvRollheader2 = "";
            for (int j = 0; j <= maxInarow; j++)
            {
                csvRollheader2 = csvRollheader2 + j.ToString()+" in a row" +",";
            }
            


            var header = string.Format("Series,Chi,Pval,Max St Peters,Avg St Peters,{0},{1}\n", csvRollheader,csvRollheader2);

            var csvOut=header+csv;


            if (logFilePath != "")
            {
                File.WriteAllText(logFilePath, csvOut.ToString());
            }
            return dieFeedback;
        }
    }
}