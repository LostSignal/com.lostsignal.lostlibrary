#pragma warning disable

//-----------------------------------------------------------------------
// <copyright file="LazyDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    [Serializable]
#if UNITY
    public class LazyDialogLogic : LazyAsset<DialogLogic>
#else
    public class LazyDialogLogic : LazyAsset<object>
#endif
    {
    }
}
