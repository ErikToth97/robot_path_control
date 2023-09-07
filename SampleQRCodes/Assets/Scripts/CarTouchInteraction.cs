using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTouchInteraction : MonoBehaviour, IMixedRealityTouchHandler, IMixedRealityPointerHandler
{
    private bool isActive = false;
    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        if (!isActive)
        {
            isActive = true;
            var cubeTM = gameObject.transform.Find("Description");
            cubeTM.gameObject.SetActive(true);
            StartCoroutine(passiveMe(5));
        }
    }


 
    IEnumerator passiveMe(int secs)
    {
        if (isActive)
        {
            isActive = false;
            yield return new WaitForSeconds(secs);
            var cubeTM = gameObject.transform.Find("Description");
            cubeTM.gameObject.SetActive(false);
        }
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) { }
    public void OnTouchUpdated(HandTrackingInputEventData eventData) { }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (!isActive)
        {
            isActive = true;
            var cubeTM = gameObject.transform.Find("Description");
            cubeTM.gameObject.SetActive(true);
            StartCoroutine(passiveMe(5));
        }
    }
}
