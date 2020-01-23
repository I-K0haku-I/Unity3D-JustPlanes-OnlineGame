using System;
using System.Drawing;

namespace JustPlanes.Core
{
    public static class JPUtils
    {
        public static PointF DoLerpCube(PointF a, PointF b, PointF c, PointF d, float amount)
        {
            PointF p0 = DoLerpQuad(a, b, c, amount);
            PointF p1 = DoLerpQuad(b, c, d, amount);
            return DoLerp(p0, p1, amount);
        }

        public static PointF DoLerpQuad(PointF a, PointF b, PointF c, float amount)
        {
            PointF p0 = DoLerp(a, b, amount);
            PointF p1 = DoLerp(b, c, amount);
            return DoLerp(p0, p1, amount);
        }

        public static PointF DoLerp(PointF first, PointF second, float amount)
        {
            return new PointF(DoLerp(first.X, second.X, amount), DoLerp(first.Y, second.Y, amount));
        }

        public static float DoLerp(float first, float second, float amount)
        {
            return first * (1 - amount) + second * amount;
        }
    }
}