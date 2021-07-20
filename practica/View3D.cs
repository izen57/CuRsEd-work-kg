using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace practica
{

    class A
    {
        public int x = 2;
    }
    class View3D
    {
        ZBuffer ZB;
        double zNear, zFar;

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

        public void Clear(Color c)
        {
            ZB.Clear(Graphicx.ColorToInt(c));
        }

        void F(A a)
        {
/*            a = new A();
*/            a.x = 5;
        }

        public void Display()
        {
            A a = new A();
            F(a);
            int y = a.x;
            Clear(Color.Green);
            ZB.Triangle(Graphicx.ColorToInt(Color.Red), 0, 0, 1, 200, 0, 1, 100, 175, 1);
        }
    }
}
