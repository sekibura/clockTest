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
        public void Initialize()
        {
            SystemManager.Get(out _timeController);
            _timeController.TimeNewMinuteUpdateAction += CheckAlarms;
        }

        public void AddAlarm(DateTime alarmTime, string name)
        {
            _alarms.Clear();
            _alarms.Add(new AlarmClock(alarmTime, name));
            Debug.Log($"ƒÓ·‡‚ÎÂÌ ·Û‰ËÎ¸ÌËÍ Ì‡ {alarmTime.Hour}:{alarmTime.Minute}");
        }

        private void CheckAlarms(DateTime alarmDateTime)
        {
            foreach (AlarmClock alarm in _alarms)
            {
                if(alarm.Time.Hour == alarmDateTime.Hour && alarm.Time.Minute == alarmDateTime.Minute)
                {
                    //ViewManager.Show<AlarmSirenView>(alarm);
                    //_alarms.Clear();
                    Debug.Log($"—–¿¡Œ“¿À ¡”ƒ»À‹Õ» ! {alarm.Time.Hour}:{alarm.Time.Minute}");
                    break;
                }
            }
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
