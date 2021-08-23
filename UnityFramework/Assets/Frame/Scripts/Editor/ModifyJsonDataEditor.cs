using LitJson;
using System.IO;
using UnityEditor;
using UnityEngine;
using WKC;

[CustomEditor(typeof(ModifyJsonData))]
public class ModifyJsonDataEditor : Editor
{
    private SerializedProperty userdata;
    private ModifyJsonData jsonData;

    private void OnEnable()
    {
        userdata = serializedObject.FindProperty("userdata");
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(userdata);
        serializedObject.ApplyModifiedProperties();

        jsonData = target as ModifyJsonData;

        if (GUILayout.Button("ÐÞ¸ÄÊý¾Ý"))
        {
            jsonData.userdata.ModifyData();
            if (!Directory.Exists(Application.persistentDataPath + "/UserData"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/UserData");
            }
            FileInfo _saveLocalFileInfo = new FileInfo(Application.persistentDataPath + @"/UserData/UserData.json");
            using (StreamWriter sw = _saveLocalFileInfo.CreateText())
            {
                Debug.LogError(jsonData.userdata.gold);
                var result = JsonMapper.ToJson(jsonData.userdata);
                sw.Write(result);
            }
        }

    }
}