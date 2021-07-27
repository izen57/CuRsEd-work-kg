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
            zNear = 0.1;
            zFar = 100;
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

        Matrix4D Perspective(double angle0, double aspectRatio, double zNear, double zFar)
        {
            var angle = angle0 * Math.PI / 180;
            var A = Matrix4D.Diagonal
            (
                1 / (aspectRatio * Math.Tan(angle / 2)),
                1 / Math.Tan(angle / 2),
                -(zFar + zNear) / (zFar - zNear),
                0
            );

            A[2, 3] = -1;
            A[3, 2] = -(/*2 **/zFar * zNear) / (zFar - zNear);
            return A;
        }

        Matrix4D Orto(double scale, double aspectRatio)
        {
            return Matrix4D.Diagonal(scale, aspectRatio * scale, 1, 1);
        }

        void SetUpCamera()
        {
            //T = Matrix4D.Translation(-ZB.width / 2, -ZB.height / 2, 0);
            T = Matrix4D.Identity();
            //P = Perspective(90, 1.0 * ZB.height / ZB.width, ZB.ZNear, ZB.ZFar);
            P = Orto(100, 1.0 * ZB.height / ZB.width);
            T = T * P;
            M = Matrix4D.LookAt(new Vector4D(8, 8, 3), Vector4D.Zero(), new Vector4D(0, 0, 1));
            T = T * M;

            //var u = T * Vector4D.Point(0, 0, 0);
            //var v = GetScreenCoordinates(Vector4D.Point(3, 0, 0));
        }

        Vector4D GetScreenCoordinates(Vector4D a)
        {
            var b = T * a;
            double w = b.W;
            b /= w;

            return new Vector4D(b.X + ZB.width / 2, b.Y + ZB.height / 2, -b.Z);
        }

        void Draw()
        {
            Triangle(Color.Red, Vector4D.Point(-1, -1, 0), Vector4D.Point(1, -1, 0), Vector4D.Point(0, 2, 0));
            Line(Color.Blue, Vector4D.Point(-5, 0, 0.1), Vector4D.Point(5, 0, 0.1));
            Line(Color.Blue, Vector4D.Point(0, -5, 0.1), Vector4D.Point(0, 5, 0.1));
            Line(Color.Blue, Vector4D.Point(0, 0, -5), Vector4D.Point(0, 0, 5));
            //ZB.Triangle(Graphicx.ColorToInt(Color.Red), 0, 0, 1, 200, 0, 1, 100, 175, 1);
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
            var q0 = /*T **/GetScreenCoordinates(p0);
            var q1 = GetScreenCoordinates(p1);
            var q2 = GetScreenCoordinates(p2);

            ZB.Triangle(Graphicx.ColorToInt(c), (int)q0.X, (int)q0.Y, (int)q0.Z, (int)q1.X, (int)q1.Y, (int)q1.Z, (int)q2.X, (int)q2.Y, (int)q2.Z);
        }

        private void Line(Color c, Vector4D p0, Vector4D p1)
        {
            var q0 = /*T **/GetScreenCoordinates(p0); // перегрузить return vector4d
            var q1 = GetScreenCoordinates(p1);

            ZB.Line(Graphicx.ColorToInt(c), (int)q0.X, (int)q0.Y, (int)q0.Z, (int)q1.X, (int)q1.Y, (int)q1.Z);
        }
        #endregion
    }
}
