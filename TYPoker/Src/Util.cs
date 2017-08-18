﻿using System;
namespace TYPoker.Src
{
    public class Util
    {
        /**
         *  Convert Int to Binary, example: 2 -> 0x10
         *  Set padding to display a fixed length
         *      example: pad = 3, display 010 instead of 10 
         */
        public static void BinaryPrint(int number, string msg="", int pad=14)
        {
            Console.Write(msg);
            Console.WriteLine(Convert.ToString(number, 2).PadLeft(pad, '0'));
        }

        public static void BinaryPrint64(Int64 number, int pad=64)
        {
            Console.WriteLine(Convert.ToString(number, 2).PadLeft(pad, '0'));
        }

        public static void DebugAssert(bool condition, string confirmation)
        {
            if(condition)
            {
                Console.WriteLine("Has " + confirmation);
            }
            else
            {
                Console.WriteLine("No " + confirmation);
            }
        }
    }
}
