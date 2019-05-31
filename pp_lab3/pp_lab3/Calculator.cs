using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pp_lab3
{
    public class Params
    {
        public double Left = 0;
        public double Right = 0;
        public double Step = 0;
        public ICriticalSection CS;

        public Params()
        {

        }
    }

    public class Calculator
    {
        public static int SpinCount = 1;
        private static double NumberPi = 0;
        private static int ThreadCount = 4;
        public static int timeout = 10000;
        public static int IterationCount = 200000000;
        private static ICriticalSection CS = new CriticalSection(); 

        static void CalculatePartOfPi(object o)
        {
            Params parameters = (Params)o;
            var current = parameters.Left;
            while (current < parameters.Right)
            {
                double SmallPiece = 4 / (1 + (current * current));
                SmallPiece *= parameters.Step;
                current += parameters.Step;

                parameters.CS.Enter();
                NumberPi += SmallPiece;
                parameters.CS.Leave();
            }
        }

        static void CalculatePartOfPiTryEnter(object o)
        {
            Params parameters = (Params)o;
            var current = parameters.Left;
            while (current < parameters.Right)
            {
                double SmallPiece = 4 / (1 + (current * current));
                SmallPiece *= parameters.Step;
                current += parameters.Step;

                while (!parameters.CS.TryEnter(timeout))
                {
                    //do nothing;
                }

                NumberPi += SmallPiece;
                parameters.CS.Leave();
            }
        }

        public static void Run()
        {
            NumberPi = 0;
            CS.SetSpinCount(SpinCount);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Thread> Threads = new List<Thread>();
            double Step = 1.0 / IterationCount;
            double Interval = 1.0 / ThreadCount;

            for (int i = 0; i < ThreadCount; i++)
            {
                Thread T = new Thread(CalculatePartOfPi);
                Params p = new Params();
                p.Left = Interval * i;
                p.Right = p.Left + Interval;
                if(i == ThreadCount - 1)
                {
                    p.Right = 1;
                }
                p.Step = Step;

                p.CS = CS;
                T.Start(p);
                Threads.Add(T);
            }

            for (int i = 0; i < Threads.Count; i++)
            {
                Threads[i].Join();
            }

            sw.Stop();
            Console.WriteLine("Pi with enter: " + NumberPi);
            Console.WriteLine("Time: " + sw.ElapsedMilliseconds.ToString());
        }

        public static void RunTryEnter()
        {
            NumberPi = 0;
            CS.SetSpinCount(SpinCount);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Thread> Threads = new List<Thread>();
            double Step = 1.0 / IterationCount;
            double Interval = 1.0 / ThreadCount;

            for (int i = 0; i < ThreadCount; i++)
            {
                Thread T = new Thread(CalculatePartOfPiTryEnter);
                Params p = new Params();
                p.Left = Interval * i;
                p.Right = p.Left + Interval;
                if (i == ThreadCount - 1)
                {
                    p.Right = 1;
                }
                p.Step = Step;
                p.CS = CS;
                T.Start(p);
                Threads.Add(T);
            }

            for (int i = 0; i < Threads.Count; i++)
            {
                Threads[i].Join();
            }

            sw.Stop();
            Console.WriteLine("Pi with try enter: " + NumberPi);
            Console.WriteLine("Time: " + sw.ElapsedMilliseconds.ToString());
        }
    }
}
