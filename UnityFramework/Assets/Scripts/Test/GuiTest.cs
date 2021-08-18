using System.Collections.Generic;
using UnityEngine;
using WKC;

public class GuiTest : MonoBehaviour
{
    GameObject obj;
    private void OnGUI()
    {
        int index = 0;

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "注册事件"))
        {
            obj = ObjectPoolManager.Instance.GetObj(ObjectPoolConfig.GetList()[0].resPath);
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "去注册事件"))
        {
            ObjectPoolManager.Instance.RecycleObject(obj);
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "去注册事件"))
        {
            ObjectPoolManager.Instance.ClearObjPool();
        }
    }
}