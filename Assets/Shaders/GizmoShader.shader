Shader "Custom/Gizmo Shader"  
{
	Properties 
	{
		_Ambient ("Ambient Color", Color) = (1,1,1,1)
		_Emission ("Emissive Color", Color) = (0.5,0.5,0.5,1)
	}

	SubShader 
	{  
		Tags { "Queue"="Overlay" "RenderType"="Overlay" }
		Cull Back
		ZWrite Off
		ZTest Always
		Lighting On
		Pass
		{				
			Material 
			{		
				Ambient [_Ambient]
				Emission [_Emission]
			}
		}
	}
}
