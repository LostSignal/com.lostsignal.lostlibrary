//-----------------------------------------------------------------------
// <copyright file="LostCommsNetworkEditor.cs" company="Giant Cranium">
//     Copyright (c) Giant Cranium. All rights reserved.
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
