using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameToolKit.EventProcessor
{
    public class EventProcessorManager
    {
        public static EventProcessorManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventProcessorManager();
                }
                return _instance;
            }
        }
        static EventProcessorManager _instance;
        /// <summary>
        /// 工作中的处理器
        /// </summary>
        List<AutomaticProcessor> _runningProcessorList;
        private EventProcessorManager() { }
        /// <summary>
        /// 设置处理器
        /// </summary>
        /// <param name="processor"></param>
        public void AttachProcessor(AutomaticProcessor processor)
        {
            processor.Attach();
            _runningProcessorList.Add(processor);
        }
        /// <summary>
        /// 移除处理器
        /// </summary>
        /// <param name="processor"></param>
        public void DetachProcessor(AutomaticProcessor processor)
        {
            processor.Detach();
            _runningProcessorList.Remove(processor);
        }
        /// <summary>
        /// 获取运行中的处理器列表
        /// </summary>
        /// <returns></returns>
        public List<AutomaticProcessor> GetEventProcessors()
        {
            return new List<AutomaticProcessor>(_runningProcessorList);
        }
    }
}
