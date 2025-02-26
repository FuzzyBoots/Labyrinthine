Shader "Custom/MazeShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        const static int maxlayer = 12;
        sampler2D TexturesArray0;
        sampler2D TexturesArray1;
        sampler2D TexturesArray2;
        sampler2D TexturesArray3;
        sampler2D TexturesArray4;
        sampler2D TexturesArray5;
        sampler2D TexturesArray6;
        sampler2D TexturesArray7;
        sampler2D TexturesArray8;
        sampler2D TexturesArray9;
        sampler2D TexturesArray10;
        sampler2D TexturesArray11;
        float _ScalerWidth;
        float _ScalerHeight;
        float _HeightFloorTexture[maxlayer];
        float _Outline[maxlayer];
        float _HeightAnotherColor[maxlayer];
        float _CellSize[maxlayer];
        float4 _Color[maxlayer];
        float4 _ColorOutline[maxlayer];
        int _Hex;
        float heightsUser[maxlayer];

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 GetTexture(int index, float2 uv)
        {
            if (index == 0) return tex2D(TexturesArray0, uv);
            else if (index == 1) return tex2D(TexturesArray1, uv);
            else if (index == 2) return tex2D(TexturesArray2, uv);
            else if (index == 3) return tex2D(TexturesArray3, uv);
            else if (index == 4) return tex2D(TexturesArray4, uv);
            else if (index == 5) return tex2D(TexturesArray5, uv);
            else if (index == 6) return tex2D(TexturesArray6, uv);
            else if (index == 7) return tex2D(TexturesArray7, uv);
            else if (index == 8) return tex2D(TexturesArray8, uv);
            else if (index == 9) return tex2D(TexturesArray9, uv);
            else if (index == 10) return tex2D(TexturesArray10, uv);
            else if (index == 11) return tex2D(TexturesArray11, uv);

            return fixed4(1, 0, 1, 1); 
        }


        float2 RotateUV(float2 uv, float angle)
        {
            float cosA = cos(angle);
            float sinA = sin(angle);

            // Matice rotace
            float2x2 rotationMatrix = float2x2(cosA, -sinA, sinA, cosA);

            // Rotace UV
            return mul(rotationMatrix, uv);
        }


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            
            for (int i = 0; i < maxlayer; i++)
            {
                heightsUser[i] = (i + 1) * _ScalerHeight;
            }
            // Square
            if( _Hex == 0)
            {
                for (int i = maxlayer - 1; i >= 0; i--)
                {
                    float worldPosX = IN.worldPos.x + (0.5 * _ScalerWidth);
                    float worldPosYMod = fmod(IN.worldPos.y + .5 * _ScalerHeight, _ScalerHeight);
                    if (IN.worldPos.y + (1.5 * _ScalerHeight) >= heightsUser[i] 
                        && IN.worldPos.y + (1.5 * _ScalerHeight) - heightsUser[i] < _ScalerHeight)
                    {
                        if (worldPosYMod < _HeightFloorTexture[i] * _ScalerHeight)
                        {

                            float2 uvBase;

                            if (abs(IN.worldNormal.y) > 0.8)
                            {
                                uvBase = IN.worldPos.xz  / _CellSize[i];
                            }
                            else{
                                 uvBase = ( IN.worldPos.zy + IN.worldPos.xy)  / _CellSize[i];
                                 uvBase.y *= 0.5;
                            }

                            fixed4 col = GetTexture(i, frac(uvBase));

                            col.a *= 0.5;

                            o.Albedo = col.rgb;
                            o.Alpha = col.a;
                            return;

                        }

                        if (fmod((worldPosX + (_Outline[i] / 2) * _ScalerWidth) , _ScalerWidth) < _Outline[i] * _ScalerWidth &&
                            fmod(IN.worldPos.z + (_Outline[i] / 2) * _ScalerWidth, _ScalerWidth) < _Outline[i] * _ScalerWidth )
                        {
                            o.Albedo = _ColorOutline[i].rgb;
                            o.Alpha = _ColorOutline[i].a;
                            return;
                        }
                        if (worldPosYMod > _HeightAnotherColor[i] * _ScalerHeight)
                        {
                            o.Albedo = _ColorOutline[i].rgb;
                            o.Alpha = _ColorOutline[i].a;
                            return;
                        }
                        o.Albedo = _Color[i].rgb;
                        o.Alpha = _Color[i].a;
                        return;
                    }  
                }
            }
            // hex
            else{

                for (int i = maxlayer - 1; i >= 0; i--)
                {
                    float _worldPosY = IN.worldPos.y;
                    float worldPosX = IN.worldPos.x + 0.5;
                    float worldPosYMod = fmod(_worldPosY + .5 * _ScalerHeight, _ScalerHeight);
                    if (_worldPosY + (1.5 * _ScalerHeight) >= heightsUser[i] 
                        && _worldPosY + (1.5 * _ScalerHeight) - heightsUser[i] < _ScalerHeight)
                    {
                        if (worldPosYMod < _HeightFloorTexture[i] * _ScalerHeight)
                        {
                            float2 uvBase;
                            if (abs(IN.worldNormal.y) > 0.8)
                            {
                                uvBase = IN.worldPos.xz  / _CellSize[i];
                            }
                            else{
                                if (abs(IN.worldNormal.x) < 0.8)
                                {
                                            uvBase = IN.worldPos.yx / _CellSize[i];
                                    uvBase = RotateUV(uvBase, radians(90.0));

                                }
                                else  if (abs(IN.worldNormal.x) > abs(IN.worldNormal.z)) 
                                {
                                    uvBase = IN.worldPos.xy / _CellSize[i];

                                    uvBase.x *= 1.75;
                                }
                            }

                            fixed4 col = GetTexture(i, frac(uvBase));

                            col.a *= 0.5;

                            o.Albedo = col.rgb;
                            o.Alpha = col.a;
                            return;
                        }
                        float offset = fmod(IN.worldPos.x, 1.5 * _ScalerWidth);

                        if (fmod(offset + (1.25 * _ScalerWidth) + ((_Outline[i] * _ScalerWidth) / 3), 1.5 * _ScalerWidth) < (_Outline[i] * _ScalerWidth)) {
                            o.Albedo = _ColorOutline[i].rgb;
                            o.Alpha = _ColorOutline[i].a;
                            return;
                        }
                        if (fmod(offset + (.25 * _ScalerWidth) + ((_Outline[i] * _ScalerWidth) / 1.5), 1.5 * _ScalerWidth) < (_Outline[i] * _ScalerWidth)) {
                            o.Albedo = _ColorOutline[i].rgb;
                            o.Alpha = _ColorOutline[i].a;
                            return;
                        }
                        if (worldPosYMod > _HeightAnotherColor[i] * _ScalerHeight)
                        {
                            o.Albedo = _ColorOutline[i].rgb;
                            o.Alpha = _ColorOutline[i].a;
                            return;
                        }
                        o.Albedo = _Color[i].rgb;
                        o.Alpha = _Color[i].a;
                        return;
                    }  
                }
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}