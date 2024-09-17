using Sekibura.ClockInterview.System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace StarGames.Digger.System
{
    public class ApplicationController : MonoBehaviour
    {
        private static bool _isLoaded = false;

        void Awake()
        {
            if (_isLoaded)
                return;

            _isLoaded = true;
            SystemManager.Register(this);
            RegisterSystems();
            GetSystems();
            SetApplicationSettings();
        }

        private void RegisterSystems()
        {
            SystemManager.Register<TimeController>();
            SystemManager.Register<AlarmClockController>();
        }

        private void GetSystems()
        {
            SystemManager.Get<TimeController>();
            SystemManager.Get<AlarmClockController>();
        }

        private void SetApplicationSettings()
        {
#if UNITY_ANDROID || UNITY_IOS
            SetMobileSettings();
#endif
        }

        private void SetMobileSettings()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 30;
        }

        private void OnDestroy()
        {
            SystemManager.Get<TimeController>().Dispose();
            SystemManager.Dispose();

            _isLoaded = false;
        }
    }
}