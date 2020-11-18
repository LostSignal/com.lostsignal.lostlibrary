//-----------------------------------------------------------------------
// <copyright file="DissonanceManager.cs" company="Giant Cranium">
//     Copyright (c) Giant Cranium. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2019_4_OR_NEWER

namespace Lost.DissonanceIntegration
{
    using Lost.PlayFab;
    using UnityEngine;

    public class DissonanceManager : Manager<DissonanceManager>
    {
#pragma warning disable 0649
        [Header("Dependencies")]
        [SerializeField] private bool startDissonanceCommsOnInitialize = true;
        [SerializeField] private PlayFabManager playfabManager;

#if USING_DISSONANCE
        [Header("Dissonance")]
        [SerializeField] private Dissonance.DissonanceComms dissonanceComms;
#endif
#pragma warning restore 0649

#if USING_DISSONANCE
        public Dissonance.DissonanceComms DissonanceComms => this.dissonanceComms;
#endif

        public override void Initialize()
        {
#if USING_DISSONANCE
            this.StartCoroutine(Coroutine());

            System.Collections.IEnumerator Coroutine()
            {
                yield return this.WaitForDependencies(this.playfabManager);

                if (this.dissonanceComms == null)
                {
                    Debug.LogError("Tring to use the DissonanceManager without specifying the dissonanceComms object.  Dissonance will not work.");
                }
                else
                {
                    this.dissonanceComms.LocalPlayerName = this.playfabManager.User.PlayFabId;

                    if (this.startDissonanceCommsOnInitialize)
                    {
                        this.StartDissonanceComms();
                    }
                }

                this.SetInstance(this);
            }

#else
            Debug.LogError("Tring to use the DissonanceManager without USING_DISSONANCE define.  Add the Dissonance package to your project.");
            this.SetInstance(this);
#endif
        }

        public void StartDissonanceComms()
        {
#if USING_DISSONANCE
            this.dissonanceComms.gameObject.SetActive(true);
#else
            Debug.LogError("Tring to use the DissonanceManager without USING_DISSONANCE define.  Add the Dissonance package to your project.");
#endif
        }
    }
}

#endif
