using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LameShaderGUI : ShaderGUI
{
    private static class Styles
    {
        public static GUIContent albedoText = EditorGUIUtility.TrTextContent("Albedo", "Albedo (RGB) and Transparency (A)");
       // public static GUIContent alphaCutoffText = EditorGUIUtility.TrTextContent("Alpha Cutoff", "Threshold for alpha cutoff");
        public static GUIContent normalMapText = EditorGUIUtility.TrTextContent("Normal Map", "Normal Map");
        
        public static string primaryMapsText = "Main Maps";
        public static string forwardText = "Forward Rendering Options";
        public static string advancedText = "Advanced Options";
        public static string lameText = "Lame Params";
    }
    public enum LameSpace
    {
        UV2D = 0,
        UV3D = 1,
        Local3D = 2,
        Texture = 3,
    }
    //Common
    MaterialProperty albedoMap = null;
    MaterialProperty albedoColor = null;
    MaterialProperty bumpMap = null;
    //Lit
    MaterialProperty speculatPower = null;
    MaterialProperty reflectionBalance = null;
    
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
    public void FindProperties(MaterialProperty[] props)
    {
        Material material = m_MaterialEditor.target as Material;
        
        //primary
        albedoMap = FindProperty("_MainTex", props);
        albedoColor = FindProperty("_Color", props);
        bumpMap = FindProperty("_BumpMap", props);
        
        bool isLit = material.shader.name.Contains("Lit");
        if(isLit) speculatPower = FindProperty("_SpecPower", props);
        if(isLit) reflectionBalance = FindProperty("_ReflectionBalance", props);
        
        
        //Lame Params
        lameSpace = FindProperty("_LameSpace", props);
        lameDirTex = FindProperty("_LameDirTex", props);
        lameColorMap = FindProperty("_LameColorMap", props);
        lameColor = FindProperty("_LameColor", props);
        independLameRGB = FindProperty("_IndependLameRGB", props);
        
        bool isTransparent = material.shader.name.Contains("Transparent");
        if(isTransparent) independLameAlpha = FindProperty("_IndependLameAlpha", props);
        
        randomColor = FindProperty("_RandomColor", props);
        randomColorBlend = FindProperty("_RandomColorBlend", props);
        lameScale = FindProperty("_LameScale", props);
        lameParallaxDepth = FindProperty("_LameParallaxDepth", props);
        lameAmount = FindProperty("_LameAmount", props);
        lameJitter = FindProperty("_LameJitter", props);
        lameStrengthMap = FindProperty("_LameStrengthMap", props);
        if(isLit) lameLightAttenInfluence = FindProperty("_LightAttenuationInfluence", props);
        lameEmissive = FindProperty("_LameEmissive", props);
        lameAnimate = FindProperty("_Animate", props);
        lameAniamtionSpeed = FindProperty("_AnimationSpeed", props);

        minimalBrightness = FindProperty("_MinimalBrightness", props);
        noLightLame = FindProperty("_NoLightLame", props);
        fixedLightDirection = FindProperty("_FixedLightDir", props);
        fixedLightColor = FindProperty("_FixedLightColor", props);
        lightSpace = FindProperty("_LightDirSpace", props);
        noLightIndependLameRGB = FindProperty("_NoLightIndependLameRGB", props);
    }
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        Material material = materialEditor.target as Material;
        m_MaterialEditor = materialEditor;
        FindProperties(props);
        ShaderPropertiesGUI(material);
    }
    
    public void ShaderPropertiesGUI(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

            //Primary
            GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);
            DoAlbedoArea(material);
            DoNormalArea();
            
            bool isLit = material.shader.name.Contains("Lit");
            if(isLit) m_MaterialEditor.ShaderProperty(speculatPower, speculatPower.displayName);
            if(isLit) m_MaterialEditor.ShaderProperty(reflectionBalance, reflectionBalance.displayName);
            
            EditorGUILayout.Space();

            //Lame
            GUILayout.Label(Styles.lameText, EditorStyles.boldLabel);
            DoLameArea(material);
        

        EditorGUILayout.Space();
        
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
        bool isTransparent = material.shader.name.Contains("Transparent"); 
        if(isTransparent) m_MaterialEditor.ShaderProperty(independLameAlpha, independLameAlpha.displayName);
        m_MaterialEditor.ShaderProperty(randomColor, randomColor.displayName);
        m_MaterialEditor.ShaderProperty(randomColorBlend, randomColorBlend.displayName);
        if(!spaceIsTexture) m_MaterialEditor.ShaderProperty(lameScale, lameScale.displayName);
        m_MaterialEditor.ShaderProperty(lameParallaxDepth, lameParallaxDepth.displayName);
        m_MaterialEditor.ShaderProperty(lameAmount, lameAmount.displayName);
        m_MaterialEditor.ShaderProperty(lameJitter, lameJitter.displayName);
        
        bool isLit = material.shader.name.Contains("Lit");
        if(isLit) m_MaterialEditor.ShaderProperty(lameLightAttenInfluence, lameLightAttenInfluence.displayName);
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
    
    void DoNormalArea()
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap);
    }
    
    void DoAlbedoArea(Material material)
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
    }
}
