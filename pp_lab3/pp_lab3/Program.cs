using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pp_lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Wrong args");
                return;
            }
            Calculator.IterationCount = int.Parse(args[0]);
            Calculator.timeout = int.Parse(args[1]);
            Calculator.SpinCount = int.Parse(args[2]);
            Calculator.Run();
            Calculator.RunTryEnter();
            Console.Read();
        }
    }
}
