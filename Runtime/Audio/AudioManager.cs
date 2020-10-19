//-----------------------------------------------------------------------
// <copyright file="AudioManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

//// TODO [bgish]: Make a validation system.  Needs to find every AudioBlock and make sure
////               it has a valid ChannelId and AudioClips.
//// TODO [bgish]: Add Components PlayAudioOnButtonClick, PlayAudioOnDialogShowHide, etc...
//// TODO [bgish]: Make Validators that these above components piont to valid Audio Blocks
//// TODO [bgish]: Add UI Slider and UI Toggle component for editing Audio Channels
//// TODO [bgish]: Make Validators that these above components piont to valid Audio Blocks

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class AudioManager : Manager<AudioManager>
    {
#pragma warning disable 0649
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int initialAudioSourcePoolSize = 10;
        [SerializeField] private int audioSourcePoolGrowSize = 10;
        [SerializeField] private string[] channelIds = new string[] { "SFX", "Music" };
#pragma warning restore 0649

        private Dictionary<string, Channel> channelsHash;
        private List<AudioSource> audioSourcePool;
        private List<Channel> channelsList;

        public override void Initialize()
        {
            // Initializing the audio source pool
            this.audioSourcePool = new List<AudioSource>(this.initialAudioSourcePoolSize);

            for (int i = 0; i < this.initialAudioSourcePoolSize; i++)
            {
                this.AddToPool();
            }

            this.SetInstance(this);
        }

        public void Play(AudioBlock audioBlock)
        {
            this.Play(audioBlock, Vector3.zero, false);
        }

        public void Play(AudioBlock audioBlock, Vector3 position)
        {
            this.Play(audioBlock, position, true);
        }

        public void Play(AudioBlock audioBlock, Vector3 position, bool usePosition)
        {
            Channel channel = this.GetAudioChannel(audioBlock.ChannelId);

            if (channel == null)
            {
                Debug.LogError($"AudioBlock {audioBlock.name} Failed.  Unkonwn Channel Id {audioBlock.ChannelId}");
                return;
            }

            if (channel.IsMuted)
            {
                return;
            }

            AudioSource audioSource = this.GetAudioSource();
            audioSource.gameObject.SetActive(true);
            audioSource.transform.position = position;
            audioSource.spatialBlend = usePosition ? 1.0f : 0.0f;
            audioSource.clip = audioBlock.GetAudioClip();
            audioSource.pitch = audioBlock.GetPitch();
            audioSource.volume = audioBlock.GetVolume() * channel.Volume;
            audioSource.Play();
        }

        public void SaveAudioSettings()
        {
            foreach (var channel in this.channelsList)
            {
                channel.Save();
            }
        }

        private Channel GetAudioChannel(string channelId)
        {
            if (this.channelsHash == null || this.channelsList == null)
            {
                this.channelsHash = new Dictionary<string, Channel>();
                this.channelsList = new List<Channel>();

                foreach (var id in this.channelIds)
                {
                    var channel = new Channel(id);
                    this.channelsHash.Add(id, channel);
                    this.channelsList.Add(channel);
                }
            }

            if (this.channelsHash.TryGetValue(channelId, out Channel result))
            {
                return result;
            }

            return null;
        }

        //// TODO [bgish]: This can be highly optimized.  Picking up where we last left off, or having
        ////               multiiple lists for playing audio sources vs non-playing audio sources.  It
        ////               works for now though, so going with it.
        private AudioSource GetAudioSource()
        {
            int poolCount = this.audioSourcePool.Count;

            for (int i = 0; i < poolCount; i++)
            {
                if (this.audioSourcePool[i].isPlaying == false)
                {
                    return this.audioSourcePool[i];
                }
            }

            // If we got here, then we need to grow the pool
            this.audioSourcePool.Capacity = this.audioSourcePool.Count + this.audioSourcePoolGrowSize;

            for (int i = 0; i < this.audioSourcePoolGrowSize; i++)
            {
                this.AddToPool();
            }

            return this.audioSourcePool[poolCount];
        }

        private void AddToPool()
        {
            var poolItem = GameObject.Instantiate(this.audioSourcePrefab, this.transform);
            poolItem.gameObject.SetActive(false);
            poolItem.name = "Audio Source " + this.audioSourcePool.Count;
            this.audioSourcePool.Add(poolItem);
        }

        public class Channel
        {
            private float volume;
            private bool isMuted;
            private string volumeKey;
            private string isMutedKey;

            public Channel(string id)
            {
                this.Id = id;
                this.IsDirty = false;
                this.volumeKey = $"AudioChannel.{this.Id}.Volume";
                this.isMutedKey = $"AudioChannel.{this.Id}.IsMuted";
                this.volume = LostPlayerPrefs.GetInt(this.volumeKey, 100) / 100.0f;
                this.isMuted = LostPlayerPrefs.GetBool(this.isMutedKey, false);
            }

            public string Id { get; private set; }

            public bool IsDirty { get; private set; }

            public float Volume
            {
                get
                {
                    return this.volume;
                }

                set
                {
                    if (this.volume != value)
                    {
                        this.IsDirty = true;
                        this.volume = value;
                    }
                }
            }

            public bool IsMuted
            {
                get
                {
                    return this.isMuted;
                }

                set
                {
                    if (this.isMuted != value)
                    {
                        this.IsDirty = true;
                        this.isMuted = value;
                    }
                }
            }

            public void Save()
            {
                if (this.IsDirty)
                {
                    this.IsDirty = false;
                    LostPlayerPrefs.SetInt(this.volumeKey, (int)(this.Volume * 100));
                    LostPlayerPrefs.SetBool(this.isMutedKey, this.IsMuted);
                    LostPlayerPrefs.Save();
                }
            }
        }
    }
}
