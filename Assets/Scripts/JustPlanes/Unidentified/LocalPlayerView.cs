using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes.Unity
{

    public class LocalPlayerView : PlayerView
    {

        public static LocalPlayerView Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("LocalPlayer instance already exists!!");
                Destroy(this);
            }
        }
    }
}