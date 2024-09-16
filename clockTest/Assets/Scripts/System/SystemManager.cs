using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarGames.Digger.System
{
    public interface IInitializable
    {
        void Initialize();
    }

    /// <summary>
    /// ������ ����� ������� �� ���� �������� 
    /// �������� 2 ������:
    ///     1) Register - ��� ����������� �������
    ///     2) Get - ��� ��������� �������� �������
    /// 
    /// ������ ������ ���� ��������� ������ ���� �������, ������� ������������ ��� �������� ������
    ///  
    /// ����� ���, ��� �������� ������� (����� ����� Get) � ���������� ����������������.
    /// �������� 2 �������� �����������:
    ///     1) ����������� ��� ���������� ������� Register(object obj) - ������ ������ ��������� � ������� � ���������� ������.
    ///     2) ����������� ������� � ��������� �������, ������������ ��� �������� ������� Register<T>(Func<T> creator) ���� Register<T>() (��� ������, ������� ����� ����������� ��� ����������)
    ///        � ���� ������, ������ ����� ������ ������ ��� ������ ������ Get
    /// </summary>
    /// 
    public class SystemManager
    {
        /// <summary>
        /// ������� � ���������� ������������
        /// </summary>
        private static Dictionary<Type, object> _systems = new Dictionary<Type, object>(32);

        /// <summary>
        /// ������� � ��������� ��� �������� ���������
        /// </summary>
        private static Dictionary<Type, Func<object>> _creators = new Dictionary<Type, Func<object>>(32);

        private static bool _getConstructsSystem;

        /// <summary>
        /// ���������������� ����� �������, ����������� ��� ������ ������ Get, � ��������� ������� �������� ����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="creator"></param>
        /// <exception cref="ArgumentException">
        /// �������������, ���� ������� ������ ���� ��� ���� ����������������
        /// </exception>
        public static void Register<T>(Func<T> creator) where T : class
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (_creators.ContainsKey(type) || _systems.ContainsKey(type))
            {
                Debug.LogError($"������� {type} ��� ���� ����������������!");
                return;
            }
#endif

            _creators.Add(typeof(T), () => creator());
        }

        /// <summary>
        /// ���������������� ����� �������, ����������� ��� ������ ������ Get, ��� �����, ������� ����� ����������� ��� ����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException">
        /// �������������, ���� ������� ������ ���� ��� ���� ����������������
        /// </exception>
        public static void Register<T>() where T : class, new()
        {
            Register<T>(() => new T());
        }

        /// <summary>
        /// ���������������� ��� ��������� �������
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentException">
        /// �������������, ���� ������� ������ ���� ��� ���� ����������������
        /// </exception>
        public static void Register<T>(T obj)
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (_creators.ContainsKey(type) || _systems.ContainsKey(type))
            {
                Debug.LogError($"������� {type} ��� ���� ����������������!");
                return;
            }
#endif

            _systems.Add(typeof(T), obj);
        }

        /// <summary>
        /// �������� ������� ���������� ����. ������� ������ ���� �������������� ����������������.
        /// ���� ��� ��������������� ��������� �������, �� ����� ����� ���.
        /// ���� ���� ���������������� ������� ��� �������� ���������� �������, �� ����� ������ �� � ����� ����� ��������� ���������.
        /// ����������� ������� �� ������ �������� Get � ������������ ��� Awake.
        /// � ���� ������ ����� ������������ ���� Start, ���� ����������� ��������� IInitializable � ������� Initialize.
        /// ������ Initialize � Start ��� ����� ������������ Get, �� ���������� ������� ����� ���� ��� �� �������������������.
        /// </summary>
        /// <exception cref="SystemException">
        /// �������������, ���� � ������������ ������� ��������� ��������� ����� Get.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// �������������, ���� ������������� ������� �� ����������������.
        /// </exception>
        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_getConstructsSystem)
            {
                Debug.LogError("�������� Get �� ������������ ������ ������� ���������. ����������� MonoBehaviour.Start ��� ���������� IInitializable.");
                return null;
            }

            object result;
            if (!_systems.TryGetValue(type, out result))
            {
                Func<object> creator;
                if (!_creators.TryGetValue(type, out creator))
                {
                    Debug.LogError($"������� ���� {typeof(T).Name} �� ���� ����������������!");
                    return null;
                }

                _getConstructsSystem = true;
                object instance = creator();
                _getConstructsSystem = false;

                _creators.Remove(type);

                _systems.Add(type, instance);
                IInitializable initializable = instance as IInitializable;
                if (initializable != null)
                    initializable.Initialize();
                return instance as T;
            }
            return result as T;
        }

        /// <summary>
        /// �������� ������� ���������� ����. ������� ������ ���� �������������� ���� ����������������.
        /// ���� ��� ��������������� ��������� �������, �� ����� ����� ���.
        /// ���� ���� ���������������� ������� ��� �������� ���������� �������, �� ����� ������ �� � ����� ����� ��������� ���������.
        /// ����������� ������� �� ������ �������� Get � ������������ ��� Awake.
        /// � ���� ������ ����� ������������ ���� Start, ���� ����������� ��������� IInitializable � ������� Initialize.
        /// ������ Initialize � Start ��� ����� ������������ Get, �� ���������� ������� ����� ���� ��� �� �������������������.
        /// </summary>
        /// <exception cref="SystemException">
        /// �������������, ���� � ������������ ������� ��������� ��������� ����� Get.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// �������������, ���� ������������� ������� ������������ �� UnityEngine.Object � �� ����������������.
        /// </exception>
        public static void Get<T>(out T obj) where T : class
        {
            obj = Get<T>();
        }

        public static void Dispose()
        {
            _systems.Clear();
            _creators.Clear();
        }
    }
}