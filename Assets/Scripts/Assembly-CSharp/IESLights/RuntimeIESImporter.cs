namespace IESLights
{
    public class RuntimeIESImporter : global::UnityEngine.MonoBehaviour
    {
        public static void Import(string path, out global::UnityEngine.Texture2D spotlightCookie, out global::UnityEngine.Cubemap pointLightCookie, int resolution = 128, bool enhancedImport = false, bool applyVignette = true)
        {
            spotlightCookie = null;
            pointLightCookie = null;
            if (IsFileValid(path))
            {
                GetIESConverterAndCubeSphere(enhancedImport, resolution, out var cubemapSphere, out var iesConverter);
                ImportIES(path, iesConverter, allowSpotlightCookies: true, applyVignette, out spotlightCookie, out pointLightCookie);
                global::UnityEngine.Object.Destroy(cubemapSphere);
            }
        }

        public static global::UnityEngine.Texture2D ImportSpotlightCookie(string path, int resolution = 128, bool enhancedImport = false, bool applyVignette = true)
        {
            if (!IsFileValid(path))
            {
                return null;
            }
            GetIESConverterAndCubeSphere(enhancedImport, resolution, out var cubemapSphere, out var iesConverter);
            ImportIES(path, iesConverter, allowSpotlightCookies: true, applyVignette, out var spotlightCookie, out var _);
            global::UnityEngine.Object.Destroy(cubemapSphere);
            return spotlightCookie;
        }

        public static global::UnityEngine.Cubemap ImportPointLightCookie(string path, int resolution = 128, bool enhancedImport = false)
        {
            if (!IsFileValid(path))
            {
                return null;
            }
            GetIESConverterAndCubeSphere(enhancedImport, resolution, out var cubemapSphere, out var iesConverter);
            ImportIES(path, iesConverter, allowSpotlightCookies: false, applyVignette: false, out var _, out var pointlightCookie);
            global::UnityEngine.Object.Destroy(cubemapSphere);
            return pointlightCookie;
        }

        private static void GetIESConverterAndCubeSphere(bool logarithmicNormalization, int resolution, out global::UnityEngine.GameObject cubemapSphere, out global::IESLights.IESConverter iesConverter)
        {
            global::UnityEngine.Object original = global::UnityEngine.Resources.Load("IES cubemap sphere");
            cubemapSphere = (global::UnityEngine.GameObject)global::UnityEngine.Object.Instantiate(original);
            iesConverter = cubemapSphere.GetComponent<global::IESLights.IESConverter>();
            iesConverter.NormalizationMode = (logarithmicNormalization ? global::IESLights.NormalizationMode.Logarithmic : global::IESLights.NormalizationMode.Linear);
            iesConverter.Resolution = resolution;
        }

        private static void ImportIES(string path, global::IESLights.IESConverter iesConverter, bool allowSpotlightCookies, bool applyVignette, out global::UnityEngine.Texture2D spotlightCookie, out global::UnityEngine.Cubemap pointlightCookie)
        {
            string targetFilename = null;
            spotlightCookie = null;
            pointlightCookie = null;
            try
            {
                iesConverter.ConvertIES(path, "", allowSpotlightCookies, rawImport: false, applyVignette, out pointlightCookie, out spotlightCookie, out var _, out targetFilename);
            }
            catch (global::IESLights.IESParseException ex)
            {
                global::UnityEngine.Debug.LogError($"[IES] Encountered invalid IES data in {path}. Error message: {ex.Message}");
            }
            catch (global::System.Exception ex2)
            {
                global::UnityEngine.Debug.LogError($"[IES] Error while parsing {path}. Please contact me through the forums or thomasmountainborn.com. Error message: {ex2.Message}");
            }
        }

        private static bool IsFileValid(string path)
        {
            if (!global::System.IO.File.Exists(path))
            {
                global::UnityEngine.Debug.LogWarningFormat("[IES] The file \"{0}\" does not exist.", path);
                return false;
            }
            if (global::System.IO.Path.GetExtension(path).ToLower() != ".ies")
            {
                global::UnityEngine.Debug.LogWarningFormat("[IES] The file \"{0}\" is not an IES file.", path);
                return false;
            }
            return true;
        }
    }
}
