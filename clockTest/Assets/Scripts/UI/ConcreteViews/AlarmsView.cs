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
    public class AlarmsView : View
    {
        [SerializeField]
        private Button _setAlarmBtn;
        [SerializeField]
        private Button _changeWatchBtn;
        [SerializeField]
        private TMP_InputField _nameInput;

        [Header("Digital watch")]
        [SerializeField]
        private GameObject _digitanWatchPnl;
        [SerializeField]
        private TMP_InputField _hoursDigitalInput;
        [SerializeField]
        private TMP_InputField _minutesDigitalInput;
        [SerializeField]
        private Sprite _analogSprite;

        [Header("Analog watch")]
        [SerializeField]
        private GameObject _analogWatchPnl;
        [SerializeField]
        private Sprite _digitalSprite;


        private AlarmClockController _alarmClockController;
        private bool _isDigital = true;
        private Image _buttonImage;
        public override void Initialize()
        {
            base.Initialize();
            SystemManager.Get(out _alarmClockController);
            _setAlarmBtn.onClick.AddListener(() => SetAlarm());
            _changeWatchBtn.onClick.AddListener(() => ChangeMode());
            _buttonImage = _changeWatchBtn.GetComponent<Image>();
        }

        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            ResetInputs();
        }

        private void ResetInputs()
        {
            _nameInput.text = "";
            _hoursDigitalInput.text = "00";
            _minutesDigitalInput.text = "00";
            _buttonImage.sprite = _analogSprite;
        }

        private void ChangeMode()
        {
            if (_isDigital)
            {
                _digitanWatchPnl.SetActive(false);
                _analogWatchPnl.SetActive(true);
                _isDigital = false;
            }
            else
            {
                _digitanWatchPnl.SetActive(true);
                _analogWatchPnl.SetActive(false);
                _isDigital = true;
            }
        }

        private void SetAlarm()
        {
            if (_isDigital)
            {
                string timeString = $"{_hoursDigitalInput.text}:{_minutesDigitalInput.text}" ; // строка с часами и минутами
                DateTime time = DateTime.ParseExact(timeString, "HH:mm", null);
                _alarmClockController.AddAlarm(time, _nameInput.text);

            }
            else
            {

            }
            ViewManager.Show<MainClockView>();
        }
    }
}
