using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������GC
//[GCOptimize]
public struct LuaBootstrap
{
    public delegate void LifeTime();
    public delegate void UpdateUI(params object[] args);

    public LifeTime Start;
    public LifeTime Update;
    public LifeTime OnDestroy;
    public UpdateUI UpdateShowUI;
}

public class LuaStart : MonoBehaviour
{
    private LuaBootstrap LB;
    void Start()
    {
        LoaderEnvManager.Instance.ExecutionLua("GameStart");
        //��Lua�ű�ӳ�����
        LB = LoaderEnvManager.Instance.Global.Get<LuaBootstrap>("GameStart");
        //ִ��Lua�ű���Start����
        LB.Start();
    }
}