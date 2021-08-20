using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace WKC
{
    public class TimeManager : Singleton<TimeManager>
    {
        /// <summary>
        /// 延迟调用函数
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">重复调用的间隔时间</param>
        /// <param name="fun"></param>
        /// <param name="repeatCount">重复次数（小于0 = 无限次）</param>
        /// <param name="args"></param>
        public void DelayPlayFun(float delay, float interval, EventCall fun, int repeatCount = 1, params object[] args)
        {
            StartCoroutine(IDelayPlayFun(delay, interval, fun, repeatCount, args));
        }

        private IEnumerator IDelayPlayFun(float delay, float interval, EventCall fun, int repeatCount, params object[] args)
        {
            if (repeatCount == 0) yield break;
            yield return new WaitForSeconds(delay);
            fun?.Invoke(args);
            if (repeatCount < 0)
            {
                while (true)
                {
                    yield return new WaitForSeconds(interval);
                    fun?.Invoke(args);
                }
            }
            else
            {
                int count = repeatCount - 1;
                while (count > 0)
                {
                    yield return new WaitForSeconds(interval);
                    fun?.Invoke(args);
                    count--;
                }
            }
        }
        
        public UnityAction UpdateEventHandler;
        public UnityAction FixedUpdateEventHandler;
        public UnityAction LateUpdateEventHandler;
        public UnityAction UpdatePerSecondEventHandler;

        /// <summary>
        /// 零点刷新回调
        /// </summary>
        public event UnityAction<DateTime> DelZeroTime;
        private DateTime _localTime;

        private void Start()
        {
            DelZeroTime += ZeroUpdateTime;
        }

        private float timer = 0;
        private float ONESEC = 1;
        private void Update()
        {
            UpdateEventHandler?.Invoke();

            timer += Time.deltaTime;
            if (timer >= ONESEC)
            {
                timer = 0f;
                UpdatePerSecondEventHandler?.Invoke();
            }

            _localTime = DateTime.UtcNow;
            if (_localTime.Day != RecordDay || _localTime.Month != RecordMonth || _localTime.Year != RecordYear)
            {
                DelZeroTime?.Invoke(_localTime);
            }
        }

        private void FixedUpdate()
        {
            FixedUpdateEventHandler?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateEventHandler?.Invoke();
        }

        private void OnDestroy()
        {
            DelZeroTime -= ZeroUpdateTime;
        }

        #region 零点相关
        private static int _recordDay;
        public static int RecordDay
        {
            set { _recordDay = value; PlayerPrefs.SetInt("dd", _recordDay); }
            get
            {
                if (_recordDay != 0) return _recordDay;
                return _recordDay = PlayerPrefs.GetInt("dd", 1);
            }
        }

        private static int _recordMonth;
        public static int RecordMonth
        {
            set { _recordMonth = value; PlayerPrefs.SetInt("mm", _recordMonth); }
            get
            {
                if (_recordMonth != 0) return _recordMonth;
                return _recordMonth = PlayerPrefs.GetInt("mm", 1);
            }
        }

        private static int _recordYear;
        public static int RecordYear
        {
            set { _recordYear = value; PlayerPrefs.SetInt("yy", _recordYear); }
            get
            {
                if (_recordYear != 0) return _recordYear;
                return _recordYear = PlayerPrefs.GetInt("yy", 1970);
            }
        }

        private void ZeroUpdateTime(DateTime dateTime)
        {
            RecordDay = dateTime.Day;
            RecordMonth = dateTime.Month;
            RecordYear = dateTime.Year;
        }
        #endregion
    }
}