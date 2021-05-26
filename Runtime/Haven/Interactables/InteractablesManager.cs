//-----------------------------------------------------------------------
// <copyright file="InteractablesManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace HavenXR
{
    using System.Collections.Generic;
    using Lost;
    using UnityEngine;

    public class InteractablesManager : Manager<InteractablesManager>, InputHandler
    {
        private Camera mainCamera;
        private int layer;

        public Camera CurrentCamera
        {
            get { return this.mainCamera; }
            set { this.mainCamera = value; }
        }

        public override void Initialize()
        {
            this.SetInstance(this);
        }

        private void Awake()
        {
            int layerNumber = LayerMask.NameToLayer(Interactable.LayerName);

            if (layerNumber == -1)
            {
                Debug.LogFormat("Trying to use Interactables system without the \"{0}\" layer defined!  This system will not work.", Interactable.LayerName);
                return;
            }

            this.mainCamera = Camera.main;
            this.layer = 1 << layerNumber;

            InputManager.OnInitialized += () =>
            {
                InputManager.Instance.AddHandler(this);
            };
        }

        private void OnInput(Lost.Input input)
        {
            if (input == null || input.InputState != InputState.Pressed)
            {
                return;
            }

            if (this.mainCamera == null)
            {
                this.mainCamera = Camera.main;
            }

            Ray ray = this.mainCamera.ScreenPointToRay(input.CurrentPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, this.layer))
            {
                Interactable interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();

                if (interactable != null && interactable.HasInput == false)
                {
                    interactable.SetInputData(input, hit.collider, this.mainCamera);
                }
                else if (interactable == null)
                {
                    Debug.LogErrorFormat(hit.collider, "GameObject {0} has a collider on the {1} layer, but not Interactable component!", hit.collider.gameObject.name, Interactable.LayerName);
                }
            }
        }

        void InputHandler.HandleInputs(List<Lost.Input> touches, Lost.Input mouse, Lost.Input pen)
        {
            this.OnInput(mouse);
            this.OnInput(pen);

            for (int i = 0; i < touches.Count; i++)
            {
                this.OnInput(touches[i]);
            }
        }
    }
}

#endif
