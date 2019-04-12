// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

//Enable this to display the default Inspector (in case the custom Inspector is broken)
//#define SHOW_DEFAULT_INSPECTOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Custom material inspector for generated shader

public class TCP2_MaterialInspector_LWRP_SG : BaseShaderGUI
{
	//Properties
	private Material targetMaterial { get { return (materialEditor == null) ? null : materialEditor.target as Material; } }
	private Stack<bool> toggledGroups = new Stack<bool>();
	bool firstTimeApply;

	//Lightweight properties
	private MaterialProperty culling;
	private MaterialProperty alphaClip;
	private MaterialProperty alphaThresholdProp;

	private static class Styles
	{
		public static GUIContent twoSidedLabel = new GUIContent("Two Sided", "Render front and back faces");
		public static GUIContent alphaClipLabel = new GUIContent("Alpha Clip", "Enable Alpha Clip");

		public static readonly string[] surfaceNames = System.Enum.GetNames(typeof(SurfaceType));
		public static readonly string[] blendNames = System.Enum.GetNames(typeof(BlendMode));

		public static string surfaceTypeLabel = "Surface Type";
		public static string blendingModeLabel = "Blending Mode";
		public static string clipThresholdLabel = "Threshold";
	}

	//--------------------------------------------------------------------------------------------------

	public override void FindProperties(MaterialProperty[] properties)
	{
		surfaceTypeProp = FindProperty("_Surface", properties);
		blendModeProp = FindProperty("_Blend", properties);
		culling = FindProperty("_Cull", properties);
		alphaClip  = FindProperty("_AlphaClip", properties);
		alphaThresholdProp = FindProperty("_Cutoff", properties);
	}

	public override void MaterialChanged(Material material)
	{
		SetupMaterialBlendMode(material);

		// A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
		// or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
		// The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
		if (material.HasProperty("_EmissionColor"))
		{
			MaterialEditor.FixupEmissiveFlag(material);
		}
	}

	private void DoSurfaceArea()
	{
		int surfaceTypeValue = (int)surfaceTypeProp.floatValue;
		EditorGUI.BeginChangeCheck();
		surfaceTypeValue = EditorGUILayout.Popup(Styles.surfaceTypeLabel, surfaceTypeValue, Styles.surfaceNames);
		if(EditorGUI.EndChangeCheck())
			surfaceTypeProp.floatValue = surfaceTypeValue;

		if((SurfaceType)surfaceTypeValue == SurfaceType.Transparent)
		{
			int blendModeValue = (int)blendModeProp.floatValue;
			EditorGUI.BeginChangeCheck();
			blendModeValue = EditorGUILayout.Popup(Styles.blendingModeLabel, blendModeValue, Styles.blendNames);
			if(EditorGUI.EndChangeCheck())
				blendModeProp.floatValue = blendModeValue;
		}

		EditorGUI.BeginChangeCheck();
		bool twoSidedEnabled = EditorGUILayout.Toggle(Styles.twoSidedLabel, culling.floatValue == 0);
		if(EditorGUI.EndChangeCheck())
			culling.floatValue = twoSidedEnabled ? 0 : 2;

		EditorGUI.BeginChangeCheck();
		bool alphaClipEnabled = (alphaClip.floatValue == 1);
		if(alphaClipEnabled)
		{
			var rect = EditorGUILayout.GetControlRect(true);
			var rightRect = rect;

			rect.width = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;
			//30 = small added margin, because the toggle checkbox doesn't fill the whole field area
			rightRect.width = (rightRect.width - rect.width) + 30;
			rightRect.x += rect.width - 30;

			var labelRect = rightRect;
			labelRect.width = 70;
			rightRect.width -= labelRect.width;
			rightRect.x += labelRect.width;

			alphaClipEnabled = EditorGUI.Toggle(rect, Styles.alphaClipLabel, alphaClip.floatValue == 1);
			GUI.Label(labelRect, Styles.clipThresholdLabel);
			alphaThresholdProp.floatValue = EditorGUI.Slider(rightRect, alphaThresholdProp.floatValue, 0f, 1f);
			//m_MaterialEditor.ShaderProperty(rightRect, alphaThresholdProp, GUIContent.none, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
		}
		else
			alphaClipEnabled = EditorGUILayout.Toggle(Styles.alphaClipLabel, alphaClip.floatValue == 1);
		if (EditorGUI.EndChangeCheck())
		{
			alphaClip.floatValue = alphaClipEnabled ? 1 : 0;
		}

		EditorGUILayout.Space();
	}

	public override void ShaderPropertiesGUI(Material material)
	{
		EditorGUI.BeginChangeCheck();
		{
			DoSurfaceArea();
		}
		if(EditorGUI.EndChangeCheck())
		{
			foreach(var obj in blendModeProp.targets)
				MaterialChanged((Material)obj);
		}
	}

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		FindProperties(properties); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
		this.materialEditor = materialEditor;
		Material material = materialEditor.target as Material;

		// Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
		// material to a lightweight shader.
		if(firstTimeApply)
		{
			MaterialChanged(material);
			firstTimeApply = false;
		}

	#if SHOW_DEFAULT_INSPECTOR
		base.OnGUI();
		return;
	#else

		//Header
		EditorGUILayout.BeginHorizontal();
		string label = (Screen.width > 450f) ? "TOONY COLORS PRO 2 - INSPECTOR (Generated Shader)" : (Screen.width > 300f ? "TOONY COLORS PRO 2 - INSPECTOR" : "TOONY COLORS PRO 2");
		TCP2_GUI.HeaderBig(label);
		if(TCP2_GUI.Button(TCP2_GUI.CogIcon, "O", "Open in Shader Generator"))
		{
			if(targetMaterial.shader != null)
			{
				TCP2_ShaderGenerator.OpenWithShader(targetMaterial.shader);
			}
		}
		EditorGUILayout.EndHorizontal();
		TCP2_GUI.Separator();

		//Iterate Shader properties

		// - Lightweight properties (Surface type, Blend mode)
		ShaderPropertiesGUI(material);

		// - Generated Shader/TCP2 properties
		materialEditor.serializedObject.Update();
		SerializedProperty mShader = materialEditor.serializedObject.FindProperty("m_Shader");
		toggledGroups.Clear();
		if(materialEditor.isVisible && !mShader.hasMultipleDifferentValues && mShader.objectReferenceValue != null)
		{
			//Retina display fix
			EditorGUIUtility.labelWidth = TCP2_Utils.ScreenWidthRetina - 120f;
			EditorGUIUtility.fieldWidth = 64f;

			EditorGUI.BeginChangeCheck();

			foreach (MaterialProperty p in properties)
			{
				bool visible = (toggledGroups.Count == 0 || toggledGroups.Peek());

				//Hacky way to separate material inspector properties into foldout groups
				if(p.name.StartsWith("__BeginGroup"))
				{
					//Foldout
					if(visible)
					{
						GUILayout.Space(8f);
						p.floatValue = EditorGUILayout.Foldout(p.floatValue > 0, p.displayName, TCP2_GUI.FoldoutBold) ? 1 : 0;
					}

					EditorGUI.indentLevel++;
					toggledGroups.Push((p.floatValue > 0) && visible);
				}
				else if(p.name.StartsWith("__EndGroup"))
				{
					EditorGUI.indentLevel--;
					toggledGroups.Pop();
					GUILayout.Space(8f);
				}
				else
				{
					//Draw regular property
					if(visible && (p.flags & (MaterialProperty.PropFlags.PerRendererData | MaterialProperty.PropFlags.HideInInspector)) == MaterialProperty.PropFlags.None)
						this.materialEditor.ShaderProperty(p, p.displayName);
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				materialEditor.PropertiesChanged();
			}
		}

	#endif     // !SHOW_DEFAULT_INSPECTOR

	#if UNITY_5_5_OR_NEWER
		TCP2_GUI.Separator();
		materialEditor.RenderQueueField();
	#endif
	#if UNITY_5_6_OR_NEWER
		materialEditor.EnableInstancingField();
	#endif
	}
}
