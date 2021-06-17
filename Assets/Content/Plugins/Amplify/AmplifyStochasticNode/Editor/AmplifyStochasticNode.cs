// Amplify Stochastic Node
// Copyright (c) Jason Booth

using UnityEngine;
using UnityEditor;
using System;
#if AMPLIFY_SHADER_EDITOR

namespace AmplifyShaderEditor
{
   [Serializable]
   [NodeAttributes( "Stochastic Sample 2D", "Stochastic", "Samples a texture with stochastic sampling" )]
   public sealed class AmplifyStochasticNode : ParentNode
   {
      private readonly string[] m_channelTypeStr = { "Red Channel", "Green Channel", "Blue Channel", "Alpha Channel", "Luminosity" };
      private readonly string[] m_channelTypeVal = { "r", "g", "b", "a", "Luminosity" };
      
      [SerializeField]
      private int m_selectedChannelInt = 0;

      [SerializeField]
      private bool isNormalMap = false;


      private InputPort m_uvPort;
      private InputPort m_texPort;
      private InputPort m_contrastPort;
      private InputPort m_scalePort;
      private InputPort m_BlendWeightsPort;


      private float defaultScale = 1.0f;
      private float defaultContrast = 0.15f;

      protected override void CommonInit( int uniqueId )
      {
         base.CommonInit( uniqueId );
         AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
         AddInputPort( WirePortDataType.SAMPLER2D, true, "Tex" );
         AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
         AddInputPort(WirePortDataType.FLOAT, false, "Contrast");
         AddInputPort(WirePortDataType.FLOAT4x4, true, "Blend Weights");

         AddOutputPort( WirePortDataType.FLOAT4, "RGBA" );
         AddOutputPort(WirePortDataType.FLOAT, "R");
         AddOutputPort(WirePortDataType.FLOAT, "G");
         AddOutputPort(WirePortDataType.FLOAT, "B");
         AddOutputPort(WirePortDataType.FLOAT, "A");
         AddOutputPort(WirePortDataType.FLOAT4x4, "Blend Weights");

         m_uvPort = m_inputPorts[ 0 ];
         m_texPort = m_inputPorts[ 1 ];
         m_scalePort = m_inputPorts[ 2 ];
         m_contrastPort = m_inputPorts[3];
         m_BlendWeightsPort = m_inputPorts[4];


         m_scalePort.FloatInternalData = 1f;
         m_contrastPort.FloatInternalData = 0.15f;

         m_useInternalPortData = false;
         m_textLabelWidth = 130;
         m_autoWrapProperties = true;
         UpdateSampler();
      }

      public override void DrawProperties()
      {
         base.DrawProperties();

         EditorGUI.BeginChangeCheck();
         m_selectedChannelInt = EditorGUILayoutPopup( "Channel", m_selectedChannelInt, m_channelTypeStr );
         if ( EditorGUI.EndChangeCheck() )
         {
            UpdateSampler();
         }
         EditorGUIUtility.labelWidth = 105;


         EditorGUI.BeginChangeCheck();

         EditorGUI.BeginDisabledGroup(m_scalePort.IsConnected || m_BlendWeightsPort.IsConnected );
         defaultScale = EditorGUILayoutSlider( "Default Scale", defaultScale, 0.5f, 1.5f );
         EditorGUI.EndDisabledGroup();

         EditorGUI.BeginDisabledGroup(m_contrastPort.IsConnected || m_BlendWeightsPort.IsConnected);
         defaultContrast = EditorGUILayoutSlider("Default Contrast", defaultContrast, 0.01f, 0.99f);
         EditorGUI.EndDisabledGroup();

         isNormalMap = EditorGUILayoutToggleLeft("is Normal Map", isNormalMap);

         if (EditorGUI.EndChangeCheck())
         {
            m_sizeIsDirty = true;
         }
         if (!m_texPort.IsConnected)
         {
            EditorGUILayout.HelpBox("WARNING:\nTex must be connected to a Texture Object for this node to work\n\n", MessageType.None);
         }
      }


      private void UpdateSampler()
      {
         m_texPort.Name = "Tex (" + m_channelTypeVal[m_selectedChannelInt] + ")";
      }

      public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
      {
         if( !m_texPort.IsConnected )
         {
            UIUtils.ShowMessage( "You must connect a texture for this to work" );
            return "0";
         }


         base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );


         dataCollector.AddToIncludes(UniqueId, "Assets/AmplifyStochasticNode/StochasticSampling.cginc");



         WirePortDataType texType = WirePortDataType.SAMPLER2D;

         string textcoords = m_uvPort.GeneratePortInstructions( ref dataCollector );

         string texture = m_texPort.GenerateShaderForOutput( ref dataCollector, texType, false,true );
         string scale = defaultScale.ToString();
         if( m_scalePort.IsConnected )
            scale = m_scalePort.GeneratePortInstructions( ref dataCollector );


         string contrast = defaultContrast.ToString();
         if (m_contrastPort.IsConnected)
         {
            contrast = m_contrastPort.GeneratePortInstructions(ref dataCollector);
         }

         string localVarName = "stochasticSample" + UniqueId;
         string functionHeader = "StochasticSample2D";
         if (!m_BlendWeightsPort.IsConnected)
         {
            functionHeader += "Weights";
            switch (m_selectedChannelInt)
            {
               case 0:
                  functionHeader += "R";
                  break;
               case 1:
                  functionHeader += "G";
                  break;
               case 2:
                  functionHeader += "G";
                  break;
               case 3:
                  functionHeader += "A";
                  break;
               case 4:
                  functionHeader += "Lum";
                  break;

            }
         }

         string closing = ")";
         if (isNormalMap)
         {
            functionHeader = "half4(UnpackNormal(" + functionHeader;
            closing = ")), 1)";
         }
         if (m_BlendWeightsPort.IsConnected)
         {
            //half4 StochasticSample2DWeightsLum(sampler2D tex, float2 uv, out float3 cw, out float2 uv1, out float2 uv2, out float2 uv3, out float2 dx, out float2 dy, float scale, float contrast)

            string id = m_BlendWeightsPort.GetConnection().NodeId.ToString();
            string cw = "cw" + id;
            string uv1 = "uv1" + id;
            string uv2 = "uv2" + id;
            string uv3 = "uv3" + id;
            string dx = "dx" + id;
            string dy = "dy" + id;
            m_BlendWeightsPort.GeneratePortInstructions(ref dataCollector);
            // add locals for chaining
            dataCollector.AddLocalVariable(UniqueId, "half3", "cw" + UniqueId, cw);
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv1" + UniqueId, uv1);
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv2" + UniqueId, uv2);
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv3" + UniqueId, uv3);
            dataCollector.AddLocalVariable(UniqueId, "float2", "dx" + UniqueId, dx);
            dataCollector.AddLocalVariable(UniqueId, "float2", "dy" + UniqueId, dy);
            string funcData = string.Format("{0}({1},{2},{3},{4},{5},{6},{7}{8}", functionHeader, texture, cw, uv1, uv2, uv3, dx, dy, closing);
            dataCollector.AddLocalVariable(UniqueId, "half4 ", localVarName, funcData);
         }
         else
         {
            dataCollector.AddLocalVariable(UniqueId, "half3", "cw" + UniqueId, "0");
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv1" + UniqueId, "0");
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv2" + UniqueId, "0");
            dataCollector.AddLocalVariable(UniqueId, "float2", "uv3" + UniqueId, "0");
            dataCollector.AddLocalVariable(UniqueId, "float2", "dx" + UniqueId, "0");
            dataCollector.AddLocalVariable(UniqueId, "float2", "dy" + UniqueId, "0");

            string funcData = string.Format("{0}({1},{2},{3},{4},{5},{6},{7},{8},{9},{10}{11}", functionHeader, texture, textcoords, "cw" + UniqueId, "uv1" + UniqueId, "uv2" + UniqueId, "uv3" + UniqueId, "dx" + UniqueId, "dy" + UniqueId, scale, contrast, closing);
            dataCollector.AddLocalVariable(UniqueId, "half4", localVarName, funcData);
         }


         return GetOutputVectorItem( 0, outputId, localVarName );
      }

     
      public override void ReadFromString( ref string[] nodeParams )
      {
         base.ReadFromString( ref nodeParams );
         m_selectedChannelInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
         defaultScale = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
         defaultContrast = Convert.ToSingle(GetCurrentParam(ref nodeParams));
         isNormalMap = Convert.ToBoolean(GetCurrentParam(ref nodeParams));

         UpdateSampler();
      }

      public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
      {
         base.WriteToString( ref nodeInfo, ref connectionsInfo );

         IOUtils.AddFieldValueToString(ref nodeInfo, m_selectedChannelInt);
         IOUtils.AddFieldValueToString( ref nodeInfo, defaultScale );
         IOUtils.AddFieldValueToString(ref nodeInfo, defaultContrast); 
         IOUtils.AddFieldValueToString(ref nodeInfo, isNormalMap.ToString());


      }

      public override void Destroy()
      {
         base.Destroy();
         m_uvPort = null;
         m_texPort = null;
         m_scalePort = null;
         m_contrastPort = null;
         m_BlendWeightsPort = null;

      }
   }
}
#endif
