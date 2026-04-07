using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bashController : MonoBehaviour
{
    public Vector3 finalSize;
    public float timer;
    public GameObject parent;
    float time = 0;
    Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(parent, timer);
        startScale = transform.localScale;

        StartCoroutine(blowup());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator blowup()
    {
        while (true)
        {

            yield return new WaitForSeconds(Time.deltaTime);
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, finalSize, time / timer);
        }
    }
}
