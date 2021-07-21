using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class UIManager : BaseMgr<UIManager>
    {
        private Transform canvas;
        private GameObject eventSystem;

        #region 节点
        private Transform hideRoot;
        private Transform baseRoot;
        private Transform panelRoot;
        private Transform tipRoot;
        private Transform loadingRoot;
        #endregion

        /// <summary>
        /// 存储面板的配置信息
        /// </summary>
        private Dictionary<PanelName, PanelConfigInfo> configInfoDic;

        /// <summary>
        /// 存储所有加载进来的面板
        /// </summary>
        private Dictionary<PanelName, BasePanel> loadedPanels;

        /// <summary>
        /// 可以同时存在并且可以同时操作的面板
        /// </summary>
        private Dictionary<PanelName, BasePanel> panels;

        /// <summary>
        /// 弹窗面板
        /// </summary>
        private Stack<BasePanel> popupWindowPanels;

        /// <summary>
        /// Loading界面单独存储
        /// </summary>
        private BasePanel loadingPanel;

        public void Init()
        {
            configInfoDic = new Dictionary<PanelName, PanelConfigInfo>();
            loadedPanels = new Dictionary<PanelName, BasePanel>();
            panels = new Dictionary<PanelName, BasePanel>();
            popupWindowPanels = new Stack<BasePanel>();
            canvas = GameObject.Find("Canvas").transform;
            eventSystem = GameObject.Find("EventSystem");

            //读取面板配置文件
            TextAsset json = Resources.Load<TextAsset>("UIConfig/PanelConfig");
            ConfigInfo info = JsonUtility.FromJson<ConfigInfo>(json.text);
            for (int i = 0; i < info.panels.Count; i++)
            {
                configInfoDic.Add(info.panels[i].Name, info.panels[i]);
            }

            GameObject.DontDestroyOnLoad(canvas);
            //GameObject.DontDestroyOnLoad(eventSystem);

            hideRoot = canvas.Find("HideRoot");
            baseRoot = canvas.Find("BaseRoot");
            panelRoot = canvas.Find("PanelRoot");
            tipRoot = canvas.Find("TipRoot");
            loadingRoot = canvas.Find("LoadingRoot");
        }

        /// <summary>
        /// 加载UI面板
        /// </summary>
        /// <param name="panelName">UI面板名字</param>
        public bool LoadPanel(PanelName panelName)
        {
            if (!loadedPanels.ContainsKey(panelName))
            {
                GameObject panelPrefab = Resources.Load<GameObject>(configInfoDic[panelName].Path);
                if (!panelPrefab)
                {
                    Debug.LogError(configInfoDic[panelName].Path);
                    Debug.LogError("没有加载到UI面板Prefab：" + panelName);
                    return false;
                }
                else
                {
                    GameObject panel = GameObject.Instantiate(panelPrefab, hideRoot);
                    panel.transform.localPosition = Vector3.zero;
                    panel.transform.localRotation = Quaternion.identity;
                    panel.transform.localScale = Vector3.one;
                    panel.name = panelName.ToString();
                    BasePanel basePanel = panel.GetComponent<BasePanel>();
                    if (!basePanel)
                    {
                        Debug.LogError(panelName + "面板上没有挂载脚本" + panelName);
                        return false;
                    }
                    else
                    {
                        basePanel.panelName = panelName;
                        basePanel.panelType = configInfoDic[panelName].Type;
                        basePanel.Init();
                        loadedPanels.Add(panelName, basePanel);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 卸载UI面板
        /// </summary>
        /// <param name="panelName">UI面板名字</param>
        public void UnLoadPanel(PanelName panelName)
        {
            if (loadedPanels.ContainsKey(panelName))
            {
                if (loadedPanels[panelName].panelState != PanelState.Hide)
                {
                    loadedPanels[panelName].Hide();
                }
                if (loadedPanels[panelName].panelState == PanelState.Hide)
                {
                    loadedPanels[panelName].Destroy();
                    loadedPanels.Remove(panelName);
                }
            }
        }

        /// <summary>
        /// 卸载所有UI面板
        /// </summary>
        /// <param name="isUnLoadLoadingPanel">是否卸载Loading界面</param>
        public void UnLoadAllPanels(bool isUnLoadLoadingPanel)
        {
            foreach (var item in panels.Keys)
            {
                HidePanel(item);
            }
            while (popupWindowPanels.Count > 0)
            {
                HidePanel(popupWindowPanels.Peek().panelName);
            }
            if (isUnLoadLoadingPanel && loadingPanel != null)
            {
                HidePanel(loadingPanel.panelName);
            }
            foreach (var item in loadedPanels.Keys)
            {
                if (!isUnLoadLoadingPanel && item == loadingPanel.panelName)
                {

                }
                else
                {
                    loadedPanels[item].Destroy();
                }
            }
            loadedPanels.Clear();
            popupWindowPanels.Clear();
            panels.Clear();

            if (!isUnLoadLoadingPanel && loadingPanel != null)
            {
                loadedPanels.Add(loadingPanel.panelName, loadingPanel);
            }
        }

        /// <summary>
        /// 显示UI面板
        /// </summary>
        /// <param name="panelName">UI面板名字</param>
        public void ShowPanel(PanelName panelName)
        {
            if (LoadPanel(panelName))
            {
                BasePanel basePanel = loadedPanels[panelName];
                PanelConfigInfo info = configInfoDic[panelName];
                SetRoot(basePanel.transform, info.Type);
                switch (info.Type)
                {
                    case PanelType.Base:
                    case PanelType.PopupWindow:
                    case PanelType.Tip:
                        if (popupWindowPanels.Count > 0)
                        {
                            popupWindowPanels.Peek().OnPause();
                        }
                        basePanel.Show();
                        popupWindowPanels.Push(basePanel);
                        break;
                    case PanelType.Panel:
                        basePanel.Show();
                        panels.Add(panelName, basePanel);
                        break;
                    case PanelType.Loading:
                        basePanel.Show();
                        loadingPanel = basePanel;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 隐藏UI面板
        /// </summary>
        /// <param name="panelName">UI面板名字</param>
        public void HidePanel(PanelName panelName)
        {
            if (loadedPanels.ContainsKey(panelName))
            {
                BasePanel basePanel = loadedPanels[panelName];
                PanelConfigInfo info = configInfoDic[panelName];
                switch (info.Type)
                {
                    case PanelType.Base:
                    case PanelType.PopupWindow:
                    case PanelType.Tip:
                        if (panelName == popupWindowPanels.Peek().panelName)
                        {
                            popupWindowPanels.Pop();
                            basePanel.Hide();
                            basePanel.transform.SetParent(hideRoot);
                            if (popupWindowPanels.Count > 0)
                            {
                                popupWindowPanels.Peek().UnPause();
                            }
                        }
                        break;
                    case PanelType.Panel:
                        if (panels.ContainsKey(panelName))
                        {
                            panels.Remove(panelName);
                            basePanel.Hide();
                            basePanel.transform.SetParent(hideRoot);
                        }
                        break;
                    case PanelType.Loading:
                        if (loadingPanel != null && loadingPanel.panelName == panelName)
                        {
                            loadingPanel = null;
                            basePanel.Hide();
                            basePanel.transform.SetParent(hideRoot);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 通过UI面板类型设置父节点
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="type"></param>
        private void SetRoot(Transform panel, PanelType type)
        {
            switch (type)
            {
                case PanelType.Base:
                    panel.SetParent(baseRoot);
                    break;
                case PanelType.PopupWindow:
                    panel.SetParent(panelRoot);
                    break;
                case PanelType.Panel:
                    panel.SetParent(panelRoot);
                    break;
                case PanelType.Tip:
                    panel.SetParent(tipRoot);
                    break;
                case PanelType.Loading:
                    panel.SetParent(loadingRoot);
                    break;
                default:
                    break;
            }
        }
    }
}