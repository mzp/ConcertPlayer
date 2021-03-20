using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

public abstract class RendererInspectorExtension<T> : Editor where T:Renderer
{
    protected int subMeshCount;
    protected T renderer = null;
    protected Mesh targetMesh = null;
    protected Material[] materials = null;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    protected virtual void DrawSubMeshGUI()
    {
        GUILayout.Space(20);
        GUILayout.Label("SubMesh Order", GUIStyle.none);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.BeginVertical("Box");

        for (int i = 0; i < (int)Mathf.Min(subMeshCount, materials.Length); ++i)
        {
            var material = materials[i];

            GUILayout.BeginHorizontal();
            string materialName = "mat";
            if(material!= null)materialName = material.name;
            GUILayout.Label(i + " : " + materialName, GUIStyle.none);
            var buttonLayoutOptions = new GUILayoutOption[] { GUILayout.Width(150) };
            if (GUILayout.Button("Move to Last", buttonLayoutOptions))
            {
                OnButtonClicked(i);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
        }
        GUILayout.Space(3);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    protected abstract void OnButtonClicked(int subMeshIndex);

    protected Mesh CopyOrUpdateMeshAsAsset(Mesh originalMesh) {
        const string fileSufix = "_SubMeshReplaced";
        const string folderName = "SubMeshReplaced";

        string path = AssetDatabase.GetAssetPath(originalMesh);
        FileInfo originalFile = new FileInfo(path);
        DirectoryInfo directory = originalFile.Directory;
        if (directory.Name != folderName)
        {
            //固定のフォルダ下にファイルを複製

            string parentfolder = "Assets/" + Regex.Split(directory.FullName, "Assets")[1];
            if (parentfolder == "Assets/") parentfolder = "Assets";
            string newFolderFullPath = directory.FullName + "/" + folderName;
            string newFolderAssetsPath = "Assets/" + Regex.Split(newFolderFullPath, "Assets")[1] + "/";//Assets/~
            if (!Directory.Exists(newFolderFullPath))
            {
                AssetDatabase.CreateFolder(parentfolder, folderName);
            }
            string fileName = originalFile.Name.Split('.')[0];
            string extension = ".asset";
            string newMeshDataAssetPath = newFolderAssetsPath + fileName + fileSufix + extension;
            AssetDatabase.CreateAsset(Instantiate(originalMesh), newMeshDataAssetPath);
            AssetDatabase.SaveAssets();
            path = newMeshDataAssetPath;
        }
        return AssetDatabase.LoadAssetAtPath<Mesh>(path);
    }

    protected virtual void ReplaceSubMeshToLast(Mesh mesh, int targetSubMeshIndex)
    {
        int subMeshCount = mesh.subMeshCount;
        int lastIndex = subMeshCount - 1;
        var targetTris = mesh.GetTriangles(targetSubMeshIndex);
        var lastTris = mesh.GetTriangles(lastIndex);
        mesh.SetTriangles(targetTris, lastIndex);
        mesh.SetTriangles(lastTris, targetSubMeshIndex);
    }

    protected virtual void ReplaceMaterialToLast(Material[] materials, int targetSubMeshIndex)
    {
        int lastIndex = materials.Length-1;
        var tmp = materials[targetSubMeshIndex];
        materials[targetSubMeshIndex] = materials[lastIndex];
        materials[lastIndex] = tmp;
    }
}
