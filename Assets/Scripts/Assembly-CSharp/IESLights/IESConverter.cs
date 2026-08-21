namespace IESLights
{
    [global::UnityEngine.RequireComponent(typeof(global::IESLights.IESToCubemap))]
    [global::UnityEngine.RequireComponent(typeof(global::IESLights.IESToSpotlightCookie))]
    public class IESConverter : global::UnityEngine.MonoBehaviour
    {
        public int Resolution = 512;

        public global::IESLights.NormalizationMode NormalizationMode;

        private global::UnityEngine.Texture2D _iesTexture;

        public void ConvertIES(string filePath, string targetPath, bool createSpotlightCookies, bool rawImport, bool applyVignette, out global::UnityEngine.Cubemap pointLightCookie, out global::UnityEngine.Texture2D spotlightCookie, out global::IESLights.EXRData exrData, out string targetFilename)
        {
            global::IESLights.IESData iESData = global::IESLights.ParseIES.Parse(filePath, (!rawImport) ? NormalizationMode : global::IESLights.NormalizationMode.Linear);
            _iesTexture = global::IESLights.IESToTexture.ConvertIesData(iESData);
            if (!rawImport)
            {
                exrData = default(global::IESLights.EXRData);
                RegularImport(filePath, targetPath, createSpotlightCookies, applyVignette, out pointLightCookie, out spotlightCookie, out targetFilename, iESData);
            }
            else
            {
                pointLightCookie = null;
                spotlightCookie = null;
                RawImport(iESData, filePath, targetPath, createSpotlightCookies, out exrData, out targetFilename);
            }
            if (_iesTexture != null)
            {
                global::UnityEngine.Object.Destroy(_iesTexture);
            }
        }

        private void RegularImport(string filePath, string targetPath, bool createSpotlightCookies, bool applyVignette, out global::UnityEngine.Cubemap pointLightCookie, out global::UnityEngine.Texture2D spotlightCookie, out string targetFilename, global::IESLights.IESData iesData)
        {
            if ((createSpotlightCookies && iesData.VerticalType != global::IESLights.VerticalType.Full) || iesData.PhotometricType == global::IESLights.PhotometricType.TypeA)
            {
                pointLightCookie = null;
                GetComponent<global::IESLights.IESToSpotlightCookie>().CreateSpotlightCookie(_iesTexture, iesData, Resolution, applyVignette, flipVertically: false, out spotlightCookie);
            }
            else
            {
                spotlightCookie = null;
                GetComponent<global::IESLights.IESToCubemap>().CreateCubemap(_iesTexture, iesData, Resolution, out pointLightCookie);
            }
            BuildTargetFilename(global::System.IO.Path.GetFileNameWithoutExtension(filePath), targetPath, pointLightCookie != null, isRaw: false, NormalizationMode, iesData, out targetFilename);
        }

        private void RawImport(global::IESLights.IESData iesData, string filePath, string targetPath, bool createSpotlightCookie, out global::IESLights.EXRData exrData, out string targetFilename)
        {
            if ((createSpotlightCookie && iesData.VerticalType != global::IESLights.VerticalType.Full) || iesData.PhotometricType == global::IESLights.PhotometricType.TypeA)
            {
                global::UnityEngine.Texture2D cookie = null;
                GetComponent<global::IESLights.IESToSpotlightCookie>().CreateSpotlightCookie(_iesTexture, iesData, Resolution, applyVignette: false, flipVertically: true, out cookie);
                exrData = new global::IESLights.EXRData(cookie.GetPixels(), Resolution, Resolution);
                global::UnityEngine.Object.DestroyImmediate(cookie);
            }
            else
            {
                exrData = new global::IESLights.EXRData(GetComponent<global::IESLights.IESToCubemap>().CreateRawCubemap(_iesTexture, iesData, Resolution), Resolution * 6, Resolution);
            }
            BuildTargetFilename(global::System.IO.Path.GetFileNameWithoutExtension(filePath), targetPath, isCubemap: false, isRaw: true, global::IESLights.NormalizationMode.Linear, iesData, out targetFilename);
        }

        private void BuildTargetFilename(string name, string folderHierarchy, bool isCubemap, bool isRaw, global::IESLights.NormalizationMode normalizationMode, global::IESLights.IESData iesData, out string targetFilePath)
        {
            if (!global::System.IO.Directory.Exists(global::System.IO.Path.Combine(global::UnityEngine.Application.dataPath, $"IES/Imports/{folderHierarchy}")))
            {
                global::System.IO.Directory.CreateDirectory(global::System.IO.Path.Combine(global::UnityEngine.Application.dataPath, $"IES/Imports/{folderHierarchy}"));
            }
            float num = 0f;
            if (iesData.PhotometricType == global::IESLights.PhotometricType.TypeA)
            {
                num = global::System.Linq.Enumerable.Max(iesData.HorizontalAngles) - global::System.Linq.Enumerable.Min(iesData.HorizontalAngles);
            }
            else if (!isCubemap)
            {
                num = iesData.HalfSpotlightFov * 2f;
            }
            string text = "";
            switch (normalizationMode)
            {
                case global::IESLights.NormalizationMode.EqualizeHistogram:
                    text = "[H] ";
                    break;
                case global::IESLights.NormalizationMode.Logarithmic:
                    text = "[E] ";
                    break;
            }
            string text2 = "";
            text2 = ((!isRaw) ? (isCubemap ? "cubemap" : "asset") : "exr");
            targetFilePath = global::System.IO.Path.Combine(global::System.IO.Path.Combine("Assets/IES/Imports/", folderHierarchy), string.Format("{0}{1}{2}.{3}", text, (iesData.PhotometricType == global::IESLights.PhotometricType.TypeA || !isCubemap) ? ("[FOV " + num + "] ") : "", name, text2));
            if (global::System.IO.File.Exists(targetFilePath))
            {
                global::System.IO.File.Delete(targetFilePath);
            }
        }
    }
}
