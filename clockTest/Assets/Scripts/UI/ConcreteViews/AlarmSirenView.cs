using Sekibura.ClockInterview.System;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sekibura.ClockInterview.UI
{
    public class AlarmSirenView : View
    {
        [SerializeField]
        private TMP_Text _nameTxt;
        [SerializeField]
        private TMP_Text _timeTxt;
        [SerializeField]
        private AudioSource _audioSource;
        public override void Show(object parameter = null)
        {
            base.Show(parameter);
            if(parameter is AlarmClock)
            {
                AlarmClock alarmDateTime = (AlarmClock)parameter;
                _nameTxt.text = string.IsNullOrEmpty(alarmDateTime.Name)? "Без названия": alarmDateTime.Name;
                _timeTxt.text = alarmDateTime.Time.ToString("HH:mm");
                _audioSource.Play();
            }
        }

        public override void Hide()
        {
            base.Hide();
            _audioSource.Stop();
        }
    }
}
