
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.ComponentModel;  // for BackgroundWorker
using Console = System.Console;
using System.Timers;
using static TrueRNGRanger.DiceClass;


namespace TrueRNGRanger
{

    class Program
    {





        static void Main(string[] args)
        {
            BackgroundWorker worker = new();
            worker.DoWork += HardwareRNGinterface.worker_streamRandomBytes;
            worker.RunWorkerCompleted += HardwareRNGinterface.worker_RunWorkerCompleted;
            worker.ProgressChanged += HardwareRNGinterface.worker_ProgressChanged;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerAsync();
            long elap;

            //Console.ReadKey(true);  // event loop



            double[] perf = new double[257];

            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            //int diefaces = 6;


            //int[] supportedDie = { 2, 6,  20, 100 };
            //int[] supportedDie = { 12 };
            int[] supportedDie = { 2,4,6,8,10,12,20,100,256 };
            dieEval[] dieEvals = new dieEval[supportedDie.Length];

            Task[] rollDiceTask = new Task[supportedDie.Length];
            int i = 0;

            foreach (int diefaces in supportedDie)
            {
                int numRolls = 20000;
                // Code to test parallel running rollDiceTask[i] = Task.Run(() => TestDie(numdie, diefaces, numRolls));
                Console.WriteLine("Testing D{0:D}", diefaces);
                dieEvals[i] = DiceClass.TestDie(diefaces, numRolls, 100000, false,@"d:\temp\D"+diefaces.ToString()+".csv");
                // perf[diefaces] = ((numRolls) / (elap / 1000.0));
                i++;
            }

            //Task.WaitAll(rollDiceTask);

            elap = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("\n\nTotal Elapsed: " + elap.ToString() + "\n\n\n");
            string fair = "";
            Console.WriteLine("DieFaces,Rolls,Seconds,AvgP,stdDev,Fair,RollsPerSec");
            foreach (dieEval die in dieEvals)
            {
                if (die.fair) fair = "FAIR"; else fair = "UNFAIR";

                Console.WriteLine("{0:D},{1:D},{2:N},{3:N},{4:N},{5},{6:D}", die.diefaces,die.rolls,die.seconds,die.avgP,die.stdDev,fair, (long)(die.rolls/ die.seconds));
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