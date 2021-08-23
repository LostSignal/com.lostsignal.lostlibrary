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
    public class LazyDialogLogic : LazyAssetT<DialogLogic>
#else
    public class LazyDialogLogic : LazyAsset<object>
#endif
    {
    }
}
