using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class Matrix4D
    {
        /*
         Ссылочные типы содержат только ссылку на соответствующие данные,
         а значит поле readonly ссылочного типа будет всегда ссылаться на один объект.
         Но сам этот объект не является неизменяемым.
         Модификатор readonly запрещает замену поля другим экземпляром ссылочного типа.
         Но этот модификатор не препятствует изменению данных экземпляра,
         на которое ссылается поле только для чтения, в том числе через это поле.
         */
        readonly double[,] m = new double[4, 4];

        public Matrix4D() { }

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
            A[0, 0] = 1;
            A[1, 1] = 1;
            A[2, 2] = 1;
            A[3, 3] = 1;

            return A;
        }

        public static Matrix4D Diagonal(double a0, double a1, double a2, double a3)
        {
            var A = new Matrix4D();
            A[0, 0] = a0;
            A[1, 1] = a1;
            A[2, 2] = a2;
            A[3, 3] = a3;

            return A;
        }

        public double this[int i, int j]
        {
            get
            {
                return m[i, j];
            }
            set
            {
                m[i, j] = value;
            }
        }

        public static Matrix4D operator * (Matrix4D A, Matrix4D B)
        {
            Matrix4D C = new Matrix4D();
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                {
                    double sum = 0;
                    for (int k = 0; k < 4; ++k)
                        sum += A[i, k] * B[k, j];

                    C[i, j] = sum;
                }

            return C;
        }

        public static Vector4D operator * (Matrix4D A, Vector4D B)
        {
            // как лучше инициализировать поля: напрямую или через конструктор?
            //Vector4D C = new Vector4D
            //{
            //    X = A[0, 0] * B.X + A[0, 1] * B.Y + A[0, 2] * B.Z + A[0, 3] * B.W,
            //    Y = A[1, 0] * B.X + A[1, 1] * B.Y + A[1, 2] * B.Z + A[1, 3] * B.W,
            //    Z = A[2, 0] * B.X + A[2, 1] * B.Y + A[2, 2] * B.Z + A[2, 3] * B.W,
            //    W = A[3, 0] * B.X + A[3, 1] * B.Y + A[3, 2] * B.Z + A[3, 3] * B.W
            //};
            Vector4D C = new Vector4D
            (
                A[0, 0] * B.X + A[0, 1] * B.Y + A[0, 2] * B.Z + A[0, 3] * B.W,
                A[1, 0] * B.X + A[1, 1] * B.Y + A[1, 2] * B.Z + A[1, 3] * B.W,
                A[2, 0] * B.X + A[2, 1] * B.Y + A[2, 2] * B.Z + A[2, 3] * B.W,
                A[3, 0] * B.X + A[3, 1] * B.Y + A[3, 2] * B.Z + A[3, 3] * B.W
            );

            return C;
        }

        public static Matrix4D Translation(double x, double y, double z)
        {
            var a = Matrix4D.Identity();
            a[0, 3] = x;
            a[1, 3] = y;
            a[2, 3] = z;

            return a;
        }

        public static Matrix4D LookAt(Vector4D eye, Vector4D at, Vector4D up)
        {
            Vector4D zaxis = (at - eye).Normalized3D();
            Vector4D xaxis = zaxis.Cross3D(up);
            Vector4D yaxis = xaxis.Cross3D(zaxis);

            // negate(zaxis);
            zaxis = zaxis * (-1);

            return new Matrix4D
            (
                xaxis.X, xaxis.Y, xaxis.Z, -xaxis.Dot3D(eye),
                yaxis.X, yaxis.Y, yaxis.Z, -yaxis.Dot3D(eye),
                zaxis.X, zaxis.Y, zaxis.Z, -zaxis.Dot3D(eye),
                0, 0, 0, 1
            );
        }
    }
}
