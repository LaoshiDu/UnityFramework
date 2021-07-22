using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

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
        private Dictionary<PanelName, PanelConfig> panelConfigDic;

        /// <summary>
        /// 存储所有加载进来的面板
        /// </summary>
        private Dictionary<PanelName, BasePanel> loadedPanelDic;

        /// <summary>
        /// 可以同时存在并且可以同时操作的面板
        /// </summary>
        private Dictionary<PanelName, BasePanel> panelDic;

        /// <summary>
        /// 弹窗面板
        /// </summary>
        private Stack<BasePanel> popUpStack;

        /// <summary>
        /// Loading界面单独存储
        /// </summary>
        private BasePanel loadingPanel;

        public void Init()
        {
            panelConfigDic = new Dictionary<PanelName, PanelConfig>();
            loadedPanelDic = new Dictionary<PanelName, BasePanel>();
            panelDic = new Dictionary<PanelName, BasePanel>();
            popUpStack = new Stack<BasePanel>();
            canvas = GameObject.Find("Canvas")?.transform;

            if (canvas == null)
            {
                GameObject canvasPrefab = Resources.Load<GameObject>("Prefabs/UI/Canvas");
                canvas = GameObject.Instantiate<GameObject>(canvasPrefab).transform;
                canvas.name = "Canvas";
            }

            eventSystem = GameObject.Find("EventSystem");

            //读取面板配置文件
            TextAsset json = Resources.Load<TextAsset>("UIConfig/PanelConfig");

            PanelConfigList list = JsonMapper.ToObject<PanelConfigList>(json.text);
            for (int i = 0; i < list.panels.Count; i++)
            {
                panelConfigDic.Add(list.panels[i].name, list.panels[i]);
            }

            GameObject.DontDestroyOnLoad(canvas);

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
            if (!loadedPanelDic.ContainsKey(panelName))
            {
                GameObject panelPrefab = Resources.Load<GameObject>(panelConfigDic[panelName].path);
                if (!panelPrefab)
                {
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
                        basePanel.panelType = panelConfigDic[panelName].type;
                        basePanel.Init();
                        loadedPanelDic.Add(panelName, basePanel);
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 卸载UI面板
        /// </summary>
        /// <param name="panelName">UI面板名字</param>
        public void UnLoadPanel(PanelName panelName)
        {
            if (loadedPanelDic.ContainsKey(panelName))
            {
                if (loadedPanelDic[panelName].panelState != PanelState.Hide)
                {
                    loadedPanelDic[panelName].Hide();
                }
                if (loadedPanelDic[panelName].panelState == PanelState.Hide)
                {
                    loadedPanelDic[panelName].Destroy();
                    loadedPanelDic.Remove(panelName);
                }
            }
        }

        /// <summary>
        /// 卸载所有UI面板
        /// </summary>
        /// <param name="isUnLoadLoadingPanel">是否卸载Loading界面</param>
        public void UnLoadAllPanels(bool isUnLoadLoadingPanel)
        {
            foreach (var item in panelDic.Keys)
            {
                HidePanel(item);
            }
            while (popUpStack.Count > 0)
            {
                HidePanel(popUpStack.Peek().panelName);
            }
            if (isUnLoadLoadingPanel && loadingPanel != null)
            {
                HidePanel(loadingPanel.panelName);
            }
            foreach (var item in loadedPanelDic.Keys)
            {
                if (!isUnLoadLoadingPanel && item == loadingPanel.panelName)
                {

                }
                else
                {
                    loadedPanelDic[item].Destroy();
                }
            }
            loadedPanelDic.Clear();
            popUpStack.Clear();
            panelDic.Clear();

            if (!isUnLoadLoadingPanel && loadingPanel != null)
            {
                loadedPanelDic.Add(loadingPanel.panelName, loadingPanel);
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
                BasePanel basePanel = loadedPanelDic[panelName];
                SetRoot(basePanel.transform, basePanel.panelType);
                switch (basePanel.panelType)
                {
                    case PanelType.Base:
                    case PanelType.PopupWindow:
                    case PanelType.Tip:
                        if (popUpStack.Count > 0)
                        {
                            popUpStack.Peek().OnPause();
                        }
                        basePanel.Show();
                        popUpStack.Push(basePanel);
                        break;
                    case PanelType.Panel:
                        basePanel.Show();
                        panelDic.Add(panelName, basePanel);
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
            if (loadedPanelDic.ContainsKey(panelName))
            {
                BasePanel basePanel = loadedPanelDic[panelName];
                switch (basePanel.panelType)
                {
                    case PanelType.Base:
                    case PanelType.PopupWindow:
                    case PanelType.Tip:
                        if (popUpStack.Count > 0 && panelName == popUpStack.Peek().panelName)
                        {
                            popUpStack.Pop();
                            basePanel.Hide();
                            basePanel.transform.SetParent(hideRoot);
                            if (popUpStack.Count > 0)
                            {
                                popUpStack.Peek().UnPause();
                            }
                        }
                        break;
                    case PanelType.Panel:
                        if (panelDic.ContainsKey(panelName))
                        {
                            panelDic.Remove(panelName);
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