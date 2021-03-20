using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(MeshRenderer))]
public class MeshRendererInspectorExtension : RendererInspectorExtension<MeshRenderer>
{
    MeshFilter meshFilter = null;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if multiple subMeshes draw GUI
        renderer = target as MeshRenderer;
        materials = renderer.sharedMaterials;
        meshFilter = renderer.GetComponent<MeshFilter>();
        targetMesh = meshFilter.sharedMesh;
        if (!targetMesh) return;
        subMeshCount = targetMesh.subMeshCount;
        if (1 < subMeshCount) DrawSubMeshGUI();
    }

    protected override void OnButtonClicked(int subMeshIndex)
    {
        Mesh asset = CopyOrUpdateMeshAsAsset(targetMesh);
        Mesh newMesh = asset;
        ReplaceSubMeshToLast(newMesh, subMeshIndex);

        AssetDatabase.SaveAssets();
        ReplaceMaterialToLast(materials, subMeshIndex);
        meshFilter.sharedMesh = newMesh;
        renderer.sharedMaterials = materials;
    }
}