using UnityEngine;
using UnityEngine.UI;
using WKC;

public class MainMenuPanel : BasePanel
{
    private Text text;
    public override void Init()
    {
        base.Init();
        text = GetUIComponent<Text>("Text");
    }

    public override void Show()
    {
        base.Show();
        TimeManager.Instance.UpdateEventHandler += UpdateText;
    }

    public override void Hide()
    {
        base.Hide();
        TimeManager.Instance.UpdateEventHandler -= UpdateText;
    }

    private void UpdateText()
    {
        text.text = DataStorage.Instance.userdata.gold.ToString();
    }
}