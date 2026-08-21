namespace IESLights
{
    public class IESData
    {
        public global::System.Collections.Generic.List<float> VerticalAngles { get; set; }

        public global::System.Collections.Generic.List<float> HorizontalAngles { get; set; }

        public global::System.Collections.Generic.List<global::System.Collections.Generic.List<float>> CandelaValues { get; set; }

        public global::System.Collections.Generic.List<global::System.Collections.Generic.List<float>> NormalizedValues { get; set; }

        public global::IESLights.PhotometricType PhotometricType { get; set; }

        public global::IESLights.VerticalType VerticalType { get; set; }

        public global::IESLights.HorizontalType HorizontalType { get; set; }

        public int PadBeforeAmount { get; set; }

        public int PadAfterAmount { get; set; }

        public float HalfSpotlightFov { get; set; }
    }
}
