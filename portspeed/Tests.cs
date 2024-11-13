using Accord.Statistics;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TrueRNGRanger.DiceClass;
using TrueRNGRanger;
using portspeed;
using Accord.Statistics.Kernels;
using Accord.Statistics.Testing;

namespace portspeed
{
    internal class Tests
    {
        public static dieEval TestDie(int diefaces, int rolls, int repetitions, string logFilePath)
        {

            var csv = new StringBuilder();
            string newLine;
            dieEval dieFeedback = new dieEval();
            dieFeedback.diefaces = diefaces;
            long i, elap;
            int pc10 = rolls / 10;
            double stPeter;
            double[] pvals = new double[repetitions];

            double[] results = new double[diefaces];
            var stopwatch = new Stopwatch();
            int maxRoll = diefaces;
            long[] inarow = new long[100];
            double[] pDist = new double[10];
            double[] expected = new double[diefaces];
            int currentInARow = 0;
            stopwatch.Reset();
            stopwatch.Start();
            double runningP = 0;
            const int totalTicks = 100;
            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };
            var pbar = new ProgressBar(totalTicks, "Processing d" + diefaces.ToString(), options);

            for (int pc = 1; pc <= 100; pc = pc + 1)
            {
                pbar.Tick(); 
                for (int x = 0; x < repetitions / 100; x++)
                {
                    int maxBatchInARow = 0;
                    for (int j = 0; j < diefaces; j++)
                    {
                        results[j] = 0;
     
                        expected[j] = rolls/ (double)diefaces;
                    }
                    for (i = 0; i < rolls; i++)
                    {
                        int rol = DiceClass.RollDice(diefaces, 1);
                        if (rol == maxRoll)
                            currentInARow++;
                        else
                        {
                            inarow[currentInARow]++;
                            if (currentInARow > maxBatchInARow) maxBatchInARow = currentInARow;
                            currentInARow = 0;
                        }
                        results[rol - 1]++;
                    }

                    var chinew = new ChiSquareTest(results, expected, diefaces-1 );
                    double chi = chinew.Statistic;
                    double pval = chinew.PValue;
                    pvals[x] = pval;
                    runningP += pval;
                    if (pval >= 0 && pval <= 1)
                    {
                        int intPval = (int)Math.Truncate(pval * 10);
                        if (intPval == 10) intPval = 9;
                        pDist[intPval]++;
                    }
                    string rollResults = "";
                    string inaRowResults = "";
                    for (int j = 0; j < diefaces; j++)
                    {
                        rollResults = rollResults + results[j].ToString() + ",";
                    }
                    stPeter = 0;
                    for (int j = 1; j <= maxBatchInARow; j++)
                    {
                        inaRowResults = inaRowResults + inarow[j].ToString() + ",";
                        stPeter += Math.Pow(diefaces, j + 1);
                        inarow[j] = 0;
                    }
                    double maxStPeter = Math.Pow(diefaces, maxBatchInARow + 1);
                    newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", x.ToString(), chi.ToString(), pval.ToString(), maxStPeter.ToString(), (stPeter / (double)rolls).ToString(), rollResults, inaRowResults);
                    csv.AppendLine(newLine);
                }
            }
            elap = stopwatch.ElapsedMilliseconds;

            double[] probs2 = new double[100];
            probs2[0] = ((double)(diefaces - 1) / (double)diefaces);
            int maxInarow = 0;
            for (int j = 1; j < 100; j++)
            {
                probs2[j] = (double)(probs2[j - 1] / (double)diefaces);
                if (inarow[j] > 0 || (probs2[j] * rolls * repetitions) > 0.5) maxInarow = j;
            }
            var stdDev = Measures.StandardDeviation(pDist);

            if (stdDev / (float)repetitions < 0.1) //&& pval2>0.01)
                dieFeedback.fair = true;
            else
                dieFeedback.fair = false;

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
                csvRollheader2 = csvRollheader2 + j.ToString() + " in a row" + ",";
            }

            var header = string.Format("Series,Chi,Pval,Max St Peters,Avg St Peters,{0},{1}\n", csvRollheader, csvRollheader2);
            var csvOut = header + csv;
            if (logFilePath != "")
            {
                File.WriteAllText(logFilePath, csvOut.ToString());
            }
            return dieFeedback;
        }
    }
}
