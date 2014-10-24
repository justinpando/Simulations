Shader "_ThrustBusters/Particle_01"
{
    Properties
    {
		_AmbientColor ("Ambient Color", Color) = (1,1,1,1)
		_MainTex ("Main Tex (RGB, Alpha (A))", 2D) = "white" {}
		_NormalMap ("Bumpmap", 2D) = "bump" {}
		
		_RimSwitch ("Rim On(1)/Off(0)", float) = 0.0
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,10)) = 3.0
		
		_Cube ("Cubemap", CUBE) = "" {}
		_Reflectivity ("Reflectivity power", Range(0,1)) = 0
		
		
		_RampTex ("BRDF Ramp", 2D) = "gray" {}
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_SpecIntensity ("Specular Intensity", Range(0, 10)) = 0
		_SpecTightness ("Specular Tightness", Range(1, 200)) = 48.0
    }
    
    SubShader
    {
      Tags { "Queue"="Transparent" "RenderType"="Transparent" }
      Cull Off
      CGPROGRAM
      #pragma target 4.0
      #pragma surface surf HalfLambert alpha
      
		half4 _AmbientColor;
		sampler2D _MainTex;
		sampler2D _NormalMap;
		
		float _RimSwitch;
		float4 _RimColor;
		float _RimPower;
		
		samplerCUBE _Cube;
		float _Reflectivity;

		sampler2D _RampTex;
		half4 _SpecularColor;
		float _SpecTightness;
		float _SpecIntensity;
      
		half4 LightingHalfLambert (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			//Dot product stuff
			float NdotL = dot (s.Normal, lightDir) * 0.5 + 0.5;
			float NdotV = dot (s.Normal, viewDir);
			//BRDF SECTION
			float2 brdfUV = float2(NdotV * 0.99, NdotL * 0.99);
			float3 BRDF = tex2D (_RampTex, brdfUV.xy).rgb;
			//SPECULAR SECTION
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0.5, NdotL);
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, _SpecTightness) * _SpecIntensity;
			
			//CREATE FINAL COLOR
			float4 c;
			c.rgb = (BRDF * s.Albedo * _LightColor0.rgb) * (diff + _LightColor0.rgb * spec * _SpecularColor.rgb * _SpecularColor.a * BRDF) * (atten * 2);
			c.a = s.Alpha;
			return c;
		}
      struct Input
      {
          float2 uv_MainTex;
          float2 uv_NormalMap;
          float3 viewDir;
          float3 worldRefl;
          INTERNAL_DATA
      };
      
      void surf (Input IN, inout SurfaceOutput o)
      {
      	half4 c1 = tex2D (_MainTex, IN.uv_MainTex);
		o.Albedo = c1.rgb * _AmbientColor.rgb;
		o.Alpha = c1.a;
		o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
		half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
		half3 c2 = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb;
		half3 c3 = _RimColor.rgb * pow (rim, _RimPower);
		o.Emission = (c2 * _Reflectivity) + (c3 * _RimSwitch);
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }
  
  
  
  
  
  
  
  
  