//-----------------------------------------------------------------------
// <copyright file="GameObjectState.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

//// TODO [bgish]: Create MoveGameObject, ScaleGameObject, RotateGameObject, TintGraphic
//// TODO [bgish]: Create ToggleObjectState Component (takes two states and toggles them)

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using Lost.Networking;
    using UnityEngine;

    public class GameObjectState : NetworkBehaviour
    {
#pragma warning disable 0649
        [SerializeField] private State[] states;
#pragma warning restore 0649

        private State currentState;
        private int currentStateIndex;

        public override void Deserialize(NetworkReader reader)
        {
            int currentStateIndex = (int)reader.ReadPackedUInt32();
            this.SetState(this.states[currentStateIndex].Name);
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32((uint)this.currentStateIndex);
        }

        public void SetState(string stateName)
        {
            if (this.states?.Length > 0)
            {
                for (int i = 0; i < this.states.Length; i++)
                {
                    if (this.states[i].Name == stateName)
                    {
                        // Making sure we we're not trying to set the object to the state it's already in
                        if (this.currentState?.Name == this.states[i].Name)
                        {
                            return;
                        }

                        this.currentState?.Revert();
                        this.currentState = this.states[i];
                        this.currentStateIndex = i;
                        this.currentState.Apply();
                        return;
                    }
                }
            }

            Debug.LogError($"GameObject {this.gameObject.name} has unknown state {stateName}", this);
        }

#if UNITY_EDITOR
        public string GetEditorDisplayName(string stateName, int index)
        {
            for (int i = 0; i < this.states.Length; i++)
            {
                if (this.states[i].Name == stateName)
                {
                    var action = this.states[i].GetActionByIndex(index);
                    if (action != null)
                    {
                        return action.EditorDisplayName;
                    }
                }
            }

            return null;
        }

#endif

        protected override SendConfig GetInitialSendConfig()
        {
            return new SendConfig
            {
                SendReliable = true,
                NetworkUpdateType = NetworkUpdateType.Manual,
                UpdateFrequency = 0.25f,  // NOTE [bgish]: This should never be needed, but just in case
            };
        }

        [Serializable]
        public class State
        {
#pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeReference] private List<GameObjectStateAction> actions = new List<GameObjectStateAction>();
#pragma warning restore 0649

            public string Name => this.name;


            public void Apply()
            {
                for (int i = 0; i < this.actions.Count; i++)
                {
                    this.actions[i].Apply();
                }
            }

            public void Revert()
            {
                for (int i = 0; i < this.actions.Count; i++)
                {
                    this.actions[i].Revert();
                }
            }

            public GameObjectStateAction GetActionByIndex(int index)
            {
                return index >= 0 && index < this.actions.Count ? this.actions[index] : null;
            }
        }
    }
}

#endif
