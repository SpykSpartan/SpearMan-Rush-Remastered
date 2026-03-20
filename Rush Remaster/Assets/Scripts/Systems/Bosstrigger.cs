using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseTrigger : MonoBehaviour
{
    public GameObject popupToShow;
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            gameManager.Instance.TriggerPopup(popupToShow);
            hasTriggered = true;
        }
    }
}
