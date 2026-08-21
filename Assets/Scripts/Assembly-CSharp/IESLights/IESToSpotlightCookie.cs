namespace IESLights
{
    [global::UnityEngine.ExecuteInEditMode]
    public class IESToSpotlightCookie : global::UnityEngine.MonoBehaviour
    {
        private global::UnityEngine.Material _spotlightMaterial;

        private global::UnityEngine.Material _fadeSpotlightEdgesMaterial;

        private global::UnityEngine.Material _verticalFlipMaterial;

        private void OnDestroy()
        {
            if (_spotlightMaterial != null)
            {
                global::UnityEngine.Object.Destroy(_spotlightMaterial);
            }
            if (_fadeSpotlightEdgesMaterial != null)
            {
                global::UnityEngine.Object.Destroy(_fadeSpotlightEdgesMaterial);
            }
            if (_verticalFlipMaterial != null)
            {
                global::UnityEngine.Object.Destroy(_verticalFlipMaterial);
            }
        }

        public void CreateSpotlightCookie(global::UnityEngine.Texture2D iesTexture, global::IESLights.IESData iesData, int resolution, bool applyVignette, bool flipVertically, out global::UnityEngine.Texture2D cookie)
        {
            if (iesData.PhotometricType != global::IESLights.PhotometricType.TypeA)
            {
                if (_spotlightMaterial == null)
                {
                    _spotlightMaterial = new global::UnityEngine.Material(global::UnityEngine.Shader.Find("Hidden/IES/IESToSpotlightCookie"));
                }
                CalculateAndSetSpotHeight(iesData);
                SetShaderKeywords(iesData, applyVignette);
                cookie = CreateTexture(iesTexture, resolution, flipVertically);
            }
            else
            {
                if (_fadeSpotlightEdgesMaterial == null)
                {
                    _fadeSpotlightEdgesMaterial = new global::UnityEngine.Material(global::UnityEngine.Shader.Find("Hidden/IES/FadeSpotlightCookieEdges"));
                }
                float verticalCenter = (applyVignette ? CalculateCookieVerticalCenter(iesData) : 0f);
                global::UnityEngine.Vector2 vector = (applyVignette ? CalculateCookieFadeEllipse(iesData) : global::UnityEngine.Vector2.zero);
                cookie = BlitToTargetSize(iesTexture, resolution, vector.x, vector.y, verticalCenter, applyVignette, flipVertically);
            }
        }

        private float CalculateCookieVerticalCenter(global::IESLights.IESData iesData)
        {
            float num = 1f - (float)iesData.PadBeforeAmount / (float)iesData.NormalizedValues[0].Count;
            float num2 = (float)(iesData.NormalizedValues[0].Count - iesData.PadBeforeAmount - iesData.PadAfterAmount) / (float)iesData.NormalizedValues.Count / 2f;
            return num - num2;
        }

        private global::UnityEngine.Vector2 CalculateCookieFadeEllipse(global::IESLights.IESData iesData)
        {
            if (iesData.HorizontalAngles.Count > iesData.VerticalAngles.Count)
            {
                return new global::UnityEngine.Vector2(0.5f, 0.5f * ((float)(iesData.NormalizedValues[0].Count - iesData.PadBeforeAmount - iesData.PadAfterAmount) / (float)iesData.NormalizedValues[0].Count));
            }
            if (iesData.HorizontalAngles.Count < iesData.VerticalAngles.Count)
            {
                return new global::UnityEngine.Vector2(0.5f * (global::System.Linq.Enumerable.Max(iesData.HorizontalAngles) - global::System.Linq.Enumerable.Min(iesData.HorizontalAngles)) / (global::System.Linq.Enumerable.Max(iesData.VerticalAngles) - global::System.Linq.Enumerable.Min(iesData.VerticalAngles)), 0.5f);
            }
            return new global::UnityEngine.Vector2(0.5f, 0.5f);
        }

        private global::UnityEngine.Texture2D CreateTexture(global::UnityEngine.Texture2D iesTexture, int resolution, bool flipVertically)
        {
            global::UnityEngine.RenderTexture temporary = global::UnityEngine.RenderTexture.GetTemporary(resolution, resolution, 0, global::UnityEngine.RenderTextureFormat.ARGBFloat, global::UnityEngine.RenderTextureReadWrite.Linear);
            temporary.filterMode = global::UnityEngine.FilterMode.Trilinear;
            temporary.DiscardContents();
            global::UnityEngine.RenderTexture.active = temporary;
            global::UnityEngine.Graphics.Blit(iesTexture, _spotlightMaterial);
            if (flipVertically)
            {
                global::UnityEngine.RenderTexture temporary2 = global::UnityEngine.RenderTexture.GetTemporary(resolution, resolution, 0, global::UnityEngine.RenderTextureFormat.ARGBFloat, global::UnityEngine.RenderTextureReadWrite.Linear);
                global::UnityEngine.Graphics.Blit(temporary, temporary2);
                FlipVertically(temporary2, temporary);
                global::UnityEngine.RenderTexture.ReleaseTemporary(temporary2);
            }
            global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(resolution, resolution, global::UnityEngine.TextureFormat.RGBAFloat, mipChain: false, linear: true);
            texture2D.filterMode = global::UnityEngine.FilterMode.Trilinear;
            texture2D.wrapMode = global::UnityEngine.TextureWrapMode.Clamp;
            texture2D.ReadPixels(new global::UnityEngine.Rect(0f, 0f, resolution, resolution), 0, 0);
            texture2D.Apply();
            global::UnityEngine.RenderTexture.active = null;
            global::UnityEngine.RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }

        private global::UnityEngine.Texture2D BlitToTargetSize(global::UnityEngine.Texture2D iesTexture, int resolution, float horizontalFadeDistance, float verticalFadeDistance, float verticalCenter, bool applyVignette, bool flipVertically)
        {
            if (applyVignette)
            {
                _fadeSpotlightEdgesMaterial.SetFloat("_HorizontalFadeDistance", horizontalFadeDistance);
                _fadeSpotlightEdgesMaterial.SetFloat("_VerticalFadeDistance", verticalFadeDistance);
                _fadeSpotlightEdgesMaterial.SetFloat("_VerticalCenter", verticalCenter);
            }
            global::UnityEngine.RenderTexture temporary = global::UnityEngine.RenderTexture.GetTemporary(resolution, resolution, 0, global::UnityEngine.RenderTextureFormat.ARGBFloat, global::UnityEngine.RenderTextureReadWrite.Linear);
            temporary.filterMode = global::UnityEngine.FilterMode.Trilinear;
            temporary.DiscardContents();
            if (applyVignette)
            {
                global::UnityEngine.RenderTexture.active = temporary;
                global::UnityEngine.Graphics.Blit(iesTexture, _fadeSpotlightEdgesMaterial);
            }
            else if (flipVertically)
            {
                FlipVertically(iesTexture, temporary);
            }
            else
            {
                global::UnityEngine.Graphics.Blit(iesTexture, temporary);
            }
            global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(resolution, resolution, global::UnityEngine.TextureFormat.RGBAFloat, mipChain: false, linear: true);
            texture2D.filterMode = global::UnityEngine.FilterMode.Trilinear;
            texture2D.wrapMode = global::UnityEngine.TextureWrapMode.Clamp;
            texture2D.ReadPixels(new global::UnityEngine.Rect(0f, 0f, resolution, resolution), 0, 0);
            texture2D.Apply();
            global::UnityEngine.RenderTexture.active = null;
            global::UnityEngine.RenderTexture.ReleaseTemporary(temporary);
            return texture2D;
        }

        private void FlipVertically(global::UnityEngine.Texture iesTexture, global::UnityEngine.RenderTexture renderTarget)
        {
            if (_verticalFlipMaterial == null)
            {
                _verticalFlipMaterial = new global::UnityEngine.Material(global::UnityEngine.Shader.Find("Hidden/IES/VerticalFlip"));
            }
            global::UnityEngine.Graphics.Blit(iesTexture, renderTarget, _verticalFlipMaterial);
        }

        private void CalculateAndSetSpotHeight(global::IESLights.IESData iesData)
        {
            float value = 0.5f / global::UnityEngine.Mathf.Tan(iesData.HalfSpotlightFov * ((float)global::System.Math.PI / 180f));
            _spotlightMaterial.SetFloat("_SpotHeight", value);
        }

        private void SetShaderKeywords(global::IESLights.IESData iesData, bool applyVignette)
        {
            if (applyVignette)
            {
                _spotlightMaterial.EnableKeyword("VIGNETTE");
            }
            else
            {
                _spotlightMaterial.DisableKeyword("VIGNETTE");
            }
            if (iesData.VerticalType == global::IESLights.VerticalType.Top)
            {
                _spotlightMaterial.EnableKeyword("TOP_VERTICAL");
            }
            else
            {
                _spotlightMaterial.DisableKeyword("TOP_VERTICAL");
            }
            if (iesData.HorizontalType == global::IESLights.HorizontalType.None)
            {
                _spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Quadrant)
            {
                _spotlightMaterial.EnableKeyword("QUAD_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Half)
            {
                _spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
                _spotlightMaterial.EnableKeyword("HALF_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Full)
            {
                _spotlightMaterial.DisableKeyword("QUAD_HORIZONTAL");
                _spotlightMaterial.DisableKeyword("HALF_HORIZONTAL");
                _spotlightMaterial.EnableKeyword("FULL_HORIZONTAL");
            }
        }
    }
}
