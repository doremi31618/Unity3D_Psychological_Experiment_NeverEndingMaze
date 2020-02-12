// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BOXOPHOBIC/Atmospherics/Height Fog Per Object"
{
	Properties
	{
		[HideInInspector]_HeightFogPerObject("_HeightFogPerObject", Float) = 1
		[HideInInspector]_IsStandardPipeline("_IsStandardPipeline", Float) = 0
		[HideInInspector]_IsHeightFogShader("_IsHeightFogShader", Float) = 1
		[HideInInspector]_TransparentQueue("_TransparentQueue", Int) = 3000
		[StyledBanner(Height Fog Per Object)]_TITLEE("< TITLEE >", Float) = 1
		[BCategory(Custom Alpha Inputs)]_CUSTOM("[ CUSTOM ]", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha , One One
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityStandardBRDF.cginc"
			#pragma multi_compile AHF_DIRECTIONALMODE_OFF AHF_DIRECTIONALMODE_ON
			#pragma multi_compile AHF_NOISEMODE_OFF AHF_NOISEMODE_PROCEDURAL3D


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			//This is a late directive
			
			uniform half _IsHeightFogShader;
			uniform half _TITLEE;
			uniform half _HeightFogPerObject;
			uniform int _TransparentQueue;
			uniform half _CUSTOM;
			uniform half4 AHF_FogColor;
			uniform half4 AHF_DirectionalColor;
			uniform half AHF_DirectionalIntensity;
			uniform half AHF_DirectionalModeBlend;
			uniform half AHF_FogDistanceStart;
			uniform half AHF_FogDistanceEnd;
			uniform half AHF_FogHeightEnd;
			uniform half AHF_FogHeightStart;
			uniform half AHF_NoiseScale;
			uniform half3 AHF_NoiseSpeed;
			uniform half AHF_NoiseDistanceEnd;
			uniform half AHF_NoiseIntensity;
			uniform half AHF_NoiseModeBlend;
			uniform half AHF_FogIntensity;
			uniform half4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform half _IsStandardPipeline;
			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				int CustomVertexOffset918 = 0;
				float3 temp_cast_0 = CustomVertexOffset918;
				
				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord.xyz = ase_worldPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = temp_cast_0;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float3 temp_output_2_0_g578 = (AHF_FogColor).rgb;
				float3 gammaToLinear3_g578 = GammaToLinearSpace( temp_output_2_0_g578 );
				#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch1_g578 = temp_output_2_0_g578;
				#else
				float3 staticSwitch1_g578 = gammaToLinear3_g578;
				#endif
				float3 temp_output_924_0 = staticSwitch1_g578;
				float3 temp_output_2_0_g577 = (AHF_DirectionalColor).rgb;
				float3 gammaToLinear3_g577 = GammaToLinearSpace( temp_output_2_0_g577 );
				#ifdef UNITY_COLORSPACE_GAMMA
				float3 staticSwitch1_g577 = temp_output_2_0_g577;
				#else
				float3 staticSwitch1_g577 = gammaToLinear3_g577;
				#endif
				float3 ase_worldPos = i.ase_texcoord.xyz;
				float3 WorldPosition144 = ase_worldPos;
				float3 normalizeResult5_g572 = normalize( ( WorldPosition144 - _WorldSpaceCameraPos ) );
				float3 worldSpaceLightDir = Unity_SafeNormalize(UnityWorldSpaceLightDir(ase_worldPos));
				float dotResult6_g572 = dot( normalizeResult5_g572 , worldSpaceLightDir );
				float temp_output_7_0_g573 = -1.0;
				half DirectionalMask134 = ( ( ( dotResult6_g572 - temp_output_7_0_g573 ) / ( 1.0 - temp_output_7_0_g573 ) ) * AHF_DirectionalIntensity * AHF_DirectionalModeBlend );
				float3 lerpResult135 = lerp( temp_output_924_0 , staticSwitch1_g577 , DirectionalMask134);
				#if defined(AHF_DIRECTIONALMODE_OFF)
				float3 staticSwitch241 = temp_output_924_0;
				#elif defined(AHF_DIRECTIONALMODE_ON)
				float3 staticSwitch241 = lerpResult135;
				#else
				float3 staticSwitch241 = temp_output_924_0;
				#endif
				float temp_output_7_0_g563 = AHF_FogDistanceStart;
				half FogDistanceMask186 = saturate( ( ( distance( WorldPosition144 , _WorldSpaceCameraPos ) - temp_output_7_0_g563 ) / ( AHF_FogDistanceEnd - temp_output_7_0_g563 ) ) );
				float temp_output_7_0_g565 = AHF_FogHeightEnd;
				half FogHeightMask193 = saturate( ( ( (WorldPosition144).y - temp_output_7_0_g565 ) / ( AHF_FogHeightStart - temp_output_7_0_g565 ) ) );
				float temp_output_115_0 = ( FogDistanceMask186 * FogHeightMask193 );
				float simplePerlin3D15_g566 = snoise( ( ( WorldPosition144 * ( 1.0 / AHF_NoiseScale ) ) + ( -AHF_NoiseSpeed * _Time.y ) ) );
				float temp_output_7_0_g569 = -1.0;
				float temp_output_7_0_g560 = AHF_NoiseDistanceEnd;
				half NoiseDistanceMask353 = saturate( ( ( distance( WorldPosition144 , _WorldSpaceCameraPos ) - temp_output_7_0_g560 ) / ( 0.0 - temp_output_7_0_g560 ) ) );
				float lerpResult20_g566 = lerp( 1.0 , ( ( simplePerlin3D15_g566 - temp_output_7_0_g569 ) / ( 1.0 - temp_output_7_0_g569 ) ) , ( NoiseDistanceMask353 * AHF_NoiseIntensity * AHF_NoiseModeBlend ));
				half NoiseSimplex3D234 = lerpResult20_g566;
				#if defined(AHF_NOISEMODE_OFF)
				float staticSwitch242 = temp_output_115_0;
				#elif defined(AHF_NOISEMODE_PROCEDURAL3D)
				float staticSwitch242 = ( temp_output_115_0 * NoiseSimplex3D234 );
				#else
				float staticSwitch242 = temp_output_115_0;
				#endif
				float2 uv0_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float temp_output_1_0_g576 = tex2D( _MainTex, uv0_MainTex ).a;
				float lerpResult3_g576 = lerp( temp_output_1_0_g576 , ceil( temp_output_1_0_g576 ) , FogDistanceMask186);
				half CustomAlphaInputs897 = ( _Color.a * lerpResult3_g576 );
				float4 appendResult384 = (float4(staticSwitch241 , ( staticSwitch242 * AHF_FogIntensity * CustomAlphaInputs897 )));
				
				
				finalColor = ( appendResult384 + ( _IsStandardPipeline * 0.0 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	
	
	
}
/*ASEBEGIN
Version=17602
1927;7;1906;1014;2548.758;5254.893;1;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;885;-3328,-1664;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-2496,-1664;Float;False;WorldPosition;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;626;-3328,-128;Inherit;False;144;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;573;-3328,-896;Inherit;False;144;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;929;-3072,-128;Inherit;False;Fog Noise Distance;-1;;559;b49331aa561bee84c8546167a63b6b58;0;1;18;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;927;-3072,-896;Inherit;False;Fog Distance;-1;;562;a5f090963b8f9394a984ee752ce42488;0;1;13;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;353;-2528,-128;Half;False;NoiseDistanceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;189;-3328,-512;Inherit;False;144;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;217;-3328,256;Inherit;False;144;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;931;-3328,336;Inherit;False;353;NoiseDistanceMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;895;-3328,-3328;Inherit;False;0;892;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;186;-2528,-896;Half;False;FogDistanceMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;928;-3072,-512;Inherit;False;Fog Height;-1;;564;61a3ca69e0854664d87b5233acc9cbae;0;1;8;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;930;-3008,256;Inherit;False;Fog Noise;-1;;566;fe51dc291bbca2147aa47de2202288d1;0;2;22;FLOAT3;0,0,0;False;23;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;179;-3328,-1280;Inherit;False;144;WorldPosition;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;-2512,-512;Half;False;FogHeightMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;892;-3072,-3328;Inherit;True;Property;_MainTex;MainTex;10;0;Create;False;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;MipBias;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;902;-3008,-3136;Inherit;False;186;FogDistanceMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;-3328,-4224;Inherit;False;186;FogDistanceMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;926;-3072,-1280;Inherit;False;Fog Directional;-1;;572;b76195d9ca3254b49ac2428d15088cd6;0;1;15;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-3328,-4144;Inherit;False;193;FogHeightMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;913;-2688,-3328;Inherit;False;Handle Tex Alpha;-1;;576;92f31391e7f50294c9c2d8747c81d6b6;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;102;-3328,-4544;Half;False;Global;AHF_DirectionalColor;AHF_DirectionalColor;12;0;Create;False;0;0;False;0;1,0.6300203,0.1617647,0;1,0.5318966,0.25,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;234;-2512,256;Half;False;NoiseSimplex3D;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;894;-3328,-3584;Half;False;Property;_Color;Color;9;0;Create;False;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;137;-3328,-4736;Half;False;Global;AHF_FogColor;AHF_FogColor;4;0;Create;False;0;0;False;0;0.4411765,0.722515,1,0;0,0.5827588,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;896;-2304,-3584;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;250;-3328,-4064;Inherit;False;234;NoiseSimplex3D;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-3072,-4224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-2512,-1280;Half;False;DirectionalMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;863;-3072,-4544;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;862;-3072,-4736;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;925;-2880,-4544;Inherit;False;Handle Color Space;-1;;577;f6f44b689bae74d47a0885dbe3018c48;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;924;-2880,-4736;Inherit;False;Handle Color Space;-1;;578;f6f44b689bae74d47a0885dbe3018c48;0;1;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-3328,-4352;Inherit;False;134;DirectionalMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;897;-2144,-3584;Half;False;CustomAlphaInputs;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;276;-2880,-4096;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;898;-2432,-4032;Inherit;False;897;CustomAlphaInputs;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;278;-2432,-4128;Half;False;Global;AHF_FogIntensity;AHF_FogIntensity;3;1;[HideInInspector];Create;False;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;135;-2368,-4608;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;242;-2688,-4224;Float;False;Property;AHF_NOISEMODE;AHF_NoiseMode;14;0;Create;False;0;0;False;0;1;0;0;False;;KeywordEnum;2;_OFF;_PROCEDURAL3D;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;277;-2048,-4224;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;241;-2176,-4736;Float;False;Property;AHF_DIRECTIONALMODE;AHF_DirectionalMode;11;0;Create;False;0;0;False;0;1;0;0;False;;KeywordEnum;2;_OFF;_ON;Create;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;919;-3328,-2688;Float;False;Constant;_Int0;Int 0;5;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;935;-1600,-4608;Inherit;False;Is Pipeline;1;;579;6a59a34c4be5db64ca90ee69227573b8;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;384;-1792,-4736;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;918;-3136,-2688;Half;False;CustomVertexOffset;-1;True;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.IntNode;922;-2816,-5120;Float;False;Property;_TransparentQueue;_TransparentQueue;6;1;[HideInInspector];Create;False;0;0;True;0;3000;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;934;-1408,-4736;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;920;-1664,-4352;Inherit;False;918;CustomVertexOffset;1;0;OBJECT;0;False;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;879;-3328,-5120;Half;False;Property;_HeightFogPerObject;_HeightFogPerObject;0;1;[HideInInspector];Create;False;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;899;-3169,-5248;Half;False;Property;_CUSTOM;[ CUSTOM ];8;0;Create;True;0;0;True;1;BCategory(Custom Alpha Inputs);1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;890;-3072,-5120;Half;False;Property;_IsHeightFogShader;_IsHeightFogShader;5;1;[HideInInspector];Create;False;0;0;True;0;1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;923;-3328,-5248;Half;False;Property;_TITLEE;< TITLEE >;7;0;Create;True;0;0;True;1;StyledBanner(Height Fog Per Object);1;1;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;383;-1152,-4736;Float;False;True;-1;2;;0;1;BOXOPHOBIC/Atmospherics/Height Fog Per Object;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;4;1;False;-1;1;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;594;True;0;False;595;True;False;10;True;890;10;True;890;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;0
Node;AmplifyShaderEditor.CommentaryNode;354;-3328,-256;Inherit;False;1022.526;100;Noise Distance Mask;0;;0.7529412,1,0.7529412,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;196;-3328,-640;Inherit;False;1028.158;100;Fog Height;0;;0,0.5882353,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;180;-3328,-1408;Inherit;False;1023.152;100;Directional Light Support;0;;1,0.634,0.1617647,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;880;-3328,-5376;Inherit;False;895.4729;100;Drawers;0;;1,0.475862,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;235;-3328,128;Inherit;False;1022.805;100;Noise;0;;0.7529412,1,0.7529412,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;190;-3328,-1024;Inherit;False;1023.127;100;Fog Distance;0;;0,0.5882353,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;891;-3328,-3712;Inherit;False;2043.196;100;Custom Alpha Inputs / Add here your custom Alpha Inputs;0;;0.684,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;916;-3328,-2816;Inherit;False;2045.196;100;Custom Vertex Offset / Add here your custom Vertex Offset;0;;0.684,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;174;-3328,-1792;Inherit;False;1024.716;100;World Position;0;;0,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;612;-3328,-4864;Inherit;False;2363.196;100;Final Pass;0;;0.684,1,0,1;0;0
WireConnection;144;0;885;0
WireConnection;929;18;626;0
WireConnection;927;13;573;0
WireConnection;353;0;929;0
WireConnection;186;0;927;0
WireConnection;928;8;189;0
WireConnection;930;22;217;0
WireConnection;930;23;931;0
WireConnection;193;0;928;0
WireConnection;892;1;895;0
WireConnection;926;15;179;0
WireConnection;913;1;892;4
WireConnection;913;2;902;0
WireConnection;234;0;930;0
WireConnection;896;0;894;4
WireConnection;896;1;913;0
WireConnection;115;0;188;0
WireConnection;115;1;194;0
WireConnection;134;0;926;0
WireConnection;863;0;102;0
WireConnection;862;0;137;0
WireConnection;925;2;863;0
WireConnection;924;2;862;0
WireConnection;897;0;896;0
WireConnection;276;0;115;0
WireConnection;276;1;250;0
WireConnection;135;0;924;0
WireConnection;135;1;925;0
WireConnection;135;2;136;0
WireConnection;242;1;115;0
WireConnection;242;0;276;0
WireConnection;277;0;242;0
WireConnection;277;1;278;0
WireConnection;277;2;898;0
WireConnection;241;1;924;0
WireConnection;241;0;135;0
WireConnection;384;0;241;0
WireConnection;384;3;277;0
WireConnection;918;0;919;0
WireConnection;934;0;384;0
WireConnection;934;1;935;0
WireConnection;383;0;934;0
WireConnection;383;1;920;0
ASEEND*/
//CHKSM=67962B96AD767516B280A566316413A2B2145561