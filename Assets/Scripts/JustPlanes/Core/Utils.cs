using System;
using System.Drawing;
using Box2DX.Common;

namespace JustPlanes.Core
{
    public static class JPUtils
    {
        public static Vec2 DoLerpHermite(Vec2 a, Vec2 ma, Vec2 b, Vec2 mb, float t)
        {
            var ti = (t - 1);
            var t2 = t * t;
            var ti2 = ti * ti;
            var h00 = (1 + 2 * t) * ti2;
            var h10 = t * ti2;
            var h01 = t2 * (3 - 2 * t);
            var h11 = t2 * ti;
            return h00 * a + h10 * ma + h01 * b + h11 * mb;
        }

        public static Vec2 DoLerpCube(Vec2 a, Vec2 b, Vec2 c, Vec2 d, float amount)
        {
            Vec2 p0 = DoLerpQuad(a, b, c, amount);
            Vec2 p1 = DoLerpQuad(b, c, d, amount);
            return DoLerp(p0, p1, amount);
        }

        public static Vec2 DoLerpQuad(Vec2 a, Vec2 b, Vec2 c, float amount)
        {
            Vec2 p0 = DoLerp(a, b, amount);
            Vec2 p1 = DoLerp(b, c, amount);
            return DoLerp(p0, p1, amount);
        }

        public static Vec2 DoLerp(Vec2 first, Vec2 second, float amount)
        {
            return new Vec2(DoLerp(first.X, second.X, amount), DoLerp(first.Y, second.Y, amount));
        }

        public static float DoLerp(float first, float second, float amount)
        {
            return first * (1 - amount) + second * amount;
        }

        public static float GetRadian(float angle)
        {
            return (float)(-angle * System.Math.PI / 180f);
        }

        public static float Dot(Vec2 a, Vec2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }
}