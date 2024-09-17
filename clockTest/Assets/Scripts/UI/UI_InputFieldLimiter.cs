using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sekibura.ClockInterview.UI
{
    public class UI_InputFieldLimiter : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private int _min;
        [SerializeField]
        private int _max;

        void Start()
        {
            inputField.onEndEdit.AddListener(ValidateInput);
            ValidateInput(_min.ToString());
        }

        void ValidateInput(string input)
        {
            int value;
            if (int.TryParse(input, out value))
            {
                value = Mathf.Clamp(value, _min, _max); // ������������ �� 0 �� 100
                inputField.text = value.ToString("D2");
            }
            else
            {
                inputField.text = _min.ToString("D2"); // ���� ����� �� �����, ���������� � 0
            }
        }
    }
}
