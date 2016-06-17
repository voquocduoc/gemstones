using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    public enum Shalala { T1, T2, T3, T4, T5 };

    class Program
    {
        static void Main(string[] args)
        {
            Shalala temp = Shalala.T3;
            Shalala reference = temp;
            Shalala ref2 = temp;
            reference = Shalala.T5;
            System.Console.WriteLine("Temp = {0}, Reference = {1}\n", temp.ToString(), reference.ToString());
            System.Console.WriteLine("ref2 = " + ref2.ToString());
        }
    }
}
