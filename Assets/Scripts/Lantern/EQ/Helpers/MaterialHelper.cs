using Lantern.EQ.Helpers;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lantern.Editor.Helpers
{
    public static class MaterialHelper
    {
        public static bool IsMaterialTransparent(Material material)
        {
            return material.name.StartsWith("t25_") || material.name.StartsWith("t50_") || material.name.StartsWith("t75_");
        }

        public static float GetMaterialTransparencyValue(Material material)
        {
            if (material.name.StartsWith("t25_"))
            {
                return 0.25f;
            }
        
            if (material.name.StartsWith("t50_"))
            {
                return 0.5f;
            }
        
            if (material.name.StartsWith("t75_"))
            {
                return 0.75f;
            }

            return 0f;
        }

        public static void SetRenderMode(string renderMode, Material material)
        {
            switch (renderMode)
            {
                case "Opaque":
                {
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_RENDERFOG_ON");
                    material.SetOverrideTag("RenderType", "Opaque");
                    material.renderQueue = 2000;
                    break;
                }
                case "Cutout":
                {
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_RENDERFOG_ON");
                    material.renderQueue = 2450;
                    material.SetOverrideTag("RenderType", "Opaque");
                    break;
                }

                case "Transparent":
                {
                    material.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.EnableKeyword("_RENDERFOG_ON");
                    material.renderQueue = 3000;
                    material.SetOverrideTag("RenderType", "Transparent");
                    break;
                }
                case "TransparentAdditive":
                {
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_RENDERFOG_ON");
                    material.renderQueue = 3000;
                    material.SetOverrideTag("RenderType", "Transparent");
                    break;
                }
                case "SkyboxDiffuse":
                {
                    material.renderQueue = 1000;
                    material.DisableKeyword("_RENDERFOG_ON");
                    break;
                }
                case "SkyboxTransparent":
                {
                    material.SetInt("_SrcBlend", (int) BlendMode.One);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.DisableKeyword("_RENDERFOG_ON");
                    material.renderQueue = 1100;
                    material.SetOverrideTag("RenderType", "Transparent");
                    break;
                }
                case "SkyboxAdditiveUnlit":
                {
                    material.renderQueue = 1200;
                    material.DisableKeyword("_RENDERFOG_ON");
                    break;
                }
            }
        }

        public static Material CreateMaterial(string materialName)
        {
            Material newMaterial;
            string litShaderName = ShaderHelper.GetLitShaderName(); 
            string unlitShaderName = ShaderHelper.GetUnlitShaderName();
            string skyShaderName = ShaderHelper.GetSkyShaderName();
            string invisibleShaderName = ShaderHelper.GetInvisibleShaderName();

            if (materialName.StartsWith("tm_"))
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader); 
                SetRenderMode("Cutout", newMaterial);
                
            }
            else if(materialName.StartsWith("t25_"))
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("Transparent", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.25f;
                newMaterial.SetColor("_BaseColor", newColor);
            }
            else if(materialName.StartsWith("t50_"))
            {
                if (materialName.Contains("normalcloud") || materialName.Contains("aircloud"))
                {
                    if (!GetShader(skyShaderName, out var shader))
                    {
                        return null;
                    }
                    newMaterial = new Material(shader);
                    SetRenderMode("Transparent", newMaterial);
                    newMaterial.renderQueue = 1200;
                }
                else
                {
                    if (!GetShader(litShaderName, out var shader))
                    {
                        return null;
                    }
                    newMaterial = new Material(shader);
                    SetRenderMode("Transparent", newMaterial);
                }

                Color newColor = Color.white;
                newColor.a = 0.5f;
                newMaterial.SetColor("_BaseColor", newColor);
            }
            else if(materialName.StartsWith("t75_"))
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("Transparent", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.75f;
                newMaterial.SetColor("_BaseColor", newColor);
            }
            else if(materialName.StartsWith("tau_"))
            {
                if (!GetShader(unlitShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("TransparentAdditive", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.75f;
                newMaterial.SetColor("_BaseColor", newColor);
            }
            else if(materialName.StartsWith("ta_"))
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("TransparentAdditive", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.75f;
                newMaterial.SetColor("_BaseColor", newColor);
            }
            else if(materialName.StartsWith("ds_"))
            {
                if (!GetShader(skyShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("SkyboxDiffuse", newMaterial);
            }
            else if(materialName.StartsWith("ts_"))
            {
                if (!GetShader(skyShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("SkyboxTransparent", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.5f;
                newMaterial.SetColor("_BaseColor", newColor);
                newMaterial.renderQueue = 1200;

            }
            else if(materialName.StartsWith("taus_"))
            {
                if (!GetShader(unlitShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode("TransparentAdditive", newMaterial);
                Color newColor = Color.white;
                newColor.a = 0.75f;
                newMaterial.SetColor("_BaseColor", newColor);
                newMaterial.renderQueue = 1100;
            }
            else if(materialName.StartsWith("b_") || materialName.StartsWith("i_"))
            {
                if (!GetShader(invisibleShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
            }
            else
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader); 
                SetRenderMode("Opaque", newMaterial);
            }

            return newMaterial;
        }

        private static bool GetShader(string shaderName, out Shader shader)
        {
            shader = Shader.Find(shaderName);

            if (shader == null)
            {
                Debug.LogError($"Unable to find shader: {shaderName}");
            }

            return shader != null;
        }
    }
}
