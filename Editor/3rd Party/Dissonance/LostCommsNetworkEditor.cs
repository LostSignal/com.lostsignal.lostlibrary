//-----------------------------------------------------------------------
// <copyright file="LostCommsNetworkEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DISSONANCE

namespace Lost.DissonanceIntegration
{
    using Dissonance;
    using Dissonance.Editor;
    using UnityEditor;

    [CustomEditor(typeof(LostCommsNetwork))]
    public class LostCommsNetworkEditor
        : BaseDissonnanceCommsNetworkEditor<
            LostCommsNetwork,
            LostServer,
            LostClient,
            LostConn,
            Unit,
            Unit>
    {
    }
}

#endif
