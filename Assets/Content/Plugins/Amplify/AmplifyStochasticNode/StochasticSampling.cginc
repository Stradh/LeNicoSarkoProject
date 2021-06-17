

#ifdef UNITY_COLORSPACE_GAMMA
  #define stochastic_ColorSpaceLuminance half4(0.22, 0.707, 0.071, 0.0) // Legacy: alpha is set to 0.0 to specify gamma mode
#else
  #define stochastic_ColorSpaceLuminance half4(0.0396819152, 0.458021790, 0.00609653955, 1.0) // Legacy: alpha is set to 1.0 to specify linear mode
#endif

inline half StochasticLuminance(half3 rgb)
{
    return dot(rgb, stochastic_ColorSpaceLuminance.rgb);
}


// Compute local triangle barycentric coordinates and vertex IDs
void StochasticTriangleGrid(float2 uv,
   out float w1, out float w2, out float w3,
   out int2 vertex1, out int2 vertex2, out int2 vertex3, float scale)
{
   // Scaling of the input
   uv *= 3.464 * scale; // 2 * sqrt(3)

   // Skew input space into simplex triangle grid
   const float2x2 gridToSkewedGrid = float2x2(1.0, 0.0, -0.57735027, 1.15470054);
   float2 skewedCoord = mul(gridToSkewedGrid, uv);

   // Compute local triangle vertex IDs and local barycentric coordinates
   int2 baseId = int2(floor(skewedCoord));
   float3 temp = float3(frac(skewedCoord), 0);
   temp.z = 1.0 - temp.x - temp.y;
   if (temp.z > 0.0)
   {
      w1 = temp.z;
      w2 = temp.y;
      w3 = temp.x;
      vertex1 = baseId;
      vertex2 = baseId + int2(0, 1);
      vertex3 = baseId + int2(1, 0);
   }
   else
   {
      w1 = -temp.z;
      w2 = 1.0 - temp.y;
      w3 = 1.0 - temp.x;
      vertex1 = baseId + int2(1, 1);
      vertex2 = baseId + int2(1, 0);
      vertex3 = baseId + int2(0, 1);
   }
}

float2 StochasticSimpleHash2(float2 p)
{
   return frac(sin(mul(float2x2(127.1, 311.7, 269.5, 183.3), p)) * 43758.5453);
}


half3 StochasticBaryWeightBlend(half3 iWeights, half tex0, half tex1, half tex2, half contrast)
{
    // compute weight with height map
    const half epsilon = 1.0f / 1024.0f;
    half3 weights = half3(iWeights.x * (tex0 + epsilon), 
                             iWeights.y * (tex1 + epsilon),
                             iWeights.z * (tex2 + epsilon));

    // Contrast weights
    half maxWeight = max(weights.x, max(weights.y, weights.z));
    half transition = contrast * maxWeight;
    half threshold = maxWeight - transition;
    half scale = 1.0f / transition;
    weights = saturate((weights - threshold) * scale);
    // Normalize weights.
    half weightScale = 1.0f / (weights.x + weights.y + weights.z);
    weights *= weightScale;
    return weights;
}

void StochasticPrepareStochasticUVs(float2 uv, out float2 uv1, out float2 uv2, out float2 uv3, out half3 weights, float scale)
{
   // Get triangle info
   float w1, w2, w3;
   int2 vertex1, vertex2, vertex3;
   StochasticTriangleGrid(uv, w1, w2, w3, vertex1, vertex2, vertex3, scale);

   // Assign random offset to each triangle vertex
   uv1 = uv;
   uv2 = uv;
   uv3 = uv;
   
   uv1.xy += StochasticSimpleHash2(vertex1);
   uv2.xy += StochasticSimpleHash2(vertex2);
   uv3.xy += StochasticSimpleHash2(vertex3);
   weights = half3(w1, w2, w3);
   
}



half4 StochasticSample2DWeightsR(sampler2D tex, float2 uv, out half3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)
{
   half3 w;
   StochasticPrepareStochasticUVs(uv, uv1, uv2, uv3, w, scale);

   dx = ddx(uv);
   dy = ddy(uv);

   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   
   
   cw.xyz = StochasticBaryWeightBlend(w, G1.r, G2.r, G3.r, contrast);

   
    return G1 * cw.x + G2 * cw.y + G3 * cw.z;

}

half4 StochasticSample2DWeightsG(sampler2D tex, float2 uv, out half3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)
{
   half3 w;
   StochasticPrepareStochasticUVs(uv, uv1, uv2, uv3, w, scale);

   dx = ddx(uv);
   dy = ddy(uv);

   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   
   
   cw.xyz = StochasticBaryWeightBlend(w, G1.g, G2.g, G3.g, contrast);
   
   return G1 * cw.x + G2 * cw.y + G3 * cw.z;

}

half4 StochasticSample2DWeightsB(sampler2D tex, float2 uv, out half3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)
{
   half3 w;
   StochasticPrepareStochasticUVs(uv, uv1, uv2, uv3, w, scale);

   dx = ddx(uv);
   dy = ddy(uv);

   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   
   
   cw.xyz = StochasticBaryWeightBlend(w, G1.b, G2.b, G3.b, contrast);

   
   return G1 * cw.x + G2 * cw.y + G3 * cw.z;

}

half4 StochasticSample2DWeightsA(sampler2D tex, float2 uv, out half3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)
{
   half3 w;
   StochasticPrepareStochasticUVs(uv, uv1, uv2, uv3, w, scale);

   dx = ddx(uv);
   dy = ddy(uv);

   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   
   
   cw.xyz = StochasticBaryWeightBlend(w, G1.a, G2.a, G3.a, contrast);

   
   return G1 * cw.x + G2 * cw.y + G3 * cw.z;

}

half4 StochasticSample2DWeightsLum(sampler2D tex, float2 uv, out half3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)
{
   half3 w;
   StochasticPrepareStochasticUVs(uv, uv1, uv2, uv3, w, scale);

   dx = ddx(uv);
   dy = ddy(uv);

   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   
   cw.xyz = StochasticBaryWeightBlend(w, StochasticLuminance(G1.rgb), StochasticLuminance(G2.rgb), StochasticLuminance(G3.rgb), contrast);
   
   return G1 * cw.x + G2 * cw.y + G3 * cw.z;
}

half4 StochasticSample2D(sampler2D tex, half3 cw, float2 uv1, float2 uv2, float2 uv3, float2 dx, float2 dy)
{
   float4 G1 = tex2Dgrad(tex, uv1, dx, dy);
   float4 G2 = tex2Dgrad(tex, uv2, dx, dy);
   float4 G3 = tex2Dgrad(tex, uv3, dx, dy);
   return G1 * cw.x + G2 * cw.y + G3 * cw.z;

}



