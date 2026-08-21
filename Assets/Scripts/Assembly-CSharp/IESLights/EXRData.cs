namespace IESLights
{
    public struct EXRData
    {
        public global::UnityEngine.Color[] Pixels;

        public uint Width;

        public uint Height;

        public EXRData(global::UnityEngine.Color[] pixels, int width, int height)
        {
            Pixels = pixels;
            Width = (uint)width;
            Height = (uint)height;
        }
    }
}
