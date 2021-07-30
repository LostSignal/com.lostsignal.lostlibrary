//-----------------------------------------------------------------------
// <copyright file="Character.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class Character : MonoBehaviour
    {
        private static readonly List<Character> characters;
        private static readonly ReadOnlyCollection<Character> charactersReadOnly;

        private Transform characterTransform;

        static Character()
        {
            characters = new List<Character>(50);
            charactersReadOnly = new ReadOnlyCollection<Character>(characters);
            Bootloader.OnReset += characters.Clear;
        }

        public static ReadOnlyCollection<Character> AllCharacters
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => charactersReadOnly;
        }

#pragma warning disable 0649
        [SerializeField] private bool isMainCharacter;
        [SerializeField] private int teamId;
#pragma warning restore 0649

        public Transform Transform
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.characterTransform;
        }

        public bool IsMainCharacter
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.isMainCharacter;
        }

        public int TeamId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.teamId;
        }

        private void Awake()
        {
            this.characterTransform = this.transform;
        }

        private void OnEnable()
        {
            characters.Add(this);
        }

        private void OnDisable()
        {
            characters.Remove(this);
        }
    }
}

#endif
