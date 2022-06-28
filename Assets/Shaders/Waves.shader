Shader "Custom/Waves" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 1.0
		_Metallic ("Metallic", Range(0,1)) = 0.3

		_Speed ("Animation speed", Float) = 1.0
		_Centre ("Centre", Vector) = (-5, 5, 0, 0)
		_Amplitude ("Amplitude", Range(0, 1.0)) = 0.02
		_Frequency ("Frequency", Range(0, 1000)) = 10.0
		_Phase ("Phase", Float) = 3.0
		_Falloff ("Falloff", Range(0,20)) = 3.0

	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 normal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _Speed;
		float2 _Centre;
		float _Amplitude;
		float _Frequency;
		float _Phase;
		float _Falloff;
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o)
		{
		    UNITY_INITIALIZE_OUTPUT(Input,o);

			// Get 2D vector for vertex coordinates
			float2 ver = float2(v.vertex.x, v.vertex.y);

			// Collect and normalize wave directions
			float2 orientation = (_Centre - ver);
			float2 dir = normalize(orientation);

			// Set parameters used in simulation
			float t = _Time.y * _Speed;
			float k = 2.5;
			float amp = min(min(_Amplitude, 0.05), _Amplitude/pow(length(orientation), _Falloff));

			// Simulate first wave
			v.vertex.z += 2.0*amp*(pow(sin(_Frequency*-dot(orientation, dir) + t*_Phase) + 1.0, k)/2.0);
			
			// Get derivatives for waves
			float dt = k*_Frequency*amp*(pow(sin(_Frequency*-dot(orientation, dir) + t*_Phase) + 1.0, k-1)/2.0)*cos(_Frequency*-dot(orientation, dir) + t*_Phase);


			float dx = dir.x*dt;
			float dy = dir.y*dt;

			// Set normal for fragment
			v.normal = float3(-dx, -dy, 1.0);
			v.normal = normalize(v.normal);
			o.normal = v.normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			// Assign normal defined in vertex shader
			o.Normal = IN.normal;
			o.Alpha = c.w;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
