using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelConfigInfo
{
    [SerializeField]
    private PanelName name;
    [SerializeField]
    private PanelType type;
    [SerializeField]
    private string path;

    public PanelName Name
    {
        get
        {
            return name;
        }
    }

    public PanelType Type
    {
        get
        {
            return type;
        }
    }

    public string Path
    {
        get
        {
            return path;
        }
    }
}

public class ConfigInfo
{
    public List<PanelConfigInfo> panels;
}