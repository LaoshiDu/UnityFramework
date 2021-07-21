using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MissingScriptChecker : EditorWindow
{
    Vector2 scrollPos = Vector2.zero;

    //保存丢失脚本的资源 assetPath
    List<string> hasMissScriptFiles = new List<string>();

    //已修复资源  assetPath
    HashSet<string> fixedFiles = new HashSet<string>();

    [MenuItem("Tools/Tools/检查丢失的脚本引用")]
    private static void CheckMissScript()
    {
        GetWindow<MissingScriptChecker>("丢失脚本检查器");
    }

    //是否查找特定路径
    bool isSpecifyPath = false;
    //搜索目标文件夹
    DefaultAsset serchFolderAsset;
    //搜索目录
    string serchPath
    {
        get
        {
            if (serchFolderAsset != null)
            {
                return AssetDatabase.GetAssetPath(serchFolderAsset);
            }
            return "Assets";
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        isSpecifyPath = GUILayout.Toggle(isSpecifyPath, "指定目录搜索");
        if (isSpecifyPath)
        {
            EditorGUILayout.LabelField("文件夹 : ", GUILayout.Width(50));
            serchFolderAsset = (DefaultAsset)EditorGUILayout.ObjectField(serchFolderAsset, typeof(DefaultAsset), false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("搜索目录 : " + serchPath);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (GUILayout.Button("[检查 - 脚本丢失]"))
        {
            if (string.IsNullOrEmpty(serchPath))
            {
                EditorUtility.DisplayDialog("错误", "未指定搜索文件夹!", "确定");
                return;
            }

            hasMissScriptFiles.Clear();
            fixedFiles.Clear();
            CheckMissingScripts(serchPath);
        }

        if (hasMissScriptFiles.Count > 0)
        {
            if (GUILayout.Button("[修复 - 脚本丢失]"))
            {
                foreach (string assetPath in hasMissScriptFiles)
                {
                    FixAssetMissingScript(assetPath);
                }

            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("MissingScript 数量 : " + hasMissScriptFiles.Count);
        foreach (string assetPath in hasMissScriptFiles)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)), typeof(Object), false);
            if (fixedFiles.Contains(assetPath))
            {
                EditorGUILayout.LabelField("√", GUILayout.Width(200));
            }
            else
            {
                if (GUILayout.Button("修复", GUILayout.Width(200)))
                {
                    FixAssetMissingScript(assetPath);
                }
            }

            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.EndScrollView();
    }

    //修复asset资源上的丢失脚本
    private void FixAssetMissingScript(string assetPath)
    {
        GameObject go = PrefabUtility.LoadPrefabContents(assetPath);
        RemoveGameObjectMissingScripts(go);
        PrefabUtility.SaveAsPrefabAssetAndConnect(go, assetPath, InteractionMode.UserAction);
        PrefabUtility.UnloadPrefabContents(go);
        fixedFiles.Add(assetPath);
    }

    //判断是否存在丢失脚本
    private bool IsExistMissingScripts(GameObject go)
    {
        int missingNum = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        if (missingNum > 0)
        {
            return true;
        }
        else
        {
            foreach (Transform child in go.transform)
            {
                bool isHasMissing = IsExistMissingScripts(child.gameObject);
                if (isHasMissing) return isHasMissing;
            }
        }

        return false;
    }

    //移除丢失脚本
    private void RemoveGameObjectMissingScripts(GameObject go)
    {
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        foreach (Transform child in go.transform)
        {
            RemoveGameObjectMissingScripts(child.gameObject);
        }
    }

    //检测丢失脚本
    private void CheckMissingScripts(string serchPath)
    {
        string[] files = Directory.GetFiles(serchPath, "*.prefab", SearchOption.AllDirectories);

        int count = 0;
        foreach (string fileName in files)
        {
            count++;
            string assetPath = fileName.Replace(Application.dataPath, "Assets");

            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            bool hasMissScripts = IsExistMissingScripts(go);

            if (hasMissScripts)
            {
                Debug.Log("存在丢失脚本 assetPath : " + assetPath);
                hasMissScriptFiles.Add(assetPath);
            }
            EditorUtility.DisplayProgressBar("检查 " + serchPath + " MissingScript...", "检查中... (" + count + "/" + files.Length + ")", (float)count / (float)files.Length);
        }

        Debug.Log("搜索完成 存在MissingScript数量 : " + hasMissScriptFiles.Count);
        EditorUtility.ClearProgressBar();
    }
}