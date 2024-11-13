
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.ComponentModel;  // for BackgroundWorker
using Console = System.Console;
using System.Timers;
using static TrueRNGRanger.DiceClass;
using Accord.Math.Distances;
using portspeed;


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

            /*
             * Streaming D6 if needed
            BackgroundWorker worker2 = new();
            worker2.DoWork += DiceClass.worker_streamD6;
            worker2.RunWorkerCompleted += DiceClass.worker_streamD6Completed;
            //worker2.ProgressChanged += HardwareRNGinterface.worker_ProgressChanged;
            worker2.WorkerReportsProgress = true;
            worker2.WorkerSupportsCancellation = true;

            worker2.RunWorkerAsync();
            */

            long elap;
            double[] perf = new double[257];
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();

            //int[] testDie = { 2, 6,  20, 100 };
            //int[] testDie = { 256 };
            int[] testDie = { 2,4,6,8,10,12,20,100,256 };
            dieEval[] dieEvals = new dieEval[testDie.Length];

            Task[] rollDiceTask = new Task[testDie.Length];
            int i = 0;
            int numRolls = 20000;
            int series = 100;


            // Code to test parallel running
            /*
            foreach (int diefaces in testDie)
            {
                rollDiceTask[i] = Task.Run(() => Tests.TestDie(diefaces, numRolls, series, @"d:\temp\D" + diefaces.ToString() + ".csv"));
                i++;
            }
            Task.WaitAll(rollDiceTask);
            */
            // Code to test single threaded running
            foreach (int diefaces in testDie)
            {

                dieEvals[i] = Tests.TestDie(diefaces, numRolls, series,@"d:\temp\D"+diefaces.ToString()+".csv");
                Console.WriteLine("D{0:D}:  Rolls: {1:D}  Seconds: {2:N}  Rolls/sec {3:N}", diefaces,dieEvals[i].rolls, dieEvals[i].seconds, (long)(dieEvals[i].rolls / dieEvals[i].seconds));
                i++;
            }
            

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