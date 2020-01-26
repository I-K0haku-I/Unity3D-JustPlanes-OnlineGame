using System;
using System.Drawing;
using Box2DX.Common;

namespace JustPlanes.Core
{
    public static class JPUtils
    {
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
    }
}