using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

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
            EditorExtension.WriteScript(sw, panelName);
            sw.Close();
        }

        private void CreatePrefab()
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/Canvas");
                canvas = Instantiate(prefab);
                canvas.name = "Canvas";
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

                string prefabDir = Application.dataPath + "/Resources/Prefabs/UIPanels";
                if (!Directory.Exists(prefabDir))
                {
                    Directory.CreateDirectory(prefabDir);
                }
                string prefabPath = prefabDir + "/" + panelName + ".prefab";
                PrefabUtility.SaveAsPrefabAssetAndConnect(panel, prefabPath, InteractionMode.AutomatedAction);
            }
        }
    }
}