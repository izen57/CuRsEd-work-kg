using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class Matrix4D
    {
        double[,] m = new double[4, 4];


        public Matrix4D() {}

        public static Matrix4D Zero()
        {
            return new Matrix4D();
        }

        public Matrix4D(double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            m[0, 0] = m00; m[0, 1] = m01; m[0, 2] = m02; m[0, 3] = m03;
            m[1, 0] = m10; m[1, 1] = m11; m[1, 2] = m12; m[1, 3] = m13;
            m[2, 0] = m20; m[2, 1] = m21; m[2, 2] = m22; m[2, 3] = m23;
            m[3, 0] = m30; m[3, 1] = m31; m[3, 2] = m32; m[3, 3] = m33;
        }

        public static Matrix4D Identity()
        {
            var A = new Matrix4D();
            A.m[0, 0] = 1;
            A.m[1, 1] = 1;
            A.m[2, 2] = 1;
            A.m[3, 3] = 1;

            return A;
        }

        public static Matrix4D operator * (Matrix4D A, Matrix4D B)
        {
            var C = new Matrix4D();
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                {
                    double s = 0;
                    for (int k = 0; k < 4; ++k)
                        s += A.m[i, k] * B.m[k, j];

                    C.m[i, j] = s;
                }

            return C;
        }

        public Matrix4D LookAt(Vector4D eye, Vector4D at, Vector4D up)
        {
            Vector4D zaxis = (at - eye).Normalized3D();
            Vector4D xaxis = zaxis.Cross3D(up);
            Vector4D yaxis = xaxis.Cross3D(zaxis);

            // negate(zaxis);
            zaxis = zaxis * (-1);

            return new Matrix4D(xaxis.x, xaxis.y, xaxis.z, -xaxis.Dot3D(eye),
                yaxis.x, yaxis.y, yaxis.z, -yaxis.Dot3D(eye),
                zaxis.x, zaxis.y, zaxis.z, -zaxis.Dot3D(eye),
                0, 0, 0, 1);
        }
    }
}
