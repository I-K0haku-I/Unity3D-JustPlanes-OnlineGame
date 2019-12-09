using UnityEngine;
using UnityEngine.Events;

namespace JustPlanes
{

    public sealed class FloatEvent : UnityEvent<float> { }

    public sealed class GameObjectEvent : UnityEvent<GameObject> { }

    public sealed class GameObjectIntEvent : UnityEvent<GameObject, int> { }

}

