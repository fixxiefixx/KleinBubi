// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Snow/Diffuse"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Snow("Snow (RGB)", 2D) = "white" {}
	_Amount("Snow Amount", Range(0,1)) = 0.5
		_Multiplier("Snow Multiplier",Range(0,100))=10
		_Extrusion("Extrusion Amount", Range(0,1)) = 0.5
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
#pragma surface surf Lambert vertex:vert

		struct Input
	{
		float2 uv_MainTex;
		float2 uv_Snow;
		float snow;
	};

	float _Amount;
	float _Extrusion;
	float _Multiplier;

	void vert(inout appdata_full v, out Input o)
	{
		float3 sn = mul((float3x3)unity_ObjectToWorld, v.normal.xyz).xyz;
		sn = normalize(sn);
		float snow = max(0, sn.y) * _Amount;
		v.vertex.y += snow * _Extrusion;

		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.snow = snow;
	}

	sampler2D _MainTex;
	sampler2D _Snow;

	void surf(Input IN, inout SurfaceOutput o)
	{
		half3 main = tex2D(_MainTex, IN.uv_MainTex).rgb;
		half3 snow = tex2D(_Snow, IN.uv_Snow).rgb;
		o.Albedo = lerp(main, snow, min(1, IN.snow * _Multiplier));
	}
	ENDCG
	}
		FallBack "Diffuse"
}
