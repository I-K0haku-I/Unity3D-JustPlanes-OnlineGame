using System;
using System.Drawing;

namespace JustPlanes.Core
{
    public static class PointUtil
    {
        public static float GetMagnitude(PointF p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static float GetRadians(PointF p)
        {
            return (float)Math.Atan2(p.Y, p.X);
        }

        public static float GetDegrees(PointF p)
        {
            return GetRadians(p) * 180 / (float)Math.PI;
        }

        public static PointF Angle(float degrees)
        {
            return AngleRadians(degrees * (float)Math.PI / 180);
        }

        public static PointF AngleRadians(float radians)
        {
            return new PointF((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        public static PointF Normalize(PointF p)
        {
            return Divide(p, GetMagnitude(p));
        }

        public static PointF Add(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static PointF Subtract(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        public static PointF Multiply(PointF a, float b)
        {
            return new PointF(a.X * b, a.Y * b);
        }

        public static PointF Divide(PointF a, float b)
        {
            return new PointF(a.X / b, a.Y / b);
        }

    }
}
