//-----------------------------------------------------------------------
// <copyright file="AudioBlock.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Lost/Audio/Audio Block")]
    public class AudioBlock : ScriptableObject
    {
#pragma warning disable 0649
        [SerializeField] private string channelId;
        [SerializeField] private AudioClip[] audioClips;
        [SerializeField] private float minPitch = 1.0f;
        [SerializeField] private float maxPitch = 1.0f;
        [SerializeField] private float minVolume = 1.0f;
        [SerializeField] private float maxVolume = 1.0f;
        [SerializeField] private PlayType playType;
#pragma warning restore 0649

        private int roundRobinIndex;

        // TODO [bgish]: Add RandomWithMemory which is random, but wont repeat till all have played
        public enum PlayType
        {
            Random,
            RoundRobin,
        }

        public string ChannelId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.channelId;
        }

        public void Play()
        {
            if (AudioManager.IsInitialized)
            {
                AudioManager.Instance.Play(this);
            }
            else
            {
                Debug.LogError($"Tried to play AudioBlock {this.name} before AudioManager was initialized.", this);
            }
        }

        public void Play(Vector3 position)
        {
            if (AudioManager.IsInitialized)
            {
                AudioManager.Instance.Play(this, position);
            }
            else
            {
                Debug.LogError($"Tried to play AudioBlock {this.name} before AudioManager was initialized.", this);
            }
        }

        public AudioClip GetAudioClip()
        {
            if (this.audioClips == null || this.audioClips.Length == 0)
            {
                Debug.LogError($"AudioBlock {this.name} has no AudioClip assigned.");
                return null;
            }
            else if (this.audioClips.Length == 1)
            {
                return this.audioClips[0];
            }
            else
            {
                if (this.playType == PlayType.Random)
                {
                    int randomIndex = Random.Range(0, this.audioClips.Length);
                    return this.audioClips[randomIndex];
                }
                else if (this.playType == PlayType.RoundRobin)
                {
                    AudioClip audioClip = this.audioClips[this.roundRobinIndex++];
                    this.roundRobinIndex = this.roundRobinIndex % this.audioClips.Length;
                    return audioClip;
                }
                else
                {
                    Debug.LogError($"AudioBlock {this.name} enountered unkonwn PlayType {this.playType}");
                    return null;
                }
            }
        }

        public float GetPitch()
        {
            return Random.Range(this.minPitch, this.maxPitch);
        }

        public float GetVolume()
        {
            return Random.Range(this.minVolume, this.maxVolume);
        }
    }
}

#endif
