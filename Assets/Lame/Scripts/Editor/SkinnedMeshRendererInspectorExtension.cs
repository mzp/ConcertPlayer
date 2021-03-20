using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshRendererInspectorExtension : RendererInspectorExtension<SkinnedMeshRenderer>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if multiple subMeshes draw GUI
        renderer = target as SkinnedMeshRenderer;
        materials = renderer.sharedMaterials;
        targetMesh = renderer.sharedMesh;
        if (!targetMesh) return;
        subMeshCount = targetMesh.subMeshCount;
        DrawSubMeshGUI();
    }

    protected override void OnButtonClicked(int subMeshIndex)
    {
        Mesh asset = CopyOrUpdateMeshAsAsset(targetMesh);
        Mesh newMesh = asset;
        ReplaceSubMeshToLast(newMesh, subMeshIndex);

        AssetDatabase.SaveAssets();
        ReplaceMaterialToLast(materials, subMeshIndex);
        renderer.sharedMesh = newMesh;
        renderer.sharedMaterials = materials;
    }
}