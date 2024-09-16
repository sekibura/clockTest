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
    /// Единая точка доступа ко всем системам 
    /// Содержит 2 метода:
    ///     1) Register - для регистрации системы
    ///     2) Get - для получения инстанса системы
    /// 
    /// Хранит список всех инстансов систем либо функций, которые используются для создания систем
    ///  
    /// Перед тем, как получить систему (через метод Get) её необходимо зарегистрировать.
    /// Возможны 2 варианта регистрации:
    ///     1) Регистрации уже созданного объекта Register(object obj) - объект просто добавится в словарь с инстансами систем.
    ///     2) Регистрация системы с указанием функции, используемой при создании объекта Register<T>(Func<T> creator) либо Register<T>() (для систем, которые имеют конструктор без параметров)
    ///        В этом случае, объект будет создан только при первом вызове Get
    /// </summary>
    /// 
    public class SystemManager
    {
        /// <summary>
        /// Словарь с созданными подсистемами
        /// </summary>
        private static Dictionary<Type, object> _systems = new Dictionary<Type, object>(32);

        /// <summary>
        /// Словарь с функциями для создания подсистем
        /// </summary>
        private static Dictionary<Type, Func<object>> _creators = new Dictionary<Type, Func<object>>(32);

        private static bool _getConstructsSystem;

        /// <summary>
        /// Зарегистрировать новую систему, создаваемую при первом вызове Get, с указанием функции создания экземпляра
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="creator"></param>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если система такого типа уже была зарегистрирована
        /// </exception>
        public static void Register<T>(Func<T> creator) where T : class
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (_creators.ContainsKey(type) || _systems.ContainsKey(type))
            {
                Debug.LogError($"Система {type} уже была зарегистрирована!");
                return;
            }
#endif

            _creators.Add(typeof(T), () => creator());
        }

        /// <summary>
        /// Зарегистрировать новую систему, создаваемую при первом вызове Get, для типов, которые имеют конструктор без параметров
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если система такого типа уже была зарегистрирована
        /// </exception>
        public static void Register<T>() where T : class, new()
        {
            Register<T>(() => new T());
        }

        /// <summary>
        /// Зарегистрировать уже созданную систему
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="ArgumentException">
        /// Выбрасывается, если система такого типа уже была зарегистрирована
        /// </exception>
        public static void Register<T>(T obj)
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (_creators.ContainsKey(type) || _systems.ContainsKey(type))
            {
                Debug.LogError($"Система {type} уже была зарегистрирована!");
                return;
            }
#endif

            _systems.Add(typeof(T), obj);
        }

        /// <summary>
        /// Получить систему указанного типа. Система должна быть предварительно зарегистрирована.
        /// Если был зарегистрирован экземпляр системы, то метод вернёт его.
        /// Если была зарегистрирована функция для создания экземпляра системы, то будет вызван он и метод вернёт созданный экземпляр.
        /// Создаваемая система не должна вызывать Get в конструкторе или Awake.
        /// В этом случае можно использовать либо Start, либо реализовать интерфейс IInitializable с методом Initialize.
        /// Методы Initialize и Start уже могут использовать Get, но полученная система может быть ещё не проинициализирована.
        /// </summary>
        /// <exception cref="SystemException">
        /// Выбрасывается, если в конструкторе системы обнаружен вложенный вызов Get.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Выбрасывается, если запрашиваемая система не зарегистрирована.
        /// </exception>
        public static T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_getConstructsSystem)
            {
                Debug.LogError("Вызывать Get из конструктора другой системы запрещено. Используйте MonoBehaviour.Start или реализуйте IInitializable.");
                return null;
            }

            object result;
            if (!_systems.TryGetValue(type, out result))
            {
                Func<object> creator;
                if (!_creators.TryGetValue(type, out creator))
                {
                    Debug.LogError($"Система типа {typeof(T).Name} не была зарегистрирована!");
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
        /// Получить систему указанного типа. Система должна быть предварительно быть зарегистрирована.
        /// Если был зарегистрирован экземпляр системы, то метод вернёт его.
        /// Если была зарегистрирована функция для создания экземпляра системы, то будет вызван он и метод вернёт созданный экземпляр.
        /// Создаваемая система не должна вызывать Get в конструкторе или Awake.
        /// В этом случае можно использовать либо Start, либо реализовать интерфейс IInitializable с методом Initialize.
        /// Методы Initialize и Start уже могут использовать Get, но полученная система может быть ещё не проинициализирована.
        /// </summary>
        /// <exception cref="SystemException">
        /// Выбрасывается, если в конструкторе системы обнаружен вложенный вызов Get.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Выбрасывается, если запрашиваемая система унаследована от UnityEngine.Object и не зарегистрирована.
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