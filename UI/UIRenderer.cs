﻿using System;
using UnityEngine;

namespace CustomBeatmaps.UI
{
    /**
     * An abstracted "layer" to GUI elements
     * so we aren't forced to think about GUI items as using MonoBehaviours.
     *
     * Yes, I have drank the React functional programming koolaid
     */
    public abstract class UIRenderer
    {
        public readonly GameObject GameObject;

        public UIRenderer(string objectName)
        {
            GameObject = new GameObject(objectName);
            var renderer = GameObject.AddComponent<SimpleRenderMono>();
            renderer.OnGUIAction = OnUnityGUI;
        }
        public UIRenderer() : this(null) {}

        protected abstract void OnUnityGUI();
        
        class SimpleRenderMono : MonoBehaviour
        {
            public Action OnGUIAction;

            private void OnGUI()
            {
                OnGUIAction?.Invoke();
            }
        }
    }
}
