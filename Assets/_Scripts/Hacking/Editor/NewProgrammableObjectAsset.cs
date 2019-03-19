using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewProgrammableObjectAsset 
{
    [MenuItem("Assets/Create/Programmable Object Asset")]
    public static void CreateMyAsset()
    {
        ProgrammableObjectsScriptable asset = ScriptableObject.CreateInstance<ProgrammableObjectsScriptable>();

        AssetDatabase.CreateAsset(asset, "Assets/NewProgObjectAsset.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
