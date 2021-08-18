using UnityEngine;

public class GuiTest : MonoBehaviour
{
    private void OnGUI()
    {
        int index = 0;

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "注册事件"))
        {
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "去注册事件"))
        {
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "去注册事件"))
        {
        }
    }
}