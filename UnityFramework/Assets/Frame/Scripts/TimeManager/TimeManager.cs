using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WKC
{
    public class TimeManager : Singleton<TimeManager>
    {
        /// <summary>
        /// �ӳٵ��ú���
        /// </summary>
        /// <param name="delay">�ӳ�ʱ��</param>
        /// <param name="interval">�ظ����õļ��ʱ��</param>
        /// <param name="fun"></param>
        /// <param name="repeatCount">�ظ�������С��0 = ���޴Σ�</param>
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

        public UnityAction update;
        private void Update()
        {
            update?.Invoke();
        }
    }
}