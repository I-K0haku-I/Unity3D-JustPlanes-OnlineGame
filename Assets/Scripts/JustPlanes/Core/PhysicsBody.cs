using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System;

namespace JustPlanes.Core
{
    public class PhysicsBody
    {
        public Body body;

        private float lerpTimer = 0f;
        private bool shouldLerp = false;
        private float lerpTimeStep = 0.5f;
        private float lerpAmount = 0f;

        public PhysicsBody(World world, float posX, float posY, float boxWidth, float boxHeight)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(posX, posY);
            body = world.CreateBody(bodyDef);

            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(boxWidth, boxHeight);
            shapeDef.Density = 1f;
            shapeDef.Friction = 0.3f;
            body.CreateShape(shapeDef);
            body.SetMassFromShapes();
            // body.SetLinearVelocity(new Vec2(0f, 1f));
        }

        public void SetAngularVelocity(float amount)
        {
            body.WakeUp();
            body.SetAngularVelocity(amount);
        }

        public void SetVelocity(float amount)
        {
            body.WakeUp();
            body.SetLinearVelocity(GetDirection() * amount);
        }

        public Vec2 GetDirection()
        {
            return GetDirection(body.GetAngle());
        }

        public Vec2 GetDirection(float angle)
        {
            Vec2 vel;
            double radian = GetRadian(angle);
            vel.X = -(1f * (float)System.Math.Sin(radian));
            vel.Y = 1f * (float)System.Math.Cos(radian);
            vel.Normalize();
            return vel;
        }

        public float GetRadian(float angle)
        {
            return (float)(-angle * System.Math.PI / 180f);
        }

        public void SetWithLerp(Vec2 position, float rotation, Vec2 vel, float deltaTime)
        {
            // lerpTimer += deltaTime;
            // if (lerpTimer > 0.3f)
            // {
            //     lerpTimer -= 0.3f;

            // }
            // float t = lerpTimer * 0.8f;

            if (System.Math.Abs((position - body.GetPosition()).Length()) > 0.2f)
            {
                shouldLerp = true;
                lerpAmount = 0f;
            }

            if (shouldLerp)
            {
                lerpAmount += lerpTimeStep * deltaTime;
                body.SetXForm(JPUtils.DoLerp(body.GetPosition(), position, lerpAmount), JPUtils.DoLerp(body.GetAngle(), rotation, lerpAmount));
                body.SetLinearVelocity(JPUtils.DoLerp(body.GetLinearVelocity(), vel, lerpAmount));
                if (System.Math.Abs((position - body.GetPosition()).Length()) < 0.01f)
                    shouldLerp = false;
            }
        }

        public float Dot(Vec2 a, Vec2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }
}