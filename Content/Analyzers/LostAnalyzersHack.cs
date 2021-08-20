#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LostAnalyzersHack.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    ////
    //// NOTE [bgish]: By putting an empty class and assembly definition file next to our analyzers, it will make sure it doesn't apply these
    ////               analyzers to the entire project, but to just this fake assembly def.  This is good because we have our own tool for
    ////               hooking up analyzers only to specific projects.
    ////
    public class LostAnalyzersHack : MonoBehaviour
    {
    }
}
