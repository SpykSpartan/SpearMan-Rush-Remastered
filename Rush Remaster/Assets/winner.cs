 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winner : MonoBehaviour
{
    public GameObject winUI;
    private void OnDestroy()
    {
        winUI.SetActive(true);
    }
}
