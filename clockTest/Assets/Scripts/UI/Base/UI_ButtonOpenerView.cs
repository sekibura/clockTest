
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Sekibura.ClockInterview.UI
{
    public class UI_ButtonOpenerView : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private View _view;

        [SerializeField]
        private bool _remember = true;
        [SerializeField]
        private bool _hideLast = true;



        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                ViewManager.Show(_view, _remember, _hideLast);
            });
        }

    }
}
