using System.Linq;
using UnityEditor.Experimental;

public class LineEndingsFormatter : AssetsModifiedProcessor
{
    // QUESTION [bgish]: Are SLN files go through this?  Probably not, but should double check

    protected override void OnAssetsModified(string[] changedAssets, string[] addedAssets, string[] deletedAssets, AssetMoveInfo[] movedAssets)
    {
        // Shaders as well?
        foreach (var asset in changedAssets.Where(a => a.EndsWith(".cs")))
        {
            ProcessCSharpFile(asset);
        }
    }

    private void ProcessCSharpFile(string assetPath)
    {
    }
}
