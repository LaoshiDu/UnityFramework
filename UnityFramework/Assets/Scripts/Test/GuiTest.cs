using UnityEngine;

public class GuiTest : MonoBehaviour
{
    private void OnGUI()
    {
        int index = 0;

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "ע���¼�"))
        {
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "ȥע���¼�"))
        {
        }

        if (GUI.Button(new Rect(10 + 110 * index++, 100, 100, 100), "ȥע���¼�"))
        {
        }
    }
}