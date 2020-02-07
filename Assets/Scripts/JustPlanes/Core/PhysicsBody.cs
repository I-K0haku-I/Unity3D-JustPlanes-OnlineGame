using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using System;
using System.Collections.Generic;
using JustPlanes.Core.Network;

namespace JustPlanes.Core
{
    public class PhysicsBody
    {
        private PhysicsManager physics;
        public Body body;

        public List<InputBodyState> inputBuffer = new List<InputBodyState>();

        private bool shouldLerp = false;
        private float lerpTimer;
        public float MaxLerpTime = 1f;
        public float LerpDistanceTrigger = 0.2f;
        private float inputBufferBackTime = 1f;

        public PhysicsBody(PhysicsManager physics, float posX, float posY, float boxWidth, float boxHeight)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.Position.Set(posX, posY);
            this.physics = physics;
            body = physics.CreateBody(bodyDef);

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

            DebugLog.Warning($"[PhysicsBody] to lerp distance: {(position - body.GetPosition()).Length()}");

            if (!shouldLerp && System.Math.Abs((position - body.GetPosition()).Length()) > LerpDistanceTrigger)
            {
                shouldLerp = true;
                lerpTimer = 0f;
            }

            if (shouldLerp)
            {
                // lerpTimer += deltaTime;
                // var t = lerpTimer / MaxLerpTime;
                // t = t * t;
                // t = t * t * (3f - 2f * t);
                // t = t * t * t * (t * (6f * t - 15f) + 10f);
                // t = 1f - (float)System.Math.Cos(t * System.Math.PI * 0.5f);
                // DebugLog.Warning($"[PhysicsBody] lerp amount: {t}");
                // DebugLog.Warning("HELLO " + t + ", lerptimer " + lerpTimer + ", lerptime: " + MaxLerpTime);
                var t = MaxLerpTime;
                body.SetXForm(JPUtils.DoLerp(body.GetPosition(), position, t), JPUtils.DoLerp(body.GetAngle(), rotation, t));
                body.SetLinearVelocity(JPUtils.DoLerp(body.GetLinearVelocity(), vel, t));
                if (System.Math.Abs((position - body.GetPosition()).Length()) < 0.01f)
                    shouldLerp = false;
            }
        }

        public void StoreInputState(float newVelToAdd, float newAngVelToSet, float timestamp)
        {
            inputBuffer.Add(new InputBodyState()
            {
                NewVelocity = newVelToAdd,
                NewAngularVelocity = newAngVelToSet,
                OldVelocity = body.GetLinearVelocity(),
                OldAngularVelocity = body.GetAngularVelocity(),
                Timestamp = timestamp
            });
            if (inputBuffer[0].Timestamp < timestamp - inputBufferBackTime)
                inputBuffer.RemoveAt(0);
        }

        public float Dot(Vec2 a, Vec2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public void ReCalculate(Transform2DNetworkData state, float currTime, float startTime)
        {
            body.SetXForm(state.Position, state.Rotation);

            var i = inputBuffer.Count;
            while (i >= 0)
            {
                if (startTime > inputBuffer[i].Timestamp)
                {
                    i++;
                    break;
                }
                i--;
            }

            //  no input
            if (i > inputBuffer.Count)
                return;


            InputBodyState input;
            InputBodyState nextInput;
            while (i + 1 <= inputBuffer.Count)
            {
                input = inputBuffer[i];
                nextInput = inputBuffer[i + 1];
                SetVelocity(input.NewVelocity);
                SetAngularVelocity(input.NewAngularVelocity);
                physics.Update(nextInput.Timestamp - input.Timestamp);
            }
            input = inputBuffer[i];
            nextInput = inputBuffer[i + 1];
            SetVelocity(input.NewVelocity);
            SetAngularVelocity(input.NewAngularVelocity);

            // body.SetLinearVelocity(state.Velocity);
            // body.SetAngularVelocity(state.)
            // body.GetWorld()
        }
    }

    public class InputBodyState
    {
        public float NewVelocity;
        public float NewAngularVelocity;
        public float Timestamp;
        internal float OldAngularVelocity;
        internal Vec2 OldVelocity;
    }
}