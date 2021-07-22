using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace practica
{
    class View3D
    {
        ZBuffer ZB;
        readonly double zNear;
        readonly double zFar;
        Matrix4D P, M, T; // матрицы преобразования (объектив), модельная матрица (располжение наблюдателя), матриц

        public View3D()
        {
            zNear = 0.01;
            zFar = 1000;
        }

        public UInt64[] ColorBuffer()
        {
            return ZB.GetColorBuffer();
        }

        public void SetViewPort(int w, int h)
        {
            ZB = new ZBuffer(w, h, zNear, zFar);
        }

        void Clear(Color c)
        {
            ZB.Clear(Graphicx.ColorToInt(c));
        }

        Matrix4D Perspective(double angle, double aspectRatio, double zNear, double zFar)
        {
            var A = Matrix4D.Diagonal(1 / (aspectRatio * Math.Tan(angle / 2)),
                1 / Math.Tan(angle / 2),
                -(zFar + zNear) / (zFar - zNear),
                0);

            A[2, 3] = -(2 * zFar * zNear) / (zFar - zNear);
            A[3, 2] = -1;
            return A;
        }

        void SetUpCamera()
        {
            T = Matrix4D.Identity();
            P = Perspective(90, 1.0 * ZB.height / ZB.width, ZB.ZNear, ZB.ZFar);
            T = P * T;
            M = Matrix4D.LookAt(new Vector4D(0, -10, 8), Vector4D.Zero(), new Vector4D(0, 0, 1));
            T = M * T;
        }

        void Draw()
        {
            ZB.Triangle(Graphicx.ColorToInt(Color.Red), 0, 0, 1, 200, 0, 1, 100, 175, 1);
        }

        public void Display()
        {
            Clear(Color.Green);
            SetUpCamera();
            Draw();
        }
        #region Primitives
        private void Triangle(Color c, Vector4D p0, Vector4D p1, Vector4D p2)
        {
            var q0 = T * p0; // перегрузить return vector4d
            var q1 = T * p1;
            var q2 = T * p2;

            ZB.Triangle(Graphicx.ColorToInt(c), (int)q0.X, (int)q0.Y, (int)q0.Z, (int)q1.X, (int)q1.Y, (int)q1.Z, (int)q2.X, (int)q2.Y, (int)q2.Z);
        }
        #endregion
    }
}
