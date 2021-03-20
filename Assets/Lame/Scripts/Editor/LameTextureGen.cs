using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lame
{
    public class LameTextureGen : IDisposable
    {
        private PreviewRenderUtility previewRender;
        public Mesh mesh;
        public int subMeshIndex = 0;
        public Vector2Int resolution = new Vector2Int(1024, 1024);
        public float lameScale = 10.0f;
        private RenderTexture previewRenderTexture;
        public RenderTexture PreviewRenderTexture => previewRenderTexture;

        public LameTextureGen(Mesh mesh = null, int subMeshIndex = 0, float lameScale = 10.0f, int width = 1024, int height = 1024)
        {
            this.mesh = mesh;
            this.subMeshIndex = subMeshIndex;
            this.lameScale = lameScale;
            this.resolution.x = width;
            this.resolution.y = height;
            if(previewRender == null)previewRender = new PreviewRenderUtility();
            ResetRenderTexture();
        }

        public void ResetParams(Mesh mesh, int subMeshIndex = 0, float lameScale = 10.0f, int width = 1024, int height = 1024)
        {
            this.mesh = mesh;
            this.subMeshIndex = subMeshIndex;
            this.lameScale = lameScale;
            resolution.x = width;
            resolution.y = height;
            ResetRenderTexture();
        }

        public void ResetRenderTexture()
        {
            previewRenderTexture = new RenderTexture((int) resolution.x, (int) resolution.y, 0, RenderTextureFormat.ARGB32, 0);
            previewRenderTexture.filterMode = FilterMode.Point;
            previewRenderTexture.antiAliasing = 1;
        } 

        public void Dispose()
        {
            Debug.Log("clean up");
            previewRender.Cleanup();
            previewRenderTexture.Release();   
            Object.DestroyImmediate(previewRenderTexture);
            previewRenderTexture = null;
        }

        public void RenderingTexture(bool gen2DFlag)
        {
            var rect = new Rect(0, 0, resolution.x, resolution.y);

            previewRender.BeginPreview(rect, GUIStyle.none);
            previewRender.camera.aspect = 1f;
            previewRender.camera.nearClipPlane = 0.001f;
            previewRender.camera.farClipPlane = 100;
            previewRender.camera.transform.position = new Vector3(0, 0, 10);
            previewRender.camera.transform.rotation = Quaternion.Euler(0, 180, 0);
            previewRender.camera.clearFlags = CameraClearFlags.SolidColor;
            previewRender.camera.orthographic = true;
            previewRender.camera.orthographicSize = 0.5f;
            previewRender.camera.allowMSAA = false;
            previewRender.camera.targetTexture = previewRenderTexture;

            var position = Vector3.zero;
            var scale = Vector3.one;
            Material mat = new Material(Shader.Find("Lame/TextureGen"));
            mat.SetFloat("_LameScale", lameScale);
            mat.SetInt("_Gen2DFlag", gen2DFlag ? 1 : 0);
            previewRender.DrawMesh(mesh, position, scale, Quaternion.Euler(0, 0, 0), mat, subMeshIndex, null, null, false);
            previewRender.camera.Render();
            previewRender.EndPreview();
        }

        public void SaveTexture(string filePath)
        {
            Texture2D texture2D = new Texture2D(previewRenderTexture.width, previewRenderTexture.height,
                TextureFormat.RGB24, false, false);
            texture2D.filterMode = FilterMode.Point;

            RenderTexture tmp = RenderTexture.active;
            RenderTexture.active = previewRenderTexture;
            texture2D.ReadPixels(new Rect(0, 0, previewRenderTexture.width, previewRenderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = tmp;

            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                TextureImporterSettings settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.filterMode = FilterMode.Point;
                settings.textureType = TextureImporterType.SingleChannel;
                settings.wrapMode = TextureWrapMode.Repeat;
                settings.singleChannelComponent = TextureImporterSingleChannelComponent.Red;
                settings.sRGBTexture = false;
                settings.alphaSource = TextureImporterAlphaSource.None;
                importer.SetTextureSettings(settings);
                importer.crunchedCompression = false;
                importer.maxTextureSize = previewRenderTexture.width;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.compressionQuality = 0;
                importer.mipmapEnabled = false;
                EditorUtility.SetDirty(importer);
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}