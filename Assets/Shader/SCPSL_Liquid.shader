Shader "SCPSL/Liquid"
{
Properties
{
    _Fill ("Fill", Float) = 0
    [HDR] _SideColor ("SideColor", Color) = (0.4,0.4745098,0.4,1)
    _WobbleZ ("WobbleZ", Float) = 0
    _WobbleX ("WobbleX", Float) = 0
    [HideInInspector] __dirty ("", Float) = 1
}
SubShader
{
   Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest"}
   LOD 200
   Cull Off
   CGPROGRAM
   #pragma surface surf Standard addshadow fullforwardshadows
   #pragma target 3.0
   
   struct Input
   {
    float3 worldPos;
   };
   float _Fill;
   float _WobbleX;
   float _WobbleZ;
   fixed4 _SideColor;

   void surf(Input IN, inout SurfaceOutputStandard o)
   {
    float3 objectPos = mul(unity_WorldToObject, float4(IN.worldPos, 1)).xyz;
    float wobbleX = objectPos.x * _WobbleZ;
    float wobbleZ = dot(float2(-0.4480736255645751953125, -0.893996655941009521484375), objectPos.
yz);
    wobbleX = (_WobbleX * wobbleZ) + wobbleX;
    float pivotY = unity_ObjectToWorld._m13;
    float heightCheck = (IN.worldPos.y - pivotY) + wobbleX;
    clip(_Fill - heightCheck);

    float peak = max(_SideColor.r, max(_SideColor.g, _SideColor.b));
    float3 col = (_SideColor.rgb / max(peak, 1.0)) * 0.01;
    o.Albedo = 0;
    o.Emission = col;
    o.Metallic = 0;
    o.Smoothness = 0;
    o.Alpha = 1;
   }
   ENDCG
}
CustomEditor "ASEMaterialInspector"
}