// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SNB_Nature/SNB_Foliage_Deferred"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_EmissionRMetallicGSmoothnessB("Emission (R), Metallic (G), Smoothness (B)", 2D) = "black" {}
		_Cutoff( "Mask Clip Value", Float ) = 0.75
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Float) = 1
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 5)) = 1
		_WindFoliageAmplitude("Wind Foliage Amplitude", Range( 0 , 1)) = 0
		_WindFoliageSpeed("Wind Foliage Speed", Range( 0 , 1)) = 0
		_WindTrunkAmplitude("Wind Trunk Amplitude", Range( 0 , 1)) = 0
		_WindTrunkSpeed("Wind Trunk Speed", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#pragma instancing_options procedural:setup
		#pragma multi_compile GPU_FRUSTUM_ON__
		#include "VS_indirect.cginc"
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _WindTrunkSpeed;
		uniform float _WindTrunkAmplitude;
		uniform float _WindFoliageSpeed;
		uniform float _WindFoliageAmplitude;
		uniform float _NormalStrength;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _EmissionRMetallicGSmoothnessB;
		uniform float4 _EmissionRMetallicGSmoothnessB_ST;
		uniform float _SmoothnessStrength;
		uniform float _Cutoff = 0.75;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_130_0 = ( _Time.y * ( 2.0 * _WindTrunkSpeed ) );
			float4 appendResult141 = (float4(( ( sin( temp_output_130_0 ) * _WindTrunkAmplitude ) * v.color.b ) , 0.0 , ( v.color.b * ( ( _WindTrunkAmplitude * 0.5 ) * cos( temp_output_130_0 ) ) ) , 0.0));
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult204 = (float4(ase_worldPos.x , ase_worldPos.y , ase_worldPos.z , 0.0));
			float2 panner93 = ( ( _Time.y * _WindFoliageSpeed ) * float2( 2,2 ) + appendResult204.xy);
			float simplePerlin2D101 = snoise( panner93 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( appendResult141 + float4( ( simplePerlin2D101 * _WindFoliageAmplitude * ase_vertexNormal * v.color.r ) , 0.0 ) ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalStrength );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode36 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode36.rgb;
			float2 uv_EmissionRMetallicGSmoothnessB = i.uv_texcoord * _EmissionRMetallicGSmoothnessB_ST.xy + _EmissionRMetallicGSmoothnessB_ST.zw;
			o.Smoothness = saturate( ( tex2D( _EmissionRMetallicGSmoothnessB, uv_EmissionRMetallicGSmoothnessB ).b * _SmoothnessStrength ) );
			o.Alpha = 1;
			clip( tex2DNode36.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
-1920;47;1906;963;2905.767;1308.784;2.31053;True;True
Node;AmplifyShaderEditor.CommentaryNode;144;-2741.2,967.8284;Float;False;1821.23;666.407;Vertex offset using Blue Vertex Color channel;14;140;141;137;135;134;136;138;139;133;130;143;131;129;148;Wind Trunk;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;131;-2752.534,1253.118;Float;False;Property;_WindTrunkSpeed;Wind Trunk Speed;12;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-2465.337,1233.448;Float;False;2;2;0;FLOAT;2;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2736.908,1722.336;Float;False;1797.273;852.4986;Vertex offset using Red Vertex Color channel base on panning noise;11;97;204;203;101;99;132;98;93;95;94;96;Wind Foliage;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;129;-2650.948,1040.889;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;203;-2579.08,1863.01;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;96;-2467.55,2460.16;Float;False;Property;_WindFoliageSpeed;Wind Foliage Speed;10;0;Create;True;0;0;False;0;0;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;94;-2499.761,2290.74;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-2187.593,1232.33;Float;False;Property;_WindTrunkAmplitude;Wind Trunk Amplitude;11;0;Create;True;0;0;False;0;0;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;130;-2310.304,1109.096;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;204;-2294.56,1876.072;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;139;-1921.291,1277.47;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;133;-2021.105,1103.079;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;140;-2137.852,1457.162;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2160.924,2319.374;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;136;-1642.718,1198.159;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;146;-2117.948,-977.7754;Float;False;1272.888;652.4158;;4;104;106;36;105;Base Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-1840.604,1074.578;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1772.326,1419.792;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;93;-1975.992,1875.543;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,2;False;1;FLOAT;0.1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-1607.789,2222.813;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-1399.146,1088.823;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;206;-691.6966,-298.315;Float;False;Property;_SmoothnessStrength;Smoothness Strength;5;0;Create;True;0;0;False;0;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;106;-1834.936,-750.888;Float;True;Property;_EmissionRMetallicGSmoothnessB;Emission (R), Metallic (G), Smoothness (B);1;0;Create;True;0;0;False;0;None;b40e4f8e896e21344a3d5ca84d541ca5;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;132;-1607.833,2379.677;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;98;-1616.121,2123.507;Float;False;Property;_WindFoliageAmplitude;Wind Foliage Amplitude;9;0;Create;True;0;0;False;0;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;101;-1590.079,1867.231;Float;True;Simplex2D;1;0;FLOAT2;1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-1446.229,1433.917;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;-1099.605,1343.745;Float;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-2044.889,-869.4987;Float;False;Property;_NormalStrength;Normal Strength;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;205;-329.0202,-310.3469;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1135.6,2100.149;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;155;-2172.946,605.8249;Float;False;748.0724;269.9816;Light attenuation required for Point Lights;3;175;169;163;Translucency Point Lights;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;159;-2164.882,108.0065;Float;False;991.063;415.0306;Adjustable Light Attenuation (directional light shadow tweaking);7;188;178;176;173;172;170;160;Translucency Directional Lights (shadow control);1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;157;-2155.309,-222.3481;Float;False;607.4456;256.4624;Outputs 0 for Point, 1 for Dir;2;190;174;Is Point Light ?;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;158;-2982.524,21.2605;Float;False;718.8501;473.0243;Useful tweakings for round objects with spherical normals;5;189;186;177;171;162;Translucency Power;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;156;-4046.946,-53.0321;Float;False;925.5469;667.5338;Based on Edward del Villar free tutorial;8;187;179;168;167;166;165;164;161;Translucency Base;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;154;-1164.76,-196.0675;Float;False;675.6133;476.7519;;3;192;191;183;Translucency Control;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;192;-1125.997,35.0799;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;173;-2021.867,405.8529;Float;False;174;isPointLight;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-1806.313,-105.0114;Float;False;isPointLight;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;187;-3291.188,48.37326;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;177;-2630.074,190.6458;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-2750.976,339.8462;Float;False;Constant;_TranslucencyScale;Translucency Scale;11;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;178;-1412.469,139.3351;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;-1569.202,147.4384;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-1824.937,-545.4274;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;fcbd578e80a32e8469fe6fecd607d8aa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;166;-4008.671,172.3153;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;169;-2151.307,696.2078;Float;False;Property;_PointLightTranslucency;Point Light Translucency;8;0;Create;True;0;0;False;0;1;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;207;-119.3212,-284.5642;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-1676.944,696.2086;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;176;-1337.907,366.7979;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;-1121.379,203.2011;Float;False;Property;_TranslucencyForce;Translucency Force;6;0;Create;True;0;0;False;0;0.4;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;167;-3406.412,281.9287;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;179;-4003.836,-3.533116;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;104;-1830.615,-929.0303;Float;True;Property;_NormalMap;Normal Map;3;1;[Normal];Create;True;0;0;False;0;None;fc84c738c0737df418cb63cacdcb9f84;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;160;-1733.657,275.5548;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;165;-4007.473,330.4934;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;-353.0079,380.2104;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-1075.481,380.9782;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;168;-4011.717,505.0903;Float;False;Constant;_TranslucencyModifier;Translucency Modifier;8;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;-907.9036,377.9584;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;-3531.263,272.5973;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-55.07156,-7.667254;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;-3694.104,417.5468;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;163;-2146.864,766.9682;Float;False;174;isPointLight;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;170;-1785.276,411.2484;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;162;-2495.428,196.5452;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;190;-2082.49,-113.994;Float;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;186;-2926.648,336.2466;Float;False;Constant;_TranslucencyPower;Translucency Power;9;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-641.9191,-73.25418;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;142;-113.758,1364.8;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;189;-2806.876,189.3459;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;188;-2116.789,240.1963;Float;False;Property;_DirectionalShadows;Directional Shadows;7;0;Create;True;0;0;False;0;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;353.0061,-358.2258;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;SNB_Nature/SNB_Foliage_Deferred;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;7;Custom;0.75;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;4;Pragma;multi_compile _ LOD_FADE_CROSSFADE;False;;Pragma;instancing_options procedural:setup;False;;Pragma;multi_compile GPU_FRUSTUM_ON__;False;;Include;VS_indirect.cginc;False;;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;148;1;131;0
WireConnection;130;0;129;0
WireConnection;130;1;148;0
WireConnection;204;0;203;1
WireConnection;204;1;203;2
WireConnection;204;2;203;3
WireConnection;139;0;143;0
WireConnection;133;0;130;0
WireConnection;140;0;130;0
WireConnection;95;0;94;0
WireConnection;95;1;96;0
WireConnection;134;0;133;0
WireConnection;134;1;143;0
WireConnection;138;0;139;0
WireConnection;138;1;140;0
WireConnection;93;0;204;0
WireConnection;93;1;95;0
WireConnection;135;0;134;0
WireConnection;135;1;136;3
WireConnection;101;0;93;0
WireConnection;137;0;136;3
WireConnection;137;1;138;0
WireConnection;141;0;135;0
WireConnection;141;2;137;0
WireConnection;205;0;106;3
WireConnection;205;1;206;0
WireConnection;99;0;101;0
WireConnection;99;1;98;0
WireConnection;99;2;97;0
WireConnection;99;3;132;1
WireConnection;174;0;190;2
WireConnection;187;0;179;0
WireConnection;187;1;167;0
WireConnection;177;0;189;0
WireConnection;177;1;171;0
WireConnection;178;0;172;0
WireConnection;172;1;160;0
WireConnection;207;0;205;0
WireConnection;175;0;162;0
WireConnection;175;1;169;0
WireConnection;175;2;163;0
WireConnection;176;0;162;0
WireConnection;176;1;178;0
WireConnection;176;2;170;0
WireConnection;167;0;166;0
WireConnection;104;5;105;0
WireConnection;160;0;188;0
WireConnection;182;0;181;0
WireConnection;182;1;191;0
WireConnection;185;0;176;0
WireConnection;185;1;175;0
WireConnection;181;0;185;0
WireConnection;164;0;166;0
WireConnection;164;1;161;0
WireConnection;180;1;182;0
WireConnection;161;0;165;0
WireConnection;161;1;168;0
WireConnection;170;0;173;0
WireConnection;162;0;177;0
WireConnection;191;0;36;0
WireConnection;191;1;192;0
WireConnection;191;2;183;0
WireConnection;142;0;141;0
WireConnection;142;1;99;0
WireConnection;189;0;187;0
WireConnection;189;1;186;0
WireConnection;0;0;36;0
WireConnection;0;1;104;0
WireConnection;0;4;207;0
WireConnection;0;10;36;4
WireConnection;0;11;142;0
ASEEND*/
//CHKSM=36DFDCDBB8488C951C4A150167FE0FB93BB22585