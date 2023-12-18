Shader "Hidden/Com/Amequus/Levers/FillComplexPolygon"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #define MAX_VERTICES 512
            float2 _polygonVertices[MAX_VERTICES];
            uint _vertexCount; // Actual number of vertices in the polygon

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float2 DistancePointLineSegment(float2 pnt, float2 lineStart, float2 lineEnd) {
                float2 lineVec = lineEnd - lineStart;
                float2 pointVec = pnt - lineStart;
                
                float projectionLength = dot(pointVec, lineVec) / dot(lineVec, lineVec);
                float2 projection = lineStart + projectionLength * lineVec;

                if (projectionLength >= 0.0 && projectionLength <= 1.0) {
                    // Projection is on the segment
                    return length(pnt - projection);
                } else {
                    // Projection is not on the segment, return distance to the closest endpoint
                    return min(length(pnt - lineStart), length(pnt - lineEnd));
                }
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                float2 pnt = IN.worldPosition.xy;

                uint intersections = 0;

                float distanceToLine = 3.4028235e+38;

                for (uint i = 0; i < _vertexCount; i+=2)
                {
                    // Get the current vertex and the next vertex (wrap around at the end).
                    float2 vertexA = _polygonVertices[i];
                    float2 vertexB = _polygonVertices[i + 1];

                    // Check if the line from this vertex to the next crosses the horizontal line from the test point.
                    bool inVerticalRange = (vertexA.y > pnt.y) != (vertexB.y > pnt.y);
                    if (inVerticalRange &&
                        (pnt.x < (vertexB.x - vertexA.x) * (pnt.y - vertexA.y) / (vertexB.y - vertexA.y) + vertexA.x))
                    {
                        intersections++;
                    }
                    distanceToLine = min(distanceToLine, DistancePointLineSegment(pnt, vertexA, vertexB));
                }

                // If the number of intersections is odd, the point is inside the polygon.
                bool inside = (intersections % 2) != 0;

                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color.a *= smoothstep(0, 1, distanceToLine);

                if(!inside) {
                    color.a = 0.0;
                }
                
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
