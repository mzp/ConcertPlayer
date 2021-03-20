// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

using System;
using UnityEngine;
//using TargetAttributes = UnityEditor.BuildTargetDiscovery.TargetAttributes;
using UnityEditor;

internal class LameStandardShaderGUI : ShaderGUI
{
    private enum WorkflowMode
    {
        Specular,
        Metallic,
        Dielectric
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }

    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }

    public enum LameSpace
    {
        UV2D = 0,
        UV3D = 1,
        Local3D = 2,
        Texture = 3,
    }

    private static class Styles
    {
        public static GUIContent uvSetLabel = EditorGUIUtility.TrTextContent("UV Set");

        public static GUIContent albedoText = EditorGUIUtility.TrTextContent("Albedo", "Albedo (RGB) and Transparency (A)");
        public static GUIContent alphaCutoffText = EditorGUIUtility.TrTextContent("Alpha Cutoff", "Threshold for alpha cutoff");
        public static GUIContent specularMapText = EditorGUIUtility.TrTextContent("Specular", "Specular (RGB) and Smoothness (A)");
        public static GUIContent metallicMapText = EditorGUIUtility.TrTextContent("Metallic", "Metallic (R) and Smoothness (A)");
        public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness value");
        public static GUIContent smoothnessScaleText = EditorGUIUtility.TrTextContent("Smoothness", "Smoothness scale factor");
        public static GUIContent smoothnessMapChannelText = EditorGUIUtility.TrTextContent("Source", "Smoothness texture and channel");
        public static GUIContent highlightsText = EditorGUIUtility.TrTextContent("Specular Highlights", "Specular Highlights");
        public static GUIContent reflectionsText = EditorGUIUtility.TrTextContent("Reflections", "Glossy Reflections");
        public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
        public static GUIContent heightMapText = EditorGUIUtility.TrTextContent("Height Map", "Height Map (G)");
        public static GUIContent occlusionText = EditorGUIUtility.TrTextContent("Occlusion", "Occlusion (G)");
        public static GUIContent emissionText = EditorGUIUtility.TrTextContent("Color", "Emission (RGB)");
        public static GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Detail Mask", "Mask for Secondary Maps (A)");
        public static GUIContent detailAlbedoText = EditorGUIUtility.TrTextContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
        public static GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");

        public static string primaryMapsText = "Main Maps";
        public static string secondaryMapsText = "Secondary Maps";
        public static string forwardText = "Forward Rendering Options";
        public static string renderingMode = "Rendering Mode";
        public static string advancedText = "Advanced Options";
        public static string lameText = "Lame Params";
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
    }

    MaterialProperty blendMode = null;
    MaterialProperty albedoMap = null;
    MaterialProperty albedoColor = null;
    MaterialProperty alphaCutoff = null;
    MaterialProperty specularMap = null;
    MaterialProperty specularColor = null;
    MaterialProperty metallicMap = null;
    MaterialProperty metallic = null;
    MaterialProperty smoothness = null;
    MaterialProperty smoothnessScale = null;
    MaterialProperty smoothnessMapChannel = null;
    MaterialProperty highlights = null;
    MaterialProperty reflections = null;
    MaterialProperty bumpScale = null;
    MaterialProperty bumpMap = null;
    MaterialProperty occlusionStrength = null;
    MaterialProperty occlusionMap = null;
    MaterialProperty heigtMapScale = null;
    MaterialProperty heightMap = null;
    MaterialProperty emissionColorForRendering = null;
    MaterialProperty emissionMap = null;
    MaterialProperty detailMask = null;
    MaterialProperty detailAlbedoMap = null;
    MaterialProperty detailNormalMapScale = null;
    MaterialProperty detailNormalMap = null;
    MaterialProperty uvSetSecondary = null;
    
    //Lame Properties
    MaterialProperty lameSpace = null;
    MaterialProperty lameDirTex = null;
    MaterialProperty lameColorMap = null;
    MaterialProperty lameColor = null;
    MaterialProperty independLameRGB = null;
    MaterialProperty independLameAlpha = null;
    MaterialProperty randomColor = null;
    MaterialProperty randomColorBlend = null;
    MaterialProperty lameScale = null;
    MaterialProperty lameParallaxDepth = null;
    MaterialProperty lameAmount = null;
    MaterialProperty lameJitter = null;
    MaterialProperty lameStrengthMap = null;
    MaterialProperty lameLightAttenInfluence = null;
    MaterialProperty lameEmissive = null;
    MaterialProperty lameAnimate = null;
    MaterialProperty lameAniamtionSpeed = null;
    //VRC
    MaterialProperty minimalBrightness = null;
    MaterialProperty noLightLame = null;
    MaterialProperty fixedLightDirection = null;
    MaterialProperty fixedLightColor = null;
    MaterialProperty lightSpace = null;
    MaterialProperty noLightIndependLameRGB = null;

    MaterialEditor m_MaterialEditor;
    WorkflowMode m_WorkflowMode = WorkflowMode.Specular;

    bool m_FirstTimeApply = true;

    public void FindProperties(MaterialProperty[] props)
    {
        blendMode = FindProperty("_Mode", props);
        albedoMap = FindProperty("_MainTex", props);
        albedoColor = FindProperty("_Color", props);
        alphaCutoff = FindProperty("_Cutoff", props);
        specularMap = FindProperty("_SpecGlossMap", props, false);
        specularColor = FindProperty("_SpecColor", props, false);
        metallicMap = FindProperty("_MetallicGlossMap", props, false);
        metallic = FindProperty("_Metallic", props, false);
        if (specularMap != null && specularColor != null)
            m_WorkflowMode = WorkflowMode.Specular;
        else if (metallicMap != null && metallic != null)
            m_WorkflowMode = WorkflowMode.Metallic;
        else
            m_WorkflowMode = WorkflowMode.Dielectric;
        smoothness = FindProperty("_Glossiness", props);
        smoothnessScale = FindProperty("_GlossMapScale", props, false);
        smoothnessMapChannel = FindProperty("_SmoothnessTextureChannel", props, false);
        highlights = FindProperty("_SpecularHighlights", props, false);
        reflections = FindProperty("_GlossyReflections", props, false);
        bumpScale = FindProperty("_BumpScale", props);
        bumpMap = FindProperty("_BumpMap", props);
        heigtMapScale = FindProperty("_Parallax", props);
        heightMap = FindProperty("_ParallaxMap", props);
        occlusionStrength = FindProperty("_OcclusionStrength", props);
        occlusionMap = FindProperty("_OcclusionMap", props);
        emissionColorForRendering = FindProperty("_EmissionColor", props);
        emissionMap = FindProperty("_EmissionMap", props);
        detailMask = FindProperty("_DetailMask", props);
        detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
        detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
        detailNormalMap = FindProperty("_DetailNormalMap", props);
        uvSetSecondary = FindProperty("_UVSec", props);

        //
        minimalBrightness = FindProperty("_MinimalBrightness", props);
        //Lame Params
        lameSpace = FindProperty("_LameSpace", props);
        lameDirTex = FindProperty("_LameDirTex", props);
        lameColorMap = FindProperty("_LameColorMap", props);
        lameColor = FindProperty("_LameColor", props);
        independLameRGB = FindProperty("_IndependLameRGB", props);
        independLameAlpha = FindProperty("_IndependLameAlpha", props);
        randomColor = FindProperty("_RandomColor", props);
        randomColorBlend = FindProperty("_RandomColorBlend", props);
        lameScale = FindProperty("_LameScale", props);
        lameParallaxDepth = FindProperty("_LameParallaxDepth", props);
        lameAmount = FindProperty("_LameAmount", props);
        lameJitter = FindProperty("_LameJitter", props);
        lameStrengthMap = FindProperty("_LameStrengthMap", props);
        lameLightAttenInfluence = FindProperty("_LightAttenuationInfluence", props);
        lameEmissive = FindProperty("_LameEmissive", props);
        lameAnimate = FindProperty("_Animate", props);
        lameAniamtionSpeed = FindProperty("_AnimationSpeed", props);

        noLightLame = FindProperty("_NoLightLame", props);
        fixedLightDirection = FindProperty("_FixedLightDir", props);
        fixedLightColor = FindProperty("_FixedLightColor", props);
        lightSpace = FindProperty("_LightDirSpace", props);
        noLightIndependLameRGB = FindProperty("_NoLightIndependLameRGB", props);
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a standard shader.
        // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
        if (m_FirstTimeApply)
        {
            MaterialChanged(material, m_WorkflowMode);
            m_FirstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        // Detect any changes to the material
        EditorGUI.BeginChangeCheck();
        {
            BlendModePopup();

            // Primary properties
            GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
            DoAlbedoArea(material);
            DoSpecularMetallicArea();
            DoNormalArea();
            m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);
            m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
            m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
            DoEmissionArea(material);
            EditorGUI.BeginChangeCheck();
            m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);
            if (EditorGUI.EndChangeCheck())
                emissionMap.textureScaleAndOffset = albedoMap.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake

            EditorGUILayout.Space();

            // Secondary properties
            GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel);
            m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap);
            m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
            m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
            m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);

            // Third properties
            GUILayout.Label(Styles.forwardText, EditorStyles.boldLabel);
            if (highlights != null)
                m_MaterialEditor.ShaderProperty(highlights, Styles.highlightsText);
            if (reflections != null)
                m_MaterialEditor.ShaderProperty(reflections, Styles.reflectionsText);

            //Lame propertieds
            GUILayout.Label(Styles.lameText, EditorStyles.boldLabel);
            DoLameArea(material);
        }
        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in blendMode.targets)
                MaterialChanged((Material)obj, m_WorkflowMode);
        }

        EditorGUILayout.Space();

        // NB renderqueue editor is not shown on purpose: we want to override it based on blend mode
        GUILayout.Label(Styles.advancedText, EditorStyles.boldLabel);
        m_MaterialEditor.EnableInstancingField();
        m_MaterialEditor.DoubleSidedGIField();
    }

    void DoLameArea(Material material)
    {
        //lame space
        m_MaterialEditor.ShaderProperty(lameSpace, lameSpace.displayName);
        var space = (LameSpace) (int) lameSpace.floatValue;
        SetLameSpace(material, space);
        bool spaceIsTexture = space == LameSpace.Texture;
        if(spaceIsTexture) m_MaterialEditor.ShaderProperty(lameDirTex, lameDirTex.displayName);
        m_MaterialEditor.ShaderProperty(lameStrengthMap, lameStrengthMap.displayName);
        m_MaterialEditor.ShaderProperty(lameColorMap, lameColorMap.displayName);
        m_MaterialEditor.ShaderProperty(lameColor, lameColor.displayName);
        m_MaterialEditor.ShaderProperty(independLameRGB, independLameRGB.displayName);
        var mode = (BlendMode)blendMode.floatValue;
        bool isTransparent = mode == BlendMode.Transparent || mode == BlendMode.Fade; 
        if(isTransparent) m_MaterialEditor.ShaderProperty(independLameAlpha, independLameAlpha.displayName);
        m_MaterialEditor.ShaderProperty(randomColor, randomColor.displayName);
        m_MaterialEditor.ShaderProperty(randomColorBlend, randomColorBlend.displayName);
        if(!spaceIsTexture) m_MaterialEditor.ShaderProperty(lameScale, lameScale.displayName);
        m_MaterialEditor.ShaderProperty(lameParallaxDepth, lameParallaxDepth.displayName);
        m_MaterialEditor.ShaderProperty(lameAmount, lameAmount.displayName);
        m_MaterialEditor.ShaderProperty(lameJitter, lameJitter.displayName);
        
        m_MaterialEditor.ShaderProperty(lameLightAttenInfluence, lameLightAttenInfluence.displayName);
        m_MaterialEditor.ShaderProperty(lameEmissive, lameEmissive.displayName);
        m_MaterialEditor.ShaderProperty(lameAnimate, lameAnimate.displayName);
        m_MaterialEditor.ShaderProperty(lameAniamtionSpeed, lameAniamtionSpeed.displayName);
        
        m_MaterialEditor.ShaderProperty(noLightLame, noLightLame.displayName);
        m_MaterialEditor.ShaderProperty(fixedLightDirection, fixedLightDirection.displayName);
        m_MaterialEditor.ShaderProperty(fixedLightColor, fixedLightColor.displayName);
        m_MaterialEditor.ShaderProperty(lightSpace, lightSpace.displayName);
        m_MaterialEditor.ShaderProperty(noLightIndependLameRGB, noLightIndependLameRGB.displayName);

        m_MaterialEditor.ShaderProperty(minimalBrightness, minimalBrightness.displayName);
    }

    private void SetLameSpace(Material material, LameSpace lameSpace)
    {
        if (lameSpace == LameSpace.UV2D)
        {
            material.EnableKeyword("_LAMESPACE_UV2D");
        } else if (lameSpace == LameSpace.UV3D)
        {
            material.EnableKeyword("_LAMESPACE_UV3D");
        } else if (lameSpace == LameSpace.Local3D)
        {
            material.EnableKeyword("_LAMESPACE_LOCAL3D");
        } else if (lameSpace == LameSpace.Texture)
        {
            material.EnableKeyword("_LAMESPACE_TEXTURE");
        }
    }

    internal void DetermineWorkflow(MaterialProperty[] props)
    {
        if (FindProperty("_SpecGlossMap", props, false) != null && FindProperty("_SpecColor", props, false) != null)
            m_WorkflowMode = WorkflowMode.Specular;
        else if (FindProperty("_MetallicGlossMap", props, false) != null && FindProperty("_Metallic", props, false) != null)
            m_WorkflowMode = WorkflowMode.Metallic;
        else
            m_WorkflowMode = WorkflowMode.Dielectric;
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        // _Emission property is lost after assigning Standard shader to the material
        // thus transfer it before assigning the new shader
        if (material.HasProperty("_Emission"))
        {
            material.SetColor("_EmissionColor", material.GetColor("_Emission"));
        }

        base.AssignNewShaderToMaterial(material, oldShader, newShader);

        if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
        {
            SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
            return;
        }

        BlendMode blendMode = BlendMode.Opaque;
        if (oldShader.name.Contains("/Transparent/Cutout/"))
        {
            blendMode = BlendMode.Cutout;
        }
        else if (oldShader.name.Contains("/Transparent/"))
        {
            // NOTE: legacy shaders did not provide physically based transparency
            // therefore Fade mode
            blendMode = BlendMode.Fade;
        }
        material.SetFloat("_Mode", (float)blendMode);

        DetermineWorkflow(MaterialEditor.GetMaterialProperties(new Material[] { material }));
        MaterialChanged(material, m_WorkflowMode);
    }

    void BlendModePopup()
    {
        EditorGUI.showMixedValue = blendMode.hasMixedValue;
        var mode = (BlendMode)blendMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
            blendMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    void DoNormalArea()
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
        if (bumpScale.floatValue != 1)
            // && BuildTargetDiscovery.PlatformHasFlag(EditorUserBuildSettings.activeBuildTarget, TargetAttributes.HasIntegratedGPU))
            if (m_MaterialEditor.HelpBoxWithButton(
                EditorGUIUtility.TrTextContent("Bump scale is not supported on mobile platforms"),
                EditorGUIUtility.TrTextContent("Fix Now")))
            {
                bumpScale.floatValue = 1;
            }
    }

    void DoAlbedoArea(Material material)
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
        if (((BlendMode)material.GetFloat("_Mode") == BlendMode.Cutout))
        {
            m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        }
    }

    void DoEmissionArea(Material material)
    {
        // Emission for GI?
        if (m_MaterialEditor.EmissionEnabledProperty())
        {
            bool hadEmissionTexture = emissionMap.textureValue != null;

            // Texture and HDR color controls
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, false);

            // If texture was assigned and color was black set color to white
            float brightness = emissionColorForRendering.colorValue.maxColorComponent;
            if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                emissionColorForRendering.colorValue = Color.white;

            // change the GI flag and fix it up with emissive as black if necessary
            m_MaterialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
        }
    }

    void DoSpecularMetallicArea()
    {
        bool hasGlossMap = false;
        if (m_WorkflowMode == WorkflowMode.Specular)
        {
            hasGlossMap = specularMap.textureValue != null;
            m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, specularMap, hasGlossMap ? null : specularColor);
        }
        else if (m_WorkflowMode == WorkflowMode.Metallic)
        {
            hasGlossMap = metallicMap.textureValue != null;
            m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, metallicMap, hasGlossMap ? null : metallic);
        }

        bool showSmoothnessScale = hasGlossMap;
        if (smoothnessMapChannel != null)
        {
            int smoothnessChannel = (int)smoothnessMapChannel.floatValue;
            if (smoothnessChannel == (int)SmoothnessMapChannel.AlbedoAlpha)
                showSmoothnessScale = true;
        }

        int indentation = 2; // align with labels of texture properties
        m_MaterialEditor.ShaderProperty(showSmoothnessScale ? smoothnessScale : smoothness, showSmoothnessScale ? Styles.smoothnessScaleText : Styles.smoothnessText, indentation);

        ++indentation;
        if (smoothnessMapChannel != null)
            m_MaterialEditor.ShaderProperty(smoothnessMapChannel, Styles.smoothnessMapChannelText, indentation);
    }

    public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                break;
            case BlendMode.Fade:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
            case BlendMode.Transparent:
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                break;
        }
    }

    static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
    {
        int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
        if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
            return SmoothnessMapChannel.AlbedoAlpha;
        else
            return SmoothnessMapChannel.SpecularMetallicAlpha;
    }

    static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
    {
        // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
        // (MaterialProperty value might come from renderer material property block)
        SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
        if (workflowMode == WorkflowMode.Specular)
            SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
        else if (workflowMode == WorkflowMode.Metallic)
            SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
        SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
        SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

        // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
        // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
        // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
        MaterialEditor.FixupEmissiveFlag(material);
        bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
        SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

        if (material.HasProperty("_SmoothnessTextureChannel"))
        {
            SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
        }
    }

    static void MaterialChanged(Material material, WorkflowMode workflowMode)
    {
        SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

        SetMaterialKeywords(material, workflowMode);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}

