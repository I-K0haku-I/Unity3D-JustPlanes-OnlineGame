
using System;
using System.Drawing;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

namespace JustPlanes.Core
{
    public class Physics
    {
        public float Velocity = 0f;
        public float AngularVelocity = 100f;

        public World world;
        public Body body;
        private int i;

        public Physics()
        {
            i = 0;
            AABB worldAABB = new AABB();
            worldAABB.LowerBound.Set(-100f, -100f);
            worldAABB.UpperBound.Set(100f, 100f);

            Vec2 gravity = new Vec2(0f, 0f);
            bool doSleep = true;
            world = new World(worldAABB, gravity, doSleep);

            BodyDef groundBodyDef = new BodyDef();
            groundBodyDef.Position.Set(0f, -10f);
            Body groundBody = world.CreateBody(groundBodyDef);

            PolygonDef groundShapeDef = new PolygonDef();
            groundShapeDef.SetAsBox(50f, 10f);
            groundBody.CreateShape(groundShapeDef);

            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(0f, 4f);
            body = world.CreateBody(bodyDef);

            PolygonDef shapeDef = new PolygonDef();
            shapeDef.SetAsBox(1f, 1f);
            shapeDef.Density = 1f;
            shapeDef.Friction = 0.3f;
            body.CreateShape(shapeDef);
            body.SetMassFromShapes();
            // body.SetLinearVelocity(new Vec2(0f, 1f));
        }

        public void Update(float deltaTime)
        {
            body.SetLinearVelocity(GetVelocity());
            body.SetAngularVelocity(GetAngularVelocity());
            world.Step(deltaTime, 8, 1);
            i++;

            Vec2 position = body.GetPosition();
            float angle = body.GetAngle();

            DebugLog.Warning($"Step: {i} - X: {position.X}, Y: {position.Y}, Angle: {angle}");
        }

        private float GetAngularVelocity()
        {
            return AngularVelocity;
        }

        private Vec2 GetVelocity()
        {
            Vec2 vel;
            double radian = -body.GetAngle() * System.Math.PI / 180f;
            vel.X = -(1f * (float)System.Math.Sin(radian));
            vel.Y = 1f * (float)System.Math.Cos(radian);
            vel.Normalize();
            return vel * Velocity;
        }
    }
}