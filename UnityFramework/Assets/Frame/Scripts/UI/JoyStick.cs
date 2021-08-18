using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WKC
{
    public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public JoyStickType stickType = JoyStickType.Static;
        public RectTransform bg;
        public RectTransform bar;
        /// <summary>
        /// 是否触摸了虚拟摇杆
        /// </summary>
        private bool isTouched = false;

        /// <summary>
        /// 摇杆最大半径,以像素为单位
        /// </summary>
        public float JoyStickRadius = 50;

        /// <summary>
        /// 注册触摸过程事件
        /// </summary>
        public Action<Vector2, bool> OnJoyStickTouchMove;

        public void OnDrag(PointerEventData eventData)
        {
            isTouched = true;
            bg.gameObject.SetActive(true);

            Vector3 worldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(bar, eventData.position, eventData.pressEventCamera, out worldPosition))
            {
                bar.position = worldPosition;
            }

            if (stickType == JoyStickType.Dynamic)
            {
                float dis = Vector3.Distance(bar.position, bg.position);
                if (dis > JoyStickRadius)
                {
                    bar.position = worldPosition;
                    bg.position = worldPosition - (worldPosition - bg.position).normalized * JoyStickRadius;
                }
                else
                {
                    bar.position = worldPosition;
                }
            }
            else
            {
                if (bar.localPosition.magnitude > JoyStickRadius)
                {
                    bar.localPosition = bar.localPosition.normalized * JoyStickRadius;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isTouched = false;
            bg.gameObject.SetActive(true);
            Vector3 worldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(bg, eventData.position, eventData.pressEventCamera, out worldPosition))
            {
                bg.position = worldPosition;
                bar.localPosition = Vector3.zero;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isTouched = false;
            bg.gameObject.SetActive(false);
        }

        public void RemoveEvent()
        {
            OnJoyStickTouchMove = null;
            bar.localPosition = Vector3.zero;
            isTouched = false;
        }

        void Update()
        {
            OnJoyStickTouchMove?.Invoke(bar.localPosition.normalized, isTouched);
        }
    }
}