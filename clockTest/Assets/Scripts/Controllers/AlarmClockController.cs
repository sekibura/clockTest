using PimDeWitte.UnityMainThreadDispatcher;
using Sekibura.ClockInterview.UI;
using StarGames.Digger.System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sekibura.ClockInterview.System
{
    public class AlarmClockController : IInitializable
    {
        private List<AlarmClock> _alarms = new List<AlarmClock>();
        private TimeController _timeController;
        public Action<AlarmClock> AlarmAddedAction;
        public Action AlarmClearedAction;
        public void Initialize()
        {
            SystemManager.Get(out _timeController);
            _timeController.TimeNewMinuteUpdateAction += CheckAlarms;
        }

        public void AddAlarm(DateTime alarmTime, string name)
        {
            DeleteAlarms();
            var alarm = new AlarmClock(alarmTime, name);
            _alarms.Add(alarm);
            Debug.Log($"Добавлен будильник на {alarmTime.Hour}:{alarmTime.Minute} - {alarmTime.ToString()}");
            AlarmAddedAction?.Invoke(alarm);
        }

        private void CheckAlarms(DateTime alarmDateTime)
        {
            Debug.Log("Проверка будильников");
            foreach (AlarmClock alarm in _alarms)
            {
                if((alarm.Time.Hour == alarmDateTime.Hour) && (alarm.Time.Minute == alarmDateTime.Minute))
                {
                    Debug.Log($"СРАБОТАЛ бу ДИЛЬНИК! {alarm.Time.Hour}:{alarm.Time.Minute}");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => ViewManager.Show<AlarmSirenView>(alarm, hideLast:false));

                    DeleteAlarms();
                    
                    break;
                }
            }
        }


        public List<AlarmClock> GetAlarms()
        {
            return _alarms;
        }

        public void DeleteAlarms()
        {
            _alarms.Clear();
            AlarmClearedAction?.Invoke();
        }

        public void Dispose(){}
    }

    public struct AlarmClock
    {
        public DateTime Time;
        public string Name;

        public AlarmClock(DateTime time, string name)
        {
            Time = time;
            Name = name;
        }
    }
}
