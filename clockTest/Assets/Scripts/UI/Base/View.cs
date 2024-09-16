using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sekibura.ClockInterview.UI
{
    public abstract class View : MonoBehaviour
    {
        public Button BackButton;
        public virtual void Initialize()
        {
            if (BackButton != null)
                BackButton.onClick.AddListener(() => {
                    ViewManager.ShowLast();
                    OnBackButton();
                });
        }

        public virtual void OnBackButton() { }
        public virtual void Hide() => gameObject.SetActive(false);
        public virtual void Show(object parameter = null) => gameObject.SetActive(true);

    }
}