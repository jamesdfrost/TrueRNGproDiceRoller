
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



            double[] perf = new double[101];

            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            //int diefaces = 6;
            int numdie = 1;
            int[] supportedDie = { 2, 4, 6, 8, 10, 12, 20, 100 };
            Task[] rollDiceTask = new Task[supportedDie.Length];
            int i = 0;
            foreach (int diefaces in supportedDie)
            {
                int numRolls = 100000000;
                // Code to test parallel running rollDiceTask[i] = Task.Run(() => TestDie(numdie, diefaces, numRolls));

                elap = DiceClass.TestDie(numdie, diefaces, numRolls);
                perf[diefaces] = ((numRolls) / (elap / 1000.0));
                i++;
            }
            //Task.WaitAll(rollDiceTask);

            elap = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("\n\nElapsed: " + elap.ToString() + "\n\n\n");


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