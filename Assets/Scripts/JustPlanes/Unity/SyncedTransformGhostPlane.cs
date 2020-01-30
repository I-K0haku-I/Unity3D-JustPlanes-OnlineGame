using System;
using JustPlanes.Core.Network;
using UnityEngine;


namespace JustPlanes.Unity
{
    [RequireComponent(typeof(TestPlane))]
    public class SyncedTransformGhostPlane : MonoBehaviour
    {
        [SerializeField]
        private bool isGhostEnable = false;
        private SyncedTransformGhostPlaneState state = SyncedTransformGhostPlaneState.Disabled;

        [SerializeField]
        private Sprite ghostSprite;
        [SerializeField]
        private Color ghostColor;
        private SyncedTransform2D syncedTransform;
        private GameObject ghost;
        private SpriteRenderer rend;

        private void Start()
        {
            var plane = GetComponent<TestPlane>();
            ghost = new GameObject(plane.gameObject.name + " - Ghost Plane");
            // ghost.hideFlags = HideFlags.HideAndDontSave;
            ghost.SetActive(false);
            syncedTransform = plane.plane.transform2D;
            ghost.transform.localScale = new Vector3(2f, 2f, 1f);
            rend = ghost.AddComponent<SpriteRenderer>();
            rend.sprite = ghostSprite;
        }

        private void Update()
        {
            if (state == SyncedTransformGhostPlaneState.Enabled)
            {
                rend.color = ghostColor;
                if (!isGhostEnable)
                    state = SyncedTransformGhostPlaneState.OnDisable;
            }
            else if (state == SyncedTransformGhostPlaneState.OnDisable)
            {
                syncedTransform.OnStateCalculated -= HandleStateCalculated;
                ghost.SetActive(false);
                state = SyncedTransformGhostPlaneState.Disabled;
            }
            else if (state == SyncedTransformGhostPlaneState.OnEnable)
            {
                ghost.SetActive(true);
                syncedTransform.OnStateCalculated += HandleStateCalculated;
                state = SyncedTransformGhostPlaneState.Enabled;
            }
            else
            {
                if (isGhostEnable)
                    state = SyncedTransformGhostPlaneState.OnEnable;
            }
        }

        private void HandleStateCalculated(Transform2DNetworkData data)
        {
            ghost.transform.position = new Vector3(data.Position.X, data.Position.Y);
            ghost.transform.rotation = Quaternion.AngleAxis(data.Rotation, Vector3.back);
        }

        private void OnApplicationQuit()
        {
            DestroyImmediate(ghost);
        }
    }

    public enum SyncedTransformGhostPlaneState
    {
        Enabled,
        Disabled,
        OnEnable,
        OnDisable
    }
}