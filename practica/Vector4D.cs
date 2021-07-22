using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class Vector4D
    {
        public readonly double X, Y, Z, W; // где применять свойства и где readonly
        // double[] arr = new double[4] { X, Y, Z, W }; // почему нельзя так

        public Vector4D() {}

        public Vector4D(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 0;
        }

        public static Vector4D Point(double x, double y, double z)
        {
            return new Vector4D(x, y, z, 1);
        }

        public static Vector4D Zero()
        {
            return new Vector4D();
        }

        public static Vector4D operator + (Vector4D A, Vector4D B)
        {
            return new Vector4D(A.X + B.X, A.Y + B.Y, A.Z + B.Z, A.W + B.W);
        }

        public static Vector4D operator - (Vector4D A, Vector4D B)
        {
            return new Vector4D(A.X - B.X, A.Y - B.Y, A.Z - B.Z, A.W - B.W);
        }

        public static Vector4D operator / (Vector4D A, double k)
        {
            return new Vector4D(A.X / k, A.Y / k, A.Z / k, A.W / k);
        }

        public static Vector4D operator * (Vector4D A, double k)
        {
            return new Vector4D(A.X * k, A.Y * k, A.Z * k, A.W * k);
        }

        public static Vector4D operator * (double k, Vector4D A)
        {
            return new Vector4D(A.X * k, A.Y * k, A.Z * k, A.W * k);
        }

        public double Lenght3D()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector4D Normalized3D()
        {
            return this / Lenght3D();
        }

        public double Dot3D(Vector4D A) // скалярное произв.
        {
            return A.X * X + A.Y * Y + A.Z * Z;
        }

        public Vector4D Cross3D(Vector4D A) // векторное произв.
        {
            return new Vector4D(Y * A.Z - Z * A.Y, Z * A.X - X * A.Z, X * A.Y - Y * A.X);
        }
    }
}
