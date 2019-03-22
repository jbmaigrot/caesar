using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if (UNITY_EDITOR) 
public class ProgObjAssetEditor : EditorWindow
{
    public ProgrammableObjectsScriptable ProgObjAsset;
    string hackPath;

    [MenuItem("Window/Programmable Object Asset Editor %#e")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(ProgObjAssetEditor));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Programmable Object Asset", GUILayout.ExpandWidth(false)))
        {
            NewProgObjAsset();
        }
        if (GUILayout.Button("Open Programmable Object Asset", GUILayout.ExpandWidth(false)))
        {
            OpenProgObjAsset();
        }
        if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
        {
            AssetDatabase.SaveAssets();
        }
        
        hackPath = EditorGUILayout.TextField(hackPath, GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false)) && hackPath != AssetDatabase.GetAssetPath(ProgObjAsset))
        {
            hackPath = AssetDatabase.GenerateUniqueAssetPath(hackPath);
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(ProgObjAsset), hackPath);
        }
        if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ProgObjAsset));
            hackPath = "";
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (ProgObjAsset)
        {
            GUILayout.Label("List of InputButton :");
            for (int i = 0; i < ProgObjAsset.inputCodes.Count; i++)
            {
                InOutVignette interVar = new InOutVignette();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Code : ");
                interVar.code = EditorGUILayout.TextField(ProgObjAsset.inputCodes[i].code, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label(" Description : ");
                interVar.parameter_int = EditorGUILayout.IntField(ProgObjAsset.inputCodes[i].parameter_int, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                interVar.parameter_string = EditorGUILayout.TextField(ProgObjAsset.inputCodes[i].parameter_string, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label("Is fixed");
                interVar.is_fixed = EditorGUILayout.Toggle(ProgObjAsset.inputCodes[i].is_fixed);
                ProgObjAsset.inputCodes[i] = interVar;
                if (GUILayout.Button(" Destroy Input", GUILayout.ExpandWidth(false)))
                {
                    ProgObjAsset.inputCodes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Input", GUILayout.ExpandWidth(false)))
            {
                InOutVignette interVar = new InOutVignette();
                interVar.code = "";
                interVar.parameter_int = 0;
                interVar.parameter_string = "";
                interVar.is_fixed = false;
                ProgObjAsset.inputCodes.Add(interVar);
            }

            EditorGUILayout.Space();

            GUILayout.Label("List of OutputButton :");
            for (int i = 0; i < ProgObjAsset.outputCodes.Count; i++)
            {
                InOutVignette interVar = new InOutVignette();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Code : ");
                interVar.code = EditorGUILayout.TextField(ProgObjAsset.outputCodes[i].code, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label(" parametre : ");
                interVar.parameter_int = EditorGUILayout.IntField(ProgObjAsset.outputCodes[i].parameter_int, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                interVar.parameter_string = EditorGUILayout.TextField(ProgObjAsset.outputCodes[i].parameter_string, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label("Is fixed");
                interVar.is_fixed = EditorGUILayout.Toggle(ProgObjAsset.outputCodes[i].is_fixed);
                ProgObjAsset.outputCodes[i] = interVar;
                if (GUILayout.Button(" Destroy Output", GUILayout.ExpandWidth(false)))
                {
                    ProgObjAsset.outputCodes.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Output", GUILayout.ExpandWidth(false)))
            {
                InOutVignette interVar = new InOutVignette();
                interVar.code = "";
                interVar.parameter_int = 0;
                interVar.parameter_string = "";
                interVar.is_fixed = false;
                ProgObjAsset.outputCodes.Add(interVar);
            }

            GUILayout.Label("List of Arrow :");
            for (int i = 0; i < ProgObjAsset.graph.Count; i++)
            {
                Arrow interVar = new Arrow();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("input : ");
                interVar.input = EditorGUILayout.IntField(ProgObjAsset.graph[i].input, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                GUILayout.Label("output : ");
                interVar.output = EditorGUILayout.IntField(ProgObjAsset.graph[i].output, GUILayout.ExpandWidth(false), GUILayout.MinWidth(30));
                ProgObjAsset.graph[i] = interVar;
                if (GUILayout.Button(" Destroy Arrow", GUILayout.ExpandWidth(false)))
                {
                    ProgObjAsset.graph.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Arrow", GUILayout.ExpandWidth(false)))
            {
                Arrow interVar = new Arrow();
                interVar.input = 0;
                interVar.output = 0;
                ProgObjAsset.graph.Add(interVar);
            }
        }
        /*EditorUtility.SetDirty(HackingAsset);
        AssetDatabase.SaveAssets();*/
    }

    private void NewProgObjAsset()
    {
        ProgObjAsset = ScriptableObject.CreateInstance<ProgrammableObjectsScriptable>();
        AssetDatabase.CreateAsset(ProgObjAsset, AssetDatabase.GenerateUniqueAssetPath("Assets/NewProgObjectAsset.asset"));

        EditorUtility.SetDirty(ProgObjAsset);
        AssetDatabase.SaveAssets();
        if (ProgObjAsset)
        {
            hackPath = AssetDatabase.GetAssetPath(ProgObjAsset);
            EditorPrefs.SetString("ObjectPath", hackPath);
        }
    }

    private void OpenProgObjAsset()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Programmable Object Asset", "", "");
        if (absPath.StartsWith(Application.dataPath))
        {
            hackPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            ProgObjAsset = AssetDatabase.LoadAssetAtPath(hackPath, typeof(ProgrammableObjectsScriptable)) as ProgrammableObjectsScriptable;
            if (ProgObjAsset)
            {
                EditorPrefs.SetString("ObjectPath", hackPath);
            }
        }
    }
}
#endif