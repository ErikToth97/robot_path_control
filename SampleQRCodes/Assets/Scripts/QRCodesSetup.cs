using System;
using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using Microsoft.MixedReality.QR;
namespace QRTracking
{
    public class QRCodesSetup : MonoBehaviour
    {
        public QRCodesManager qrCodesManager;

        bool IsTrackerRunning = false;

        void Awake()
        {
        }

        public void ButtonPressed()
        {
            IsTrackerRunning = qrCodesManager.IsTrackerRunning;
            if (IsTrackerRunning)
            {
                Debug.Log("Tracker stopped");
                qrCodesManager.StopQRTracking();
            }
            else
            {
                qrCodesManager.StartQRTracking();
            }
        }
    }
}
