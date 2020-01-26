
using System;
using System.Collections.Generic;
using System.Drawing;
using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;

namespace JustPlanes.Core
{
    public class PhysicsManager
    {

        public World world;
        private List<PhysicsBody> bodies = new List<PhysicsBody>();

        public PhysicsManager()
        {
            AABB worldAABB = new AABB();
            worldAABB.LowerBound.Set(-1000f, -1000f);
            worldAABB.UpperBound.Set(1000f, 1000f);

            Vec2 gravity = new Vec2(0f, 0f);
            bool doSleep = true;
            world = new World(worldAABB, gravity, doSleep);

            // BodyDef groundBodyDef = new BodyDef();
            // groundBodyDef.Position.Set(0f, -10f);
            // Body groundBody = world.CreateBody(groundBodyDef);

            // PolygonDef groundShapeDef = new PolygonDef();
            // groundShapeDef.SetAsBox(50f, 10f);
            // groundBody.CreateShape(groundShapeDef);

        }

        public void Update(float deltaTime)
        {
            // foreach (var body in bodies)
            //     body.Update();
            world.Step(deltaTime, 8, 2);
        }

        public PhysicsBody CreateBody(float posX, float posY, float boxWidth, float boxHeight)
        {
            var body = new PhysicsBody(world, posX, posY, boxWidth, boxHeight);
            bodies.Add(body);
            return body;
        }
    }
}