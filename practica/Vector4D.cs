using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class Vector4D
    {
        public double x, y, z, w;

        public Vector4D() {}

        public Vector4D(double x, double y, double z, double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0;
        }
        public static Vector4D Point(double x, double y, double z)
        {
            return new Vector4D(x, y, z, 1);
        }

        public static Vector4D Zero()
        {
            return new Vector4D();
        }

        public static Vector4D operator  + (Vector4D A, Vector4D B)
        {
            return new Vector4D(A.x + B.x, A.y + B.y, A.z + B.z, A.w + B.w);
        }

        public static Vector4D operator  - (Vector4D A, Vector4D B)
        {
            return new Vector4D(A.x - B.x, A.y - B.y, A.z - B.z, A.w - B.w);
        }

        public static Vector4D operator / (Vector4D A, double k)
        {
            return new Vector4D(A.x / k, A.y / k, A.z / k, A.w / k);
        }

        public static Vector4D operator * (Vector4D A, double k)
        {
            return new Vector4D(A.x * k, A.y * k, A.z * k, A.w * k);
        }

        public static Vector4D operator * (double k, Vector4D A)
        {
            return new Vector4D(A.x * k, A.y * k, A.z * k, A.w * k);
        }

        public double Lenght3D()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        public Vector4D Normalized3D()
        {
            return this / Lenght3D();
        }

        public double Dot3D(Vector4D A) // скалярное произв.
        {
            return A.x * x + A.y * y + A.z * z;
        }

        public Vector4D Cross3D(Vector4D A) // векторное произв.
        {
            return new Vector4D(y * A.z - z * A.y, z * A.x - x * A.z, x * A.y - y * A.x);
        }
    }
}
