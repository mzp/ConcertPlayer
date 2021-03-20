using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Lame
{
    public class LameTextureGenWindow : EditorWindow
    {
        private static Mesh mesh;
        private static int subMeshIndex = 0;
        private static int resolution = 1024;
        private static float[] lameScale = {3.0f, 50.0f};
        private static string filePath = "Assets/LameTexture.png";

        private static LameTextureGen gen2D;
        private static LameTextureGen gen3D;

        private static LameTextureGenWindow window;
        private int fixedheight = 20;

        private static int resIndex = 3;
        private static string[] resolutions =
        {
            "4096",
            "2048",
            "1024",
            "512",
            "256",
            "128"
        };

        [MenuItem("Gen/Lame/TextureGen")]
        public static void Init()
        {
            window = EditorWindow.GetWindow<LameTextureGenWindow>("LameTextureGenWindow");
            gen2D = new LameTextureGen(mesh, subMeshIndex, lameScale[0], resolution, resolution);
            gen3D = new LameTextureGen(mesh, subMeshIndex, lameScale[1], resolution, resolution);
        }

        private void OnDestroy()
        {
            gen2D.Dispose();
            gen3D.Dispose();
        }

        public void OnGUI()
        {
            mesh = ObjectField("Mesh", mesh, true);
            subMeshIndex = IntField("Submesh Index", subMeshIndex);

            resIndex = EditorGUILayout.Popup("Resolution", resIndex, resolutions);
            resolution = Int32.Parse(resolutions[resIndex]);
            GUILayout.Space(20);
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int i = 0; i < 2; i++)
                {
                    GUILayout.Space(20);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        lameScale[i] = FloatField("Lame Scale", lameScale[i]);
                        filePath = StringField("File Path", filePath);
                    }
                }
            }
            
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (mesh != null)
                {
                    gen2D.ResetParams(mesh, subMeshIndex, lameScale[0], resolution, resolution);
                    gen2D.RenderingTexture(true);
                    gen3D.ResetParams(mesh, subMeshIndex, lameScale[1], resolution, resolution);
                    gen3D.RenderingTexture(false);
                    int w = (int) window.position.width;
                    
                    var style = new GUIStyle();
                    float halfSize = w / 2.0f;
                    float previewSize = halfSize * 0.8f;
                    //previewSizeとresolutionの比較でpreの方は大きければ、画像解像度修正をしたい。
                    style.fixedWidth    = (int) previewSize;
                    style.fixedHeight   = (int) previewSize;
                    style.margin.top    = (int) (halfSize * 0.1);
                    style.margin.bottom = (int) (halfSize * 0.1);
                    style.margin.left   = (int) (halfSize * 0.1);
                    style.margin.right  = (int) (halfSize * 0.1);
                    
                    GUILayout.Box(gen2D.PreviewRenderTexture, style);
                    GUILayout.Space((int)halfSize * 0.1f);
                    GUILayout.Box(gen3D.PreviewRenderTexture, style);
                }
            }

            if (mesh != null)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Texture(2D)"))
                    {
                        gen2D?.SaveTexture(filePath);
                    }

                    if (GUILayout.Button("Generate Texture(3D)"))
                    {
                        gen3D?.SaveTexture(filePath);
                    }
                }
            }
        }

        private T ObjectField<T>(string label, T obj, bool allowSceneObject = false) where T : Object
        {
            return (T) EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObject,
                GUILayout.Height(fixedheight));
        }

        private int IntField(string label, int integer)
        {
            return EditorGUILayout.IntField(label, integer, GUILayout.Height(fixedheight));
        }

        private float FloatField(string label, float floatValue)
        {
            return EditorGUILayout.FloatField(label, floatValue, GUILayout.Height(fixedheight));
        }

        private string StringField(string label, string str)
        {
            return EditorGUILayout.TextField(label, str, GUILayout.Height(fixedheight));
        }

        private Vector2Int Vec2IntField(string label, Vector2Int vec)
        {
            return EditorGUILayout.Vector2IntField(label, vec, GUILayout.Height(fixedheight * 2));
        }
    }
}