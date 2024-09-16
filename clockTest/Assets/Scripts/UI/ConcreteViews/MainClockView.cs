using PimDeWitte.UnityMainThreadDispatcher;
using Sekibura.ClockInterview.System;
using StarGames.Digger.System;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sekibura.ClockInterview.UI
{
    public class MainClockView : View
    {
        [SerializeField]
        private Button _syncTimeBtn;


        [Header("Digital")]
        [SerializeField]
        private TMP_Text _digitalWatch;
        [Header("Analog")]
        [SerializeField]
        private GameObject _hourArrow;
        [SerializeField]
        private GameObject _minuteArrow;
        [SerializeField]
        private GameObject _secondsArrow;

        private TimeController _timeController;
        public override void Initialize()
        {
            base.Initialize();
            SystemManager.Get(out _timeController);
            _timeController.TimeUpdatedAction += UpdateWatches;
            InitDigitalWatches();
        }

        private void InitDigitalWatches()
        {
            _syncTimeBtn.onClick.AddListener(_timeController.SyncTime);
        }

        private void UpdateWatches(DateTime dateTime)
        {
            UpdateDigitalWatch(dateTime);
            UpdateAnalogWatches(dateTime);
        }

        private void UpdateDigitalWatch(DateTime dateTime)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _digitalWatch.text = dateTime.ToString("HH:mm:ss");
            });
        }

        private void UpdateAnalogWatches(DateTime currentTime)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                float hoursAngle = (currentTime.Hour % 12 + currentTime.Minute / 60f) * 30f - 90;  // 360 градусов / 12 часов = 30 градусов на час
                _hourArrow.transform.localRotation = Quaternion.Euler(0, 0, -hoursAngle);

                float minutesAngle = (currentTime.Minute + currentTime.Second / 60f) * 6f - 90;    // 360 градусов / 60 минут = 6 градусов на минуту
                _minuteArrow.transform.localRotation = Quaternion.Euler(0, 0, -minutesAngle);

                float secondsAngle = currentTime.Second * 6f - 90;                                 // 360 градусов / 60 секунд = 6 градусов на секунду
                _secondsArrow.transform.localRotation = Quaternion.Euler(0, 0, -secondsAngle);

            });
        }
    }
}
