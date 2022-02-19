using System;
using System.Drawing;

namespace practica
{
	class Graphicx
	{
		public static UInt64 ColorToInt(Color c)
		{
			UInt64 a = c.R;
			a <<= 16;
			a |= c.G;
			a <<= 16;
			a |= c.B;
			a <<= 16;
			a |= c.A;

			return a;
		}

		public static Color IntToColor(UInt64 a)
		{
			byte r = (byte) (a >> 48);
			byte g = (byte) (a >> 32);
			byte b = (byte) (a >> 16);
			byte alpha = (byte) a;

			return Color.FromArgb(alpha, r, g, b);
		}
	}
}
