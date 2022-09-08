using UnityEngine;
using UnityEngine.Rendering;

namespace Lantern.EQ.Helpers
{
    public static class MaterialHelper
    {
        public enum ShaderType
        {
            Diffuse = 0,
            Cutout = 1,
            Transparent = 2,
            TransparentAdditive = 3,
            SkyboxDiffuse = 4,
            SkyboxTransparent = 5,
            SkyboxAdditive = 6
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
                SetRenderMode(ShaderType.Cutout, newMaterial);
            }
            else if(materialName.StartsWith("t25_"))
            {
                if (!GetShader(litShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode(ShaderType.Transparent, newMaterial);
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
                    SetRenderMode(ShaderType.Transparent, newMaterial);
                    newMaterial.renderQueue = 1200;
                }
                else
                {
                    if (!GetShader(litShaderName, out var shader))
                    {
                        return null;
                    }
                    newMaterial = new Material(shader);
                    SetRenderMode(ShaderType.Transparent, newMaterial);
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
                SetRenderMode(ShaderType.Transparent, newMaterial);
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
                SetRenderMode(ShaderType.TransparentAdditive, newMaterial);
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
                SetRenderMode(ShaderType.TransparentAdditive, newMaterial);
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
                SetRenderMode(ShaderType.SkyboxDiffuse, newMaterial);
            }
            else if(materialName.StartsWith("ts_"))
            {
                if (!GetShader(skyShaderName, out var shader))
                {
                    return null;
                }
                newMaterial = new Material(shader);
                SetRenderMode(ShaderType.SkyboxTransparent, newMaterial);
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
                SetRenderMode(ShaderType.TransparentAdditive, newMaterial);
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
                SetRenderMode(ShaderType.Diffuse, newMaterial);
            }

            return newMaterial;
        }

        public static void SetRenderMode(ShaderType shaderType, Material material)
        {
            // As we're creating these materials from scratch, we must set these values.
            switch (shaderType)
            {
                case ShaderType.Diffuse:
                {
                    material.EnableKeyword("_ENABLE_FOG");
                    material.renderQueue = 2000;
                    break;
                }
                case ShaderType.Cutout:
                {
                    material.EnableKeyword("_ENABLE_FOG");
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.SetFloat("_AlphaClip", 1);
                    material.renderQueue = 2450;
                    break;
                }
                case ShaderType.Transparent:
                {
                    material.EnableKeyword("_ENABLE_FOG");
                    material.SetFloat("_Surface", 1);
                    material.SetFloat("_Blend", 0);
                    material.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetFloat("_ZWrite", 0);
                    material.renderQueue = 3000;
                    break;
                }
                case ShaderType.TransparentAdditive:
                {
                    material.SetFloat("_Surface", 1);
                    material.SetFloat("_Blend", 2);
                    material.SetInt("_SrcBlend", (int)BlendMode.One);
                    material.SetInt("_DstBlend", (int)BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.renderQueue = 3000;
                    break;
                }
                case ShaderType.SkyboxDiffuse:
                {
                    material.renderQueue = 1000;
                    break;
                }
                case ShaderType.SkyboxTransparent:
                {
                    material.SetInt("_SrcBlend", (int) BlendMode.One);
                    material.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 1100;
                    material.SetOverrideTag("RenderType", "Transparent");
                    break;
                }
                case ShaderType.SkyboxAdditive:
                {
                    material.renderQueue = 1200;
                    break;
                }
            }
        }

        public static bool IsMasked(string textureName)
        {
            return textureName.StartsWith("tm_");
        }

        public static bool IsAlphaBlended(string textureName)
        {
            return textureName.StartsWith("tau_") || textureName.StartsWith("ta_");
        }

        public static bool IsTransparent(Material material)
        {
            return material.name.StartsWith("t25_") || material.name.StartsWith("t50_") || material.name.StartsWith("t75_");
        }

        public static float GetTransparencyValue(Material material)
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
