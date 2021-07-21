using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Callbacks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WKC
{
    public class UIEditor : EditorWindow
    {
        [MenuItem("Tools/UI±à¼­Æ÷", false, 41)]
        private static void OpenUIEditor()
        {
            GetWindow<UIEditor>("UI±à¼­Æ÷");
        }

        private string panelName;
        private PanelType panelType;
        private static string uiPrefabsPath = Application.dataPath + "/Resources/Prefabs/UIPanels";

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("PanelÃû×Ö£º", GUILayout.Width(80));
            panelName = GUILayout.TextField(panelName);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("PanelÀàÐÍ£º", GUILayout.Width(80));
            panelType = (PanelType)EditorGUILayout.EnumPopup(panelType);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                CreatePrefab();
                CreateScript();
                AssetDatabase.Refresh();
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void CreateScript()
        {
            string scriptPath = Application.dataPath + "/Scripts/UIPanels";

            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }

            string script = scriptPath + "/" + panelName + ".cs";

            StreamWriter sw = File.CreateText(script);
            EditorExtension.WriteScript(sw, panelName, "BasePanel");
            sw.Close();
            EditorPrefs.SetInt("PanelType", (int)panelType);
        }

        [DidReloadScripts]
        private static void AddComponent()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var defaultAssembly = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            string panelname = EditorPrefs.GetString("PanelName");
            int paneltype = EditorPrefs.GetInt("PanelType");

            if (!string.IsNullOrWhiteSpace(panelname))
            {
                GameObject uiPanel = GameObject.Find(panelname);
                if (uiPanel != null)
                {
                    var script = defaultAssembly.GetType(panelname);
                    string prefabPath = uiPrefabsPath + "/" + panelname + ".prefab";
                    BasePanel basePanel = uiPanel.GetComponent(script) as BasePanel;
                    if (basePanel == null)
                    {
                        basePanel = uiPanel.AddComponent(script) as BasePanel;
                    }

                    basePanel.panelType = (PanelType)paneltype;
                    PrefabUtility.SaveAsPrefabAssetAndConnect(uiPanel, prefabPath, InteractionMode.AutomatedAction);

                    EditorPrefs.DeleteKey("PanelName");
                    EditorPrefs.DeleteKey("PanelType");
                }
            }

            UpdatePanelName();
        }

        private void CreatePrefab()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/Canvas");
                canvas = Instantiate(prefab);
                canvas.name = "Canvas";
            }
            GameObject panel = new GameObject(panelName);

            panel.transform.SetParent(canvas.transform);
            panel.transform.localScale = Vector3.one;
            RectTransform rTrans = panel.AddComponent<RectTransform>();
            rTrans.anchorMin = Vector2.zero;
            rTrans.anchorMax = Vector2.one;
            rTrans.pivot = new Vector2(0.5f, 0.5f);
            rTrans.offsetMax = Vector2.zero;
            rTrans.offsetMin = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0.1f);

            panel.layer = LayerMask.NameToLayer("UI");
            if (!Directory.Exists(uiPrefabsPath))
            {
                Directory.CreateDirectory(uiPrefabsPath);
            }
            string prefabPath = uiPrefabsPath + "/" + panelName + ".prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(panel, prefabPath, InteractionMode.AutomatedAction);
            EditorPrefs.SetString("PanelName", panelName);
        }
        
        private static void ModifyConfig(Dictionary<PanelType, List<GameObject>> panelDic)
        {
            bool isFirstLine = true;
            string uiConfigPath = Application.dataPath + "/Resources/UIConfig";
            if (!Directory.Exists(uiConfigPath))
            {
                Directory.CreateDirectory(uiConfigPath);
            }
            string panelConfig = uiConfigPath + "/PanelConfig.json";
            StreamWriter sw = File.CreateText(panelConfig);
            sw.WriteLine("{");
            sw.WriteLine("\t"+"\""+"panels"+"\""+":");
            sw.WriteLine("\t[");
            
            foreach (var item in panelDic)
            {
                int index = 0;
                switch (item.Key)
                {
                    case PanelType.Base:
                        index = 1;
                        WriteUIConfigItem(index, item.Value, sw, (int)item.Key, isFirstLine);
                        continue;
                    case PanelType.PopupWindow:
                        index = 101;
                        WriteUIConfigItem(index, item.Value, sw, (int)item.Key, isFirstLine);
                        continue;
                    case PanelType.Panel:
                        index = 201;
                        WriteUIConfigItem(index, item.Value, sw, (int)item.Key, isFirstLine);
                        continue;
                    case PanelType.Tip:
                        index = 301;
                        WriteUIConfigItem(index, item.Value, sw, (int)item.Key, isFirstLine);
                        continue;
                    case PanelType.Loading:
                        index = 401;
                        WriteUIConfigItem(index,item.Value,sw,(int)item.Key,isFirstLine);
                        continue;
                }
            }
            sw.WriteLine();
            sw.WriteLine("\t]");
            sw.Write("}");
            sw.Close();
        }

        private static void WriteUIConfigItem(int index,List<GameObject> item, StreamWriter sw,int type,bool isFirstLine)
        {
            for (int i = 0; i < item.Count; i++)
            {
                if (!isFirstLine)
                {
                    sw.WriteLine(",");
                }
                sw.WriteLine("\t\t{");
                sw.WriteLine("\t\t\t" + "\"" + "name" + "\"" + ":" + index + ",");
                sw.WriteLine("\t\t\t" + "\"" + "type" + "\"" + ":" + type + ",");
                sw.WriteLine("\t\t\t" + "\"" + "path" + "\"" + ":" + "\"" + "Prefabs/UIPanels/" + item[i].name + "\"");
                sw.Write("\t\t}");
                index++;
                isFirstLine = false;
            }
        }

        private static void UpdatePanelName()
        {
            Dictionary<PanelType, List<GameObject>> panelDic = new Dictionary<PanelType, List<GameObject>>();
            DirectoryInfo root = new DirectoryInfo(uiPrefabsPath);
            foreach (FileInfo f in root.GetFiles())
            {
                string name = f.Name;
                string suffix = name.Substring(name.Length - 5);
                if (suffix != ".meta")
                {
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/UIPanels/" + name);
                    BasePanel bp = go.GetComponent<BasePanel>();
                    if (panelDic.ContainsKey(bp.panelType))
                    {
                        panelDic[bp.panelType].Add(go);
                    }
                    else
                    {
                        panelDic.Add(bp.panelType, new List<GameObject>() { go });
                    }
                }
            }
            string scriptPath = Application.dataPath + "/Frame/Scripts/UI";

            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            string script = scriptPath + "/PanelName.cs";
            StreamWriter sw = File.CreateText(script);
            sw.WriteLine("public enum PanelName");
            sw.WriteLine("{");
            foreach (var item in panelDic)
            {
                int index = 0;
                switch (item.Key)
                {
                    case PanelType.Base:
                        index = 1;
                        WritePanelName(index,item.Value,sw);
                        continue;
                    case PanelType.PopupWindow:
                        index = 101;
                        WritePanelName(index, item.Value, sw);
                        continue;
                    case PanelType.Panel:
                        index = 201;
                        WritePanelName(index, item.Value, sw);
                        continue;
                    case PanelType.Tip:
                        index = 301;
                        WritePanelName(index, item.Value, sw);
                        continue;
                    case PanelType.Loading:
                        index = 401;
                        WritePanelName(index, item.Value, sw);
                        continue;
                }
            }
            sw.Write("}");
            sw.Close();
            ModifyConfig(panelDic);
            try
            {
                AssetDatabase.Refresh();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void WritePanelName(int index,List<GameObject> item, StreamWriter sw)
        {
            for (int i = 0; i < item.Count; i++)
            {
                sw.WriteLine("\t" + item[i].name + " = " + index + ",");
                index++;
            }
        }
    }
}