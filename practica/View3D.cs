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
        double zNear, zFar;

        public View3D()
        {
            zNear = 0.01;
            zFar = 1000;
        }

        public UInt64[] ColorBuffer()
        {
            return ZB.ColorBuffer();
        }

        public void SetViewPort(int w, int h)
        {
            ZB = new ZBuffer(w, h, zNear, zFar);
        }

        public void Clear(Color c)
        {
            ZB.Clear(Graphicx.ColorToInt(c));
        }
    }
}
