using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

[Title("Custom", "Main Light")]
public class MainLightNode : CodeFunctionNode
{
    public override bool hasPreview { get { return false; } }

    //This is the string that's passed into the code of the real, final shader code that is used in the Scene when you hit Apply in the ShaderGraph window.
    //As such, it will use real light data from the Scene (for instance, GetMainLight()).
    private static string functionBodyForReals = @"{
			Light mainLight = GetMainLight();
			Color = mainLight.color;
			Direction = mainLight.direction;
			float4 shadowCoord;
			#ifdef _SHADOWS_ENABLED
        	#if SHADOWS_SCREEN
				float4 clipPos = TransformWorldToHClip(WorldPos);
        		shadowCoord = ComputeShadowCoord(clipPos);
        	#else
        		shadowCoord = TransformWorldToShadowCoord(WorldPos);
        	#endif
			mainLight.attenuation = MainLightRealtimeShadowAttenuation(shadowCoord);
        	#endif
			Attenuation = mainLight.attenuation;
		}";

    //This string is passed to the node to generate the shader that's used in the ShaderGraph for preview.
    //Since the graph has no conception of what's the main light, we fake the data by hardcoding it in.
    private static string functionBodyPreview = @"{
			Color = 1;
			Direction = float3(-0.5, -.5, 0.5);
			Attenuation = 1;
		}";

    private static bool isPreview;

    //Returns a different string depending on whether we are in the graph or not.
    private static string functionBody
    {
        get
        {
            if (isPreview)
                return functionBodyPreview;
            else
                return functionBodyForReals;
        }
    }

    //Constructor
    public MainLightNode()
    {
        name = "Main Light";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("CustomFunction", BindingFlags.Static | BindingFlags.NonPublic);
    }

    //Will calculate the boolean isPreview which is used to decide which of the 2 strings to use (see above)
    public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
    {
        isPreview = generationMode == GenerationMode.Preview;

        base.GenerateNodeFunction(registry, graphContext, generationMode);
    }

    //The definition of the ports. 3 go out, 1 goes in
    //(which also has a default binding of WorldSpacePosition, so it doesn't need to be connected)
    private static string CustomFunction(
    [Slot(0, Binding.None)] out Vector3 Direction,
    [Slot(1, Binding.None)] out Vector1 Attenuation,
    [Slot(2, Binding.None)] out Vector3 Color,
    [Slot(3, Binding.WorldSpacePosition)] Vector3 WorldPos)
    {
        //These default values are needed otherwise Unity will complain that the Vector3s are not initialised.
        //They won't be zero in the shader.
        Direction = Vector3.zero;
        Color = Vector3.zero;

        return functionBody;
    }
}