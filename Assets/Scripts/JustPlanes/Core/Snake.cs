
namespace JustPlanes.Core
{
    public class Snake
    {
        public PhysicsBody body;
        public Network.SyncedTransform2D transform2D;
        private IGame game;

        public float Speed = 1f;

        public Snake(IGame game, float posX, float posY, int syncedTransformId)
        {
            this.game = game;
            this.body = game.GetPhysicsManager().CreateBody(posX, posY, 5, 5);
            this.body.SetVelocity(Speed);
            this.body.SetAngularVelocity(120f);
            transform2D = new Network.SyncedTransform2D(syncedTransformId, game, body);
        }

        public void Update(float deltaTime)
        {
            transform2D.Update(deltaTime);
        }
    }
}