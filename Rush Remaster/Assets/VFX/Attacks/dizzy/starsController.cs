using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starsController : MonoBehaviour
{
    public GameObject[] stars;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
        foreach(GameObject star in stars)
        {
            star.transform.Rotate(new Vector3(0, Time.deltaTime * (speed * 0.5f), 0));
        }
    }

    public void activate()
    {
        foreach(GameObject star in stars)
        {
            star.SetActive(true);

            StartCoroutine(deactivate());
        }
    }

    IEnumerator deactivate()
    {
        yield return new WaitForSeconds(3f);

        foreach(GameObject star in stars)
        {
            star.SetActive(false);
        }
    }
}
