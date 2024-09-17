using PimDeWitte.UnityMainThreadDispatcher;
using Sekibura.ClockInterview.System;
using StarGames.Digger.System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sekibura.ClockInterview.UI
{
    public class UI_AlarmClockPanelController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TMP_Text _nameTxt;
        [SerializeField]
        private TMP_Text _timeAlarm;
        [SerializeField]
        private Button _deleteAlarmBtn;

        private AlarmClockController _alarmClockController;
        private void Start()
        {
            SystemManager.Get(out _alarmClockController);
            _alarmClockController.AlarmAddedAction += (a)=> {
                UnityMainThreadDispatcher.Instance().Enqueue(() => ShowAlarm(a)); };
            _alarmClockController.AlarmClearedAction += () => { UnityMainThreadDispatcher.Instance().Enqueue(() => DisableView()); };
            _deleteAlarmBtn.onClick.AddListener(() => Delete());
            Init();
        }

        private void Init()
        {
            var alarms = _alarmClockController.GetAlarms();
            if (alarms.Count > 0)
                ShowAlarm(alarms[0]);
            else
                DisableView();
        }

        private void ShowAlarm(AlarmClock alarmClock)
        {
            //_nameTxt.text = alarmClock.Name;
            _timeAlarm.text = alarmClock.Time.ToString("HH:mm");
            _deleteAlarmBtn.interactable = true;
            _canvasGroup.alpha = 1;
        }

        private void Delete()
        {
            DisableView();
            _alarmClockController.DeleteAlarms();
        }

        private void DisableView()
        {
            _canvasGroup.alpha = 0;
            _deleteAlarmBtn.interactable = false;
        }
    }
}
