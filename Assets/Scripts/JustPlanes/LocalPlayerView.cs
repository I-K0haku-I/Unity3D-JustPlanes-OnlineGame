using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustPlanes {

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