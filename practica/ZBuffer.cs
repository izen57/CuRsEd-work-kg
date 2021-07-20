using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class ZBuffer
    {
        public int width, height;

        int wh;
        double zNear, zFar;
        UInt64[] B; // буфер цвета
        double[] Z; // буфер глубины

        public ZBuffer(int w, int h, double z0, double z1)
        {
            width = w;
            height = h;
            // depth = 1024;
            zNear = z0;
            zFar = z1;
            wh = width * height;
            B = new UInt64[wh];
            Z = new double[wh];
        }

        public UInt64[] GetColorBuffer()
        {
            return B;
        }

        public void Clear(UInt64 color)
        {
            for (int i = 0; i < wh; ++i)
            {
                B[i] = color;
                Z[i] = zFar;
            }
        }

        public void RasterizeSegment(int Y, int X0, int X1, double z0, double z1, UInt64 color)
        {
            if (X1 < X0)
            {
                int temp = X1;
                X1 = X0;
                X0 = temp;

                double temp1 = z0;
                z0 = z1;
                z1 = temp1;
            }
            if (X1 < 0 || X0 >= width)
                return;

            if (X0 < 0)
                X0 = 0;
            if (X1 >= width)
                X1 = width - 1;

            double k = (z1 - z0) / (X1 - X0);
            double z = z0;

            for (int X = X0, i = width * Y + X0; X <= X1; ++X, z += k, i++)
                if (z < Z[i])
                {
                    Z[i] = z;
                    B[i] = color;
                }
        }

        public void Triangle(UInt64 color, double x0, double y0, double z0, double x1, double y1, double z1, double x2, double y2, double z2, int orientation = 1)
        {
            if (y1 < y0 && y1 < y2)
            {
                Triangle(color, x1, y1, z1, x0, y0, z0, x2, y2, z2, -orientation);
                return;
            }

            if (y2 < y0 && y2 < y1)
            {
                Triangle(color, x2, y2, z2, x0, y0, z0, x1, y1, z1);
                return;
            }

            if (y2 < y1)
            {
                Triangle(color, x0, y0, z0, x2, y2, z2, x1, y1, z1, -orientation);
                return;
            }

            int Y0 = (int) Math.Round(y0);
            int Y1 = (int) Math.Round(y1);
            int Y2 = (int) Math.Round(y2);

            if (Y2 < 0 || Y0 >= height)
                return;

            double k01 = (x1 - x0) / (y1 - y0);
            double k02 = (x2 - x0) / (y2 - y0);
            double k12 = (x2 - x1) / (y2 - y1);
            double a01 = x0 - k01 * y1;
            double a02 = x0 - k02 * y0;
            double a12 = x1 - k12 * y1;
            double c01 = (z1 - z0) / (y1 - y0);
            double c02 = (z2 - z0) / (y2 - y0);
            double c12 = (z2 - z1) / (y2 - y1);
            double b01 = z0 - c01 * y1;
            double b02 = z0 - c02 * y0;
            double b12 = z1 - c12 * y1;
            
            if (Y1 >= 0)
                for (int Y = Y0 > 0? Y0 : 0; Y < Y1; ++Y)
                {
                    double xleft = a02 + k02 * Y;
                    double xright = a01 + k01 * Y;
                    double zleft = b02 + c02 * Y;
                    double zright = b01 + c01 * Y;

                    int Xleft = (int) xleft;
                    int Xright = (int) xright;
                    RasterizeSegment(Y, Xleft, Xright, zleft, zright, color);
                }

            if (Y1 < height)
                for (int Y = Y1; Y <= Y2 && Y < height; ++Y)
                {
                    double xleft = a02 + k02 * Y;
                    double xright = a12 + k12 * Y;
                    double zleft = b02 + c02 * Y;
                    double zright = b12 + c12 * Y;

                    int Xleft = (int)xleft;
                    int Xright = (int)xright;
                    RasterizeSegment(Y, Xleft, Xright, zleft, zright, color);
                }
        }
    }
}
