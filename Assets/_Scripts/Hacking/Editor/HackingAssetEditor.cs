using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if (UNITY_EDITOR) 
public class HackingAssetEditor : EditorWindow
{
    public HackingAssetScriptable HackingAsset;
    string hackPath;

    [MenuItem("Window/Hacking Asset Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(HackingAssetEditor));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("New Hacking Asset", GUILayout.ExpandWidth(false)))
        {
            NewHackingAsset();
        }
        if (GUILayout.Button("Open Hacking Asset", GUILayout.ExpandWidth(false)))
        {
            OpenHackingAsset();
        }
        hackPath = EditorGUILayout.TextField(hackPath,GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false))&&hackPath!= AssetDatabase.GetAssetPath(HackingAsset))
        {
            hackPath = AssetDatabase.GenerateUniqueAssetPath(hackPath);
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(HackingAsset), hackPath);
        }
        if(GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(HackingAsset));
            hackPath = "";
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (HackingAsset)
        {
            GUILayout.Label("Input List of Asset :");
            for (int i = 0;i< HackingAsset.inputCodes.Count; i++)
            {
                Debug.Log("j'arrive ici");
                InOutCode interVar;
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Code : ");
                interVar.code= EditorGUILayout.TextField(HackingAsset.inputCodes[i].code, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label(" Description : ");
                interVar.descriptionWithParameter = EditorGUILayout.TextField(HackingAsset.inputCodes[i].descriptionWithParameter, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                interVar.descriptionWithoutParameter = HackingAsset.inputCodes[i].descriptionWithParameter;
                GUILayout.Label(" Parametre entier ? : ");
                interVar.parameter_int = EditorGUILayout.Toggle(HackingAsset.inputCodes[i].parameter_int);
                GUILayout.Label(" Parametre string ? : ");
                interVar.parameter_string = EditorGUILayout.Toggle(HackingAsset.inputCodes[i].parameter_string);
                HackingAsset.inputCodes[i] = interVar;
                if (GUILayout.Button(" Destroy Input", GUILayout.ExpandWidth(false)))
                {
                    HackingAsset.inputCodes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if(GUILayout.Button("Add Input", GUILayout.ExpandWidth(false)))
            {
                InOutCode interVar;
                interVar.code = "";
                interVar.descriptionWithParameter = "";
                interVar.descriptionWithoutParameter = "";
                interVar.parameter_int = false;
                interVar.parameter_string = false;
                HackingAsset.inputCodes.Add(interVar);
            }

            EditorGUILayout.Space();

            GUILayout.Label("Output List of Asset :");
            for (int i = 0; i < HackingAsset.outputCodes.Count; i++)
            {
                InOutCode interVar;
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Code : ");
                interVar.code = EditorGUILayout.TextField(HackingAsset.outputCodes[i].code, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label(" Description : ");
                interVar.descriptionWithParameter = EditorGUILayout.TextField(HackingAsset.outputCodes[i].descriptionWithParameter, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                interVar.descriptionWithoutParameter = HackingAsset.outputCodes[i].descriptionWithParameter;
                GUILayout.Label(" Parametre entier ? : ");
                interVar.parameter_int = EditorGUILayout.Toggle(HackingAsset.outputCodes[i].parameter_int);
                GUILayout.Label(" Parametre string ? : ");
                interVar.parameter_string = EditorGUILayout.Toggle(HackingAsset.outputCodes[i].parameter_string);
                HackingAsset.outputCodes[i] = interVar;
                if (GUILayout.Button(" Destroy Input", GUILayout.ExpandWidth(false)))
                {
                    HackingAsset.outputCodes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add Output", GUILayout.ExpandWidth(false)))
            {
                InOutCode interVar;
                interVar.code = "";
                interVar.descriptionWithParameter = "";
                interVar.descriptionWithoutParameter = "";
                interVar.parameter_int = false;
                interVar.parameter_string = false;
                HackingAsset.outputCodes.Add(interVar);
            }


        }
        /*EditorUtility.SetDirty(HackingAsset);
        AssetDatabase.SaveAssets();*/
    }

    private void NewHackingAsset()
    {
        HackingAsset = ScriptableObject.CreateInstance<HackingAssetScriptable>();
        AssetDatabase.CreateAsset(HackingAsset, AssetDatabase.GenerateUniqueAssetPath("Assets/HackingAsset.asset"));

        EditorUtility.SetDirty(HackingAsset);
        AssetDatabase.SaveAssets();
        if (HackingAsset)
        {
            hackPath = AssetDatabase.GetAssetPath(HackingAsset);
            EditorPrefs.SetString("ObjectPath", hackPath);
        }
    }

    private void OpenHackingAsset()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Hacking Asset", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            hackPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            HackingAsset = AssetDatabase.LoadAssetAtPath(hackPath, typeof(HackingAssetScriptable)) as HackingAssetScriptable;
            if (HackingAsset)
            {
                EditorPrefs.SetString("ObjectPath", hackPath);
            }
        }
    }
}
#endif