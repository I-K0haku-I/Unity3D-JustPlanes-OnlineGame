using System;

namespace JustPlanes.Core
{
    public class Balloon : Entity
    {
        public Balloon(Guid uniqueId, string name = "Balloon", float healthPoint = 10) : base(uniqueId, name, healthPoint) { }

        public Balloon(Guid uniqueId, Transform2D transform, string name = "Balloon", float healthPoint = 10) : base(uniqueId, transform, name, healthPoint) { }

    }
}

