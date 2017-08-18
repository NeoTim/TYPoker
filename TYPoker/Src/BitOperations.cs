using System;
namespace TYPoker.Src
{
    public class BitOperations
    {
        public BitOperations()
        {
        }


		// 0 or 1    --> 0
		// 0b10      --> 1
		public static UInt32 CalLowBit(Int32 b)
		{
			// C++
			// float f = (float)(b & -b);
			// return (*(uint32_t*)&f >> 23) - 0x7f;


			if (b == 0) return 0; // unused

			UInt32 count = 0;
			while ((b & 1) == 0)
			{
				b >>= 1;
				count++;
			}

			return count; // 0 based counting
		}
		

        // TODO: find a fast bit operation
		public static UInt32 CalHighBit(Int32 b)
		{
			if (b == 0) return 0; // unused

			UInt32 count = 0;
			UInt32 v = 1;

			while(b >= v)
			{
				v <<= 1;
				count++;
			}
			
			return count - 1; // 0 based counting
		}
		

		// Bit Hacks:  at most 14-bit values in b, perfect for single suit
		public static UInt32 CountBits(UInt32 b)
		{
			return (UInt32)((b * 0x200040008001ul & 0x111111111111111ul) % 0xf);
		}
    }
}
