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
        private TMP_Text _digitalWatch;
        [SerializeField]
        private Button _syncTimeBtn;

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
        }

        private void UpdateDigitalWatch(DateTime dateTime)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _digitalWatch.text = dateTime.ToString("HH:mm:ss");
            });
        }

    }
}
