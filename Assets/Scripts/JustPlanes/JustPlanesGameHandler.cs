
namespace JustPlanes
{

    public abstract class JustPlanesGameHandler
    {
        public abstract System.Collections.Generic.List<Plane> GetPlanes();

        public abstract UnityEngine.Tilemaps.Tilemap GetMap();

    }
}