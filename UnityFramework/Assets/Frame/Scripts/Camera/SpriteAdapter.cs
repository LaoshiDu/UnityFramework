using System;
using UnityEngine;

namespace WKC
{
    /// <summary>
    /// 直接挂载使用
    /// 正交相机：拥有SpriteRenderer对象不受Size影响，始终保持屏幕比例，优先保证上下填充
    /// 透视相机：拥有SpriteRenderer对象不受FOV影响，始终保持屏幕比例，优先保证上下填充
    /// </summary>
    public class SpriteAdapter : MonoBehaviour
    {
        private Transform _selfTrans;

        private bool _isOrthographic = true;

        private Camera _cam;

        private SpriteRenderer _spriteRenderer;

        private Vector3 _localScreenPos;

        private float _distance;

        private Vector3 _localScale;

        private Vector2 _baseCamSize;

        private Vector2 _oneVec = Vector2.one;

        private Vector2 _baseScale;

        private Vector2 _cameraSize;

        private Vector3[] _corners = new Vector3[4];

        private Action ResiezeEventHandler;

        public bool IsOrthographic
        {
            get
            {
                return _isOrthographic;
            }
            set
            {
                _isOrthographic = value;
                if (_isOrthographic)
                {
                    ResiezeEventHandler = new Action(ResizeOrt);
                    _baseCamSize = new Vector2(_cam.aspect, 1f);
                    return;
                }
                ResiezeEventHandler = new Action(ResiezeProj);
            }
        }

        private void Awake()
        {
            _cam = Camera.main;
            _selfTrans = transform;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _distance = Vector3.Distance(_selfTrans.position, _cam.transform.position);
            _localScale = _selfTrans.localScale;
            _localScreenPos = _cam.WorldToScreenPoint(_selfTrans.position);
            IsOrthographic = _cam.orthographic;
        }

        private void LateUpdate()
        {
            Action resiezeEventHandler = ResiezeEventHandler;
            if (resiezeEventHandler == null)
            {
                return;
            }
            resiezeEventHandler();
        }

        private void ResizeOrt()
        {
            _cameraSize = _baseCamSize * _cam.orthographicSize * 2f;
            if (_cameraSize.x >= _cameraSize.y)
            {
                _baseScale = _oneVec * _cameraSize.x / _spriteRenderer.sprite.bounds.size.x;
            }
            else
            {
                _baseScale = _oneVec * _cameraSize.y / _spriteRenderer.sprite.bounds.size.y;
            }
            _selfTrans.localScale = _baseScale;
            _selfTrans.localPosition = _cam.ScreenToWorldPoint(_localScreenPos);
        }

        private void ResiezeProj()
        {
            _selfTrans.localPosition = _cam.ScreenToWorldPoint(_localScreenPos);
            float num = 2f * _distance * Mathf.Tan(_cam.fieldOfView * 0.5f * 0.017453292f);
            _selfTrans.localScale = _localScale * num;
        }
    }
}