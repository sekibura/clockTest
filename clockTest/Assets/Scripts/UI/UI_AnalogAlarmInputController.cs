using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sekibura.ClockInterview.UI
{
    public class UI_AnalogAlarmInputController :MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField]
        private RectTransform hourHand;
        [SerializeField]
        private RectTransform minuteHand;
        [SerializeField]
        private TMP_Text timeDisplayHour;
        [SerializeField]
        private TMP_Text timeDisplayMinutes;

        private RectTransform selectedHand = null; 
        private bool isDragging = false;
        private int _lastHour;

        [SerializeField]
        private Button _amPmModeBtn;

        private bool _isAM = true;

        public int Hours { get; private set; }
        public int Minutes { get; private set; }

        public void SwitchAMPMMode()
        {
            _isAM = !_isAM;
            UpdateTimeDisplay();
        }

        public void SelectHourHand()
        {
            selectedHand = hourHand;
            minuteHand.gameObject.GetComponent<Image>().color = Color.black;
            hourHand.gameObject.GetComponent<Image>().color = Color.red;
        }

        public void SelectMinuteHand()
        {
            selectedHand = minuteHand;
            hourHand.gameObject.GetComponent<Image>().color = Color.black;
            minuteHand.gameObject.GetComponent<Image>().color = Color.red;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && selectedHand != null)
            {
                RotateSelectedHand(eventData);
                UpdateTimeDisplay(); 
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
            selectedHand = null;
            minuteHand.gameObject.GetComponent<Image>().color = Color.black;
            hourHand.gameObject.GetComponent<Image>().color = Color.black;
        }
        private void RotateSelectedHand(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint);

            Vector2 handCenter = (Vector2)selectedHand.position;
            Vector2 direction = localPoint - handCenter;

            float angle = Mathf.Atan2(localPoint.y, localPoint.x) * Mathf.Rad2Deg;

            if(selectedHand == hourHand)
                angle = Mathf.Round(angle / 30f) * 30f;
            else
                angle = Mathf.Round(angle / 6f) * 6f;

            selectedHand.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private int GetMinutes()
        {
            float zRotation = minuteHand.eulerAngles.z;
            return EulerAngleToMinutes(zRotation);
        }

        private int GetHours()
        {
            float zRotation = hourHand.eulerAngles.z;
            if (!_isAM)
            {
                var hours = EulerAngleToClockHour(zRotation) + 12;
                return hours == 24? 0: hours;
            }    
                
            else
                return EulerAngleToClockHour(zRotation);
        }

        public int EulerAngleToClockHour(float eulerAngle)
        {
            float angle = eulerAngle % 360;
            int hour = (int)Math.Round((3 - (angle / 30)) % 12);
            if (hour <= 0) hour += 12;
            return hour;
        }

        public int EulerAngleToMinutes(float eulerAngle)
        {
            float angle = eulerAngle % 360;
            int minutes = (int)Math.Round((90 - angle) / 6) % 60;
            if (minutes < 0) minutes += 60;
            return minutes;
        }
        private void UpdateTimeDisplay()
        {
            Hours = GetHours();
            Minutes = GetMinutes();


            timeDisplayHour.text = $"{Hours:D2}";
            timeDisplayMinutes.text = $"{Minutes:D2}";
        }
    }
}
