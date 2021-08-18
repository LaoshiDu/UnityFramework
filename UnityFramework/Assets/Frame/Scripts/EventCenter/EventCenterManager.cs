using System.Collections.Generic;

namespace WKC
{
    public delegate void EventCall(params object[] args);
    public class EventCenterManager : BaseMgr<EventCenterManager>
    {
        public Dictionary<EventName, EventCall> events = new Dictionary<EventName, EventCall>();

        /// <summary>
        /// 添加监听函数
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="fun"></param>
        public void AddEventListener(EventName eventName, EventCall fun)
        {
            if (!events.ContainsKey(eventName))
            {
                events.Add(eventName, fun);
            }
            else
            {
                events[eventName] += fun;
            }
        }

        /// <summary>
        /// 移除监听函数
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="fun"></param>
        public void RemoveEventListener(EventName eventName, EventCall fun)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName] -= fun;
            }
        }

        /// <summary>
        /// 事件分发
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="args"></param>
        public void SendEvent(EventName eventName, params object[] args)
        {
            if (events.ContainsKey(eventName))
            {
                events[eventName]?.Invoke(args);
            }
        }
    }
}