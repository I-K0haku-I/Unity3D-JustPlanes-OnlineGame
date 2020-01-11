using System;

namespace JustPlanes.Core
{
    public abstract class Entity
    {
        /// <summary>
        /// Unique ID of entity.
        /// </summary>
        public readonly Guid uniqueId;

        /// <summary>
        /// Name of entity.
        /// </summary>
        public string name;
        /// <summary>
        /// Position of entity in 2D.
        /// </summary>
        public Transform2D transform2D;
        /// <summary>
        /// Health point of entity.
        /// </summary>
        public float healthPoint;


        /// <summary>
        /// Default Entity Constructor. (without transform specified)
        /// </summary>
        /// <param name="uniqueId">The unique entity ID</param>
        /// <param name="name">The name of entity</param>
        /// <param name="healthPoint">The health point of entity</param>
        public Entity(Guid uniqueId, string name = "Entity", float healthPoint = 10) : this(uniqueId, new Transform2D(), name, healthPoint) { }

        /// <summary>
        /// Default Entity Constructor.
        /// </summary>
        /// <param name="uniqueId">The unique entity ID</param>
        /// <param name="position">The position of entity</param>
        /// <param name="name">The name of entity</param>
        /// <param name="rotation">The rotation of entity</param>
        /// <param name="healthPoint">The health point of entity</param>
        public Entity(Guid uniqueId, Transform2D transform2D, string name = "Entity", float healthPoint = 10)
        {
            this.uniqueId = uniqueId;
            this.transform2D = transform2D;
            this.healthPoint = healthPoint;
        }

    }
}