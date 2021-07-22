using UnityEngine;
using UnityEngine.UI;

namespace WKC
{
    public abstract class BasePanel : MonoBehaviour
    {
        public PanelName panelName;
        public PanelType panelType;
        public PanelState panelState;

        private Button closeBtn;

        /// <summary>
        /// 初始化面板，加载时调用一次
        /// </summary>
        public virtual void Init()
        {
            closeBtn = GetUIComponent<Button>("CloseBtn");
        }

        /// <summary>
        /// 卸载面板
        /// </summary>
        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            panelState = PanelState.Showing;
            closeBtn?.onClick.AddListener(() =>
            {
                UIManager.Instance.HidePanel(panelName);
            });
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            panelState = PanelState.Hide;
            closeBtn?.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 暂停面板
        /// </summary>
        public virtual void OnPause()
        {
            panelState = PanelState.Pause;
        }

        /// <summary>
        /// 解除暂停面板
        /// </summary>
        public virtual void UnPause()
        {
            panelState = PanelState.Showing;
        }

        protected T GetUIComponent<T>(string path) where T : Component
        {
            return transform.Find(path)?.GetComponent<T>();
        }
    }
}