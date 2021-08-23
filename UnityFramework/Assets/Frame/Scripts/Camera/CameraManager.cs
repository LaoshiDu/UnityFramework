using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WKC
{
    public class CameraManager : BaseMgr<CameraManager>
    {
        public Camera uiCamera;

        public void Init()
        {
            uiCamera = GameObject.Find("Cameras/UICamera").GetComponent<Camera>();
        }
    }
}