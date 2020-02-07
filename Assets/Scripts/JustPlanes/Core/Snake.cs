
namespace JustPlanes.Core
{
    public class Snake
    {
        public PhysicsBody body;
        public Network.SyncedTransform2D transform2D;
        private IGame game;

        public float Speed = 15F;

        public Snake(IGame game, float posX, float posY, int syncedTransformId)
        {
            this.game = game;
            body = game.GetPhysicsManager().CreateBody(posX, posY, 5, 5);
            body.SetVelocity(Speed);
            body.SetAngularVelocity(120f);
            transform2D = new Network.SyncedTransform2D(syncedTransformId, game, body);
        }

        public void Update(float deltaTime)
        {
            body.SetVelocity(Speed);
            transform2D.Update(deltaTime);
        }
    }
}