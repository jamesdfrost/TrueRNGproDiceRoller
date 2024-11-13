using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Console = System.Console;
using Accord.Statistics;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.IO.Ports;
using ShellProgressBar;

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


        public class dieEval
        {
            public int diefaces { get; set; }
            public long rolls { get; set; }
            public double seconds { get; set; }
            public double avgP { get; set; }
            public double stdDev { get; set; }
            public bool fair { get; set; }
        }

        static internal void worker_streamD6Completed(object _, RunWorkerCompletedEventArgs e)
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

        static internal void worker_streamD6(object sender, DoWorkEventArgs e)
        {
            //Implement a streaming version of D6.
            int bufferSize = 100000; //Size of buffer needed. Higher values will use more memory, but the buffer is generally way faster than reading off the RNG so if your workload is peaky having a large buffer being constantly updated can have a big (positive) impact on performance
            Random rand = new Random(); //only needed if using NONE above.
            int readDataByte;
            byte readByte;
            bool success;

            Console.WriteLine("Worker: Streaming d6");
            BackgroundWorker? worker = sender as BackgroundWorker;
            if (worker != null)
            {
                while (!worker.CancellationPending)
                {
                    if (_die6stack.Count < bufferSize)
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readByte);
                            if (success)
                            {
                                readDataByte = (int)readByte + 1;
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
                        }
                    }
                }
                e.Cancel = worker.CancellationPending;
            }
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
                        _die4stack.Push((byte)(((readDataByte / 16) % 4) + 1));
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
                        byte[] readBytes = new byte[2];
                        int x = 0;
                        while (x < 2)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        long bigVal = ((long)readByte * 65536) + ((long)readBytes[0] * 256) + ((long)readBytes[1]);
                        _die8stack.Push((byte)((bigVal % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 8) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 64) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 512) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 4096) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 32768) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 262144) % 8) + 1));
                        _die8stack.Push((byte)(((bigVal / 2097152) % 8) + 1));
                    }
                    if (die == 10)
                    {
                        byte[] readBytes = new byte[5];
                        int x = 0;
                        while (x < 4)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        long bigVal = ((long)readBytes[0] * 4294967296) + ((long)readBytes[1] * 16777216) + ((long)readBytes[2] * 65536) + ((long)readBytes[3] * 256) + (long)(readByte);
                        if (bigVal <= 1000000000000)
                        {
                            _die10stack.Push((byte)((bigVal % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 10) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 100) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 1000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 10000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 100000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 1000000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 10000000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 100000000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 1000000000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 10000000000) % 10) + 1));
                            _die10stack.Push((byte)(((bigVal / 100000000000) % 10) + 1));
                        }
                    }
                    if (die == 12)
                    {
                        byte[] readBytes = new byte[5];
                        int x = 0;
                        while (x < 4)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        long bigVal = ((long)readBytes[0] * 4294967296) + ((long)readBytes[1] * 16777216) + ((long)readBytes[2] * 65536) + ((long)readBytes[3] * 256) + (long)(readByte);
                        if (bigVal <= 743008370688)
                        {
                            _die12stack.Push((byte)((bigVal % 12) + 1));
                            _die12stack.Push((byte)(((bigVal / 12) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 144) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 1728) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 20736) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 248832) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 2985984) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 35831808) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 429981696) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 5159780352) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 61917364224) % 12 + 1)));
                        }
                        else if (bigVal <= 1052595191808)
                        {
                            bigVal -= 743008370688;
                            _die12stack.Push((byte)((bigVal % 12) + 1));
                            _die12stack.Push((byte)(((bigVal / 12) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 144) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 1728) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 20736) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 248832) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 2985984) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 35831808) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 429981696) % 12 + 1)));
                            _die12stack.Push((byte)(((bigVal / 5159780352) % 12 + 1)));
                        }
                    }
                    if (die == 20)
                    {
                        byte[] readBytes = new byte[5];
                        int x = 0;
                        while (x < 5)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        long bigVal = ((long)readByte * 1099511627776) + ((long)readBytes[0] * 4294967296) + ((long)readBytes[1] * 16777216) + ((long)readBytes[2] * 65536) + ((long)readBytes[3] * 256) + (long)(readBytes[4]);
                        if (bigVal <= 204800000000000)
                        {
                            _die20stack.Push((byte)((bigVal % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 20) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 400) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 8000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 160000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 3200000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 64000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 1280000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 25600000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 512000000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 10240000000000) % 20) + 1));
                        }
                        else if (bigVal <= 276480000000000)
                        {
                            bigVal = bigVal - 204800000000000;
                            _die20stack.Push((byte)((bigVal % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 20) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 400) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 8000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 160000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 3200000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 64000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 1280000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 25600000000) % 20) + 1));
                            _die20stack.Push((byte)(((bigVal / 512000000000) % 20) + 1));
                        }
                    }
                    if (die == 100)
                    {
                        byte[] readBytes = new byte[4];
                        int x = 0;
                        while (x < 4)
                        {
                            success = HardwareRNGinterface._randomBytes.TryPop(out readBytes[x]);
                            if (success) x++;
                        }
                        long bigVal = ((long)readBytes[0] * 4294967296) + ((long)readBytes[1] * 16777216) + ((long)readBytes[2] * 65536) + ((long)readBytes[3] * 256) + (long)(readByte);
                        if (bigVal <= 1000000000000)
                        {
                            _die100stack.Push((byte)((bigVal % 100) + 1));
                            _die100stack.Push((byte)((bigVal / 100) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 10000) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 1000000) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 100000000) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 10000000000) % 100 + 1));
                        }
                        else if (bigVal <= 1090000000000)
                        {
                            bigVal -= 1000000000000;
                            _die100stack.Push((byte)((bigVal % 100) + 1));
                            _die100stack.Push((byte)((bigVal / 100) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 10000) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 1000000) % 100 + 1));
                            _die100stack.Push((byte)((bigVal / 100000000) % 100 + 1));
                        }
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

        static public int RollDice(int die, int count)
        {
            Random rnd = new Random();
            byte dieres = 0;
            int dieValue = 0;
            int totalroll = 0;
            bool success = false;
            bool coinres = false;
            int i = 0;
            while (i < count)
            {
                switch (die)
                {
                    case 2: success = _coinstack.TryPop(out coinres); if (coinres) dieValue = 2; else dieValue = 1; break;
                    case 4: success = _die4stack.TryPop(out dieres); dieValue = dieres; break;
                    case 6: success = _die6stack.TryPop(out dieres); dieValue = dieres; break;
                    case 8: success = _die8stack.TryPop(out dieres); dieValue = dieres; break;
                    case 10: success = _die10stack.TryPop(out dieres); dieValue = dieres; break;
                    case 12: success = _die12stack.TryPop(out dieres); dieValue = dieres; break;
                    case 20: success = _die20stack.TryPop(out dieres); dieValue = dieres; break;
                    case 100: success = _die100stack.TryPop(out dieres); dieValue = dieres; break;
                    case 256: success = _die256stack.TryPop(out dieres); dieValue = dieres + 1; break;
                    default: success = true; dieValue = rnd.Next(1, die - 1); break; //at the moment use the built in random number generator if its not in one of the above types. Could replace with something to use the RNG in future.
                }
                if (success)
                {
                    totalroll += dieValue;
                    i++;
                }
                else
                {
                    FillUpDice(die, 100);
                }
            }
            return totalroll;
        }
    }
}
