using System;

namespace JustPlanes.Core
{
    public class Plane : Entity
    {
        public Plane(Guid uniqueId, string name = "Plane", float healthPoint = 10) : base(uniqueId, name, healthPoint) { }

        public Plane(Guid uniqueId, Transform2D transform, string name = "Plane", float rotation = 0, float healthPoint = 10) : base(uniqueId, transform, name, healthPoint) { }

        
        public void Shoot()
        {

        }
    }
}
