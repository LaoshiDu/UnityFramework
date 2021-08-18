using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WKC
{
    public class LoadSceneManager : BaseMgr<LoadSceneManager>
    {
        private float progess;
        private EventCall callback;
        private object[] args;

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="levelName"></param>
        public void LoadScene(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="levelName">场景名字</param>
        public void LoadSceneAsync(string levelName, EventCall fun = null, params object[] obj)
        {
            callback = fun;
            args = obj;
            TimeManager.Instance.StartCoroutine(IELoadScene(levelName));
        }

        IEnumerator IELoadScene(string levelName)
        {
            UIManager.Instance.ShowPanel(PanelName.LoadingPanel);
            AsyncOperation async = SceneManager.LoadSceneAsync(levelName);
            //即使场景加载完毕也不直接显示 注意：在async.allowSceneActivation = false时，async.progress最大只能到0.9，async.isDone也是false
            //只有场景显示出来，在async.allowSceneActivation = true时，async.progress才能到1
            async.allowSceneActivation = false;
            progess = 0;
            EventCenterManager.Instance.SendEvent(EventName.LoadingProgess, progess);
            while (true)
            {
                if (async.progress < 0.9f)
                {
                    progess = Mathf.Lerp(progess, async.progress, 2 * Time.deltaTime);
                }
                else
                {
                    progess = Mathf.Lerp(progess, 1.1f, 2 * Time.deltaTime);
                }
                progess = Mathf.Clamp(progess, 0, 1);
                EventCenterManager.Instance.SendEvent(EventName.LoadingProgess, progess);
                if (progess >= 1)
                {
                    break;
                }
                yield return null;
            }
            UIManager.Instance.HidePanel(PanelName.LoadingPanel);
            async.allowSceneActivation = true;
            callback?.Invoke(args);
        }
    }
}