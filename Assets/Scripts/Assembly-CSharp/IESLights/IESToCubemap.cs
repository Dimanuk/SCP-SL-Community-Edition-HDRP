namespace IESLights
{
    [global::UnityEngine.ExecuteInEditMode]
    public class IESToCubemap : global::UnityEngine.MonoBehaviour
    {
        private global::UnityEngine.Material _iesMaterial;

        private global::UnityEngine.Material _horizontalMirrorMaterial;

        private void OnDestroy()
        {
            if (_horizontalMirrorMaterial != null)
            {
                global::UnityEngine.Object.DestroyImmediate(_horizontalMirrorMaterial);
            }
        }

        public void CreateCubemap(global::UnityEngine.Texture2D iesTexture, global::IESLights.IESData iesData, int resolution, out global::UnityEngine.Cubemap cubemap)
        {
            PrepMaterial(iesTexture, iesData);
            CreateCubemap(resolution, out cubemap);
        }

        public global::UnityEngine.Color[] CreateRawCubemap(global::UnityEngine.Texture2D iesTexture, global::IESLights.IESData iesData, int resolution)
        {
            PrepMaterial(iesTexture, iesData);
            global::UnityEngine.RenderTexture[] array = new global::UnityEngine.RenderTexture[6];
            for (int i = 0; i < 6; i++)
            {
                array[i] = global::UnityEngine.RenderTexture.GetTemporary(resolution, resolution, 0, global::UnityEngine.RenderTextureFormat.ARGBFloat, global::UnityEngine.RenderTextureReadWrite.Linear);
                array[i].filterMode = global::UnityEngine.FilterMode.Trilinear;
            }
            global::UnityEngine.Camera[] componentsInChildren = base.transform.GetChild(0).GetComponentsInChildren<global::UnityEngine.Camera>();
            for (int j = 0; j < 6; j++)
            {
                componentsInChildren[j].targetTexture = array[j];
                componentsInChildren[j].Render();
                componentsInChildren[j].targetTexture = null;
            }
            global::UnityEngine.RenderTexture temporary = global::UnityEngine.RenderTexture.GetTemporary(resolution * 6, resolution, 0, global::UnityEngine.RenderTextureFormat.ARGBFloat, global::UnityEngine.RenderTextureReadWrite.Linear);
            temporary.filterMode = global::UnityEngine.FilterMode.Trilinear;
            if (_horizontalMirrorMaterial == null)
            {
                _horizontalMirrorMaterial = new global::UnityEngine.Material(global::UnityEngine.Shader.Find("Hidden/IES/HorizontalFlip"));
            }
            global::UnityEngine.RenderTexture.active = temporary;
            for (int k = 0; k < 6; k++)
            {
                global::UnityEngine.GL.PushMatrix();
                global::UnityEngine.GL.LoadPixelMatrix(0f, resolution * 6, 0f, resolution);
                global::UnityEngine.Graphics.DrawTexture(new global::UnityEngine.Rect(k * resolution, 0f, resolution, resolution), array[k], _horizontalMirrorMaterial);
                global::UnityEngine.GL.PopMatrix();
            }
            global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(resolution * 6, resolution, global::UnityEngine.TextureFormat.RGBAFloat, mipChain: false, linear: true)
            {
                filterMode = global::UnityEngine.FilterMode.Trilinear
            };
            texture2D.ReadPixels(new global::UnityEngine.Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
            global::UnityEngine.Color[] pixels = texture2D.GetPixels();
            global::UnityEngine.RenderTexture.active = null;
            global::UnityEngine.RenderTexture[] array2 = array;
            for (int l = 0; l < array2.Length; l++)
            {
                global::UnityEngine.RenderTexture.ReleaseTemporary(array2[l]);
            }
            global::UnityEngine.RenderTexture.ReleaseTemporary(temporary);
            global::UnityEngine.Object.DestroyImmediate(texture2D);
            return pixels;
        }

        private void PrepMaterial(global::UnityEngine.Texture2D iesTexture, global::IESLights.IESData iesData)
        {
            if (_iesMaterial == null)
            {
                _iesMaterial = GetComponent<global::UnityEngine.Renderer>().sharedMaterial;
            }
            _iesMaterial.mainTexture = iesTexture;
            SetShaderKeywords(iesData, _iesMaterial);
        }

        private void SetShaderKeywords(global::IESLights.IESData iesData, global::UnityEngine.Material iesMaterial)
        {
            if (iesData.VerticalType == global::IESLights.VerticalType.Bottom)
            {
                iesMaterial.EnableKeyword("BOTTOM_VERTICAL");
                iesMaterial.DisableKeyword("TOP_VERTICAL");
                iesMaterial.DisableKeyword("FULL_VERTICAL");
            }
            else if (iesData.VerticalType == global::IESLights.VerticalType.Top)
            {
                iesMaterial.EnableKeyword("TOP_VERTICAL");
                iesMaterial.DisableKeyword("BOTTOM_VERTICAL");
                iesMaterial.DisableKeyword("FULL_VERTICAL");
            }
            else
            {
                iesMaterial.DisableKeyword("TOP_VERTICAL");
                iesMaterial.DisableKeyword("BOTTOM_VERTICAL");
                iesMaterial.EnableKeyword("FULL_VERTICAL");
            }
            if (iesData.HorizontalType == global::IESLights.HorizontalType.None)
            {
                iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
                iesMaterial.DisableKeyword("HALF_HORIZONTAL");
                iesMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Quadrant)
            {
                iesMaterial.EnableKeyword("QUAD_HORIZONTAL");
                iesMaterial.DisableKeyword("HALF_HORIZONTAL");
                iesMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Half)
            {
                iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
                iesMaterial.EnableKeyword("HALF_HORIZONTAL");
                iesMaterial.DisableKeyword("FULL_HORIZONTAL");
            }
            else if (iesData.HorizontalType == global::IESLights.HorizontalType.Full)
            {
                iesMaterial.DisableKeyword("QUAD_HORIZONTAL");
                iesMaterial.DisableKeyword("HALF_HORIZONTAL");
                iesMaterial.EnableKeyword("FULL_HORIZONTAL");
            }
        }

        private void CreateCubemap(int resolution, out global::UnityEngine.Cubemap cubemap)
        {
            cubemap = new global::UnityEngine.Cubemap(resolution, global::UnityEngine.TextureFormat.ARGB32, mipChain: false)
            {
                filterMode = global::UnityEngine.FilterMode.Trilinear
            };
            GetComponent<global::UnityEngine.Camera>().RenderToCubemap(cubemap);
        }
    }
}
