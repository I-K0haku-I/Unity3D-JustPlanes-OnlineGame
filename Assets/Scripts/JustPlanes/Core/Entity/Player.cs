using System;

namespace JustPlanes.Core
{
    public class Player : Entity
    {
        public Player(Guid uniqueId, string name = "Player", float healthPoint = 10) : base(uniqueId, name, healthPoint) { }

        public Player(Guid uniqueId, Transform2D transform, string name = "Player", float healthPoint = 10) : base(uniqueId, transform, name, healthPoint) { }


        [Obsolete("Use Entity.uniqueId instead.", false)]
        public string ConnID
        {
            get => ConnID != null ? ConnID : uniqueId.ToString();
            private set => ConnID = value;
        }
        [Obsolete("Use Entity.name instead.", false)]
        public string Name
        {
            get => name;
            set => name = value;
        }
        [Obsolete("Use Entity.transform instead.", false)]
        public int X
        {
            get => (int)transform2D.position.x;
            set => transform2D.position.x = value;
        }
        [Obsolete("Use Entity.transform instead.", false)]
        public int Y
        {
            get => (int)transform2D.position.y;
            set => transform2D.position.y = value;
        }

        [Obsolete("Use entity-style constructor instead.", false)]
        public Player(string name, int x, int y) : this(Guid.NewGuid(), new Transform2D(new Vec2(x, y)), name) { }
        [Obsolete("Use entity-style constructor instead.", false)]
        public Player(string connID, string name, int x, int y) : this(name, x, y)
        {
            ConnID = connID;
        }
    }
}
