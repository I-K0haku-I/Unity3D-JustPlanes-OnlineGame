using System;

namespace JustPlanes.Core
{
    public class Transform2D
    {
        public static readonly Transform2D Zero = new Transform2D();

        /// <summary>
        /// Position in Vec2
        /// </summary>
        public Vec2 position = null;
        /// <summary>
        /// Rotation in Degrees
        /// </summary>
        public float rotation = 0.0F;
        /// <summary>
        /// Rotation in Radians
        /// </summary>
        public float RadiansRotation
        {
            get => rotation * (float)Math.PI / 180;
            set => rotation = value * 180 / (float)Math.PI;
        }

        public Transform2D()
        {
            this.position = Vec2.Zero;
            this.rotation = 0;
        }

        public Transform2D(Vec2 position, float rotation = 0)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    public class Vec2
    {
        public static readonly Vec2 Zero = new Vec2(0, 0);

        public event Action<Vec2, Vec2> OnValueChange;

        public float x
        {
            get => x;
            set
            {
                Vec2 last = this;
                x = value;
                OnValueChange?.Invoke(last, this);
            }
        }
        public float y
        {
            get => y;
            set
            {
                Vec2 last = this;
                y = value;
                OnValueChange?.Invoke(last, this);
            }
        }

        public Vec2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }


        #region Vector Operators

        public static Vec2 operator +(Vec2 o1, Vec2 o2)
        {
            return new Vec2(o1.x + o2.x, o1.y + o2.y);
        }
        public static Vec2 operator -(Vec2 o1, Vec2 o2)
        {
            return new Vec2(o1.x - o2.x, o1.y - o2.y);
        }
        public static Vec2 operator *(Vec2 o1, float o2)
        {
            return new Vec2(o1.x * o2, o1.y * o2);
        }
        public static Vec2 operator /(Vec2 o1, float o2)
        {
            return new Vec2(o1.x / o2, o1.y / o2);
        }

        #endregion Vector Operators


        #region Object overriden methods

        public override string ToString()
        {
            return $"Vec2: {{{x}, {y}}}";
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y).GetHashCode();
        }

        #endregion Object overriden methods


        #region Vector Utils

        public float GetMagnitude()
        {
            return x * x + y * y;
        }

        public float GetRadians()
        {
            return (float)Math.Atan2(y, x);
        }

        public float GetDegrees()
        {
            return GetRadians() * 180 / (float)Math.PI;
        }

        public static Vec2 Angle(float degrees)
        {
            return AngleRadians(degrees * (float)Math.PI / 180);
        }

        public static Vec2 AngleRadians(float radians)
        {
            return new Vec2((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        public static Vec2 Normalize(Vec2 vec)
        {
            return vec / vec.GetMagnitude();
        }

        #endregion Vector Utils

    }
}
