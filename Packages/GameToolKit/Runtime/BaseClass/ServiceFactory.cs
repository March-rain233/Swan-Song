using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameToolKit
{
    /// <summary>
    /// ����������
    /// </summary>
    public class ServiceFactory : SingletonBase<ServiceFactory>
    {
        Dictionary<System.Type, IService> managers = new Dictionary<System.Type, IService>();

        /// <summary>
        /// ע�������
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public void Register<TService, TActualService>() 
            where TService : IService 
            where TActualService : TService
        {
            var type = typeof(TService);
            if (managers.ContainsKey(type))
            {
                Debug.LogWarning($"{type}�Ѵ���");
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
        /// <param name="creator">��������</param>
        public void Register<TService>(System.Func<TService> creator) 
            where TService : IService
        {
            var type = typeof(TService);
            if (managers.ContainsKey(type))
            {
                Debug.LogWarning($"{type}�Ѵ���");
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
        /// ��ȡ������
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
                Debug.LogError($"{type}ȱʧ");
                return default;
            }
        }
    }
}
