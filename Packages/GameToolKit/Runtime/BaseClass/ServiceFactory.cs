using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// 管理器工厂
    /// </summary>
    public class ServiceFactory : SingletonBase<ServiceFactory>
    {
        Dictionary<System.Type, IService> managers = new Dictionary<System.Type, IService>();

        /// <summary>
        /// 注册管理器
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public void Register<TService, TActualService>() 
            where TService : IService 
            where TActualService : TService
        {
            var type = typeof(TService);
            if (managers.ContainsKey(type))
            {
                Debug.LogWarning($"{type}已存在");
            }
            else
            {
                AddManager(type, System.Activator.CreateInstance<TActualService>());
            }
        }

        /// <summary>
        /// <inheritdoc cref="Register{TService}"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="creator">创建函数</param>
        public void Register<TService>(System.Func<TService> creator) 
            where TService : IService
        {
            var type = typeof(TService);
            if (managers.ContainsKey(type))
            {
                Debug.LogWarning($"{type}已存在");
            }
            else
            {
                AddManager(type, creator());
            }
        }

        void AddManager(System.Type type, IService service)
        {
            managers.Add(type, service);
            service.Init();
        }

        /// <summary>
        /// 获取管理器
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public TService GetService<TService>() 
            where TService : class, IService
        {
            var type = typeof(TService);
            if(managers.TryGetValue(type, out var manager))
            {
                return manager as TService;
            }
            else
            {
                Debug.LogError($"{type}缺失");
                return default;
            }
        }
    }
}
