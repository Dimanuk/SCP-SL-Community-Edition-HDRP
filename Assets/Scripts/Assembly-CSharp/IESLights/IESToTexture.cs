namespace IESLights
{
    public class IESToTexture : global::UnityEngine.MonoBehaviour
    {
        public static global::UnityEngine.Texture2D ConvertIesData(global::IESLights.IESData data)
        {
            global::UnityEngine.Texture2D texture2D = new global::UnityEngine.Texture2D(data.NormalizedValues.Count, data.NormalizedValues[0].Count, global::UnityEngine.TextureFormat.RGBAFloat, mipChain: false, linear: true)
            {
                filterMode = global::UnityEngine.FilterMode.Trilinear,
                wrapMode = global::UnityEngine.TextureWrapMode.Clamp
            };
            global::UnityEngine.Color[] array = new global::UnityEngine.Color[texture2D.width * texture2D.height];
            for (int i = 0; i < texture2D.width; i++)
            {
                for (int j = 0; j < texture2D.height; j++)
                {
                    float num = data.NormalizedValues[i][j];
                    array[i + j * texture2D.width] = new global::UnityEngine.Color(num, num, num, num);
                }
            }
            texture2D.SetPixels(array);
            texture2D.Apply();
            return texture2D;
        }
    }
}
