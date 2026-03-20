using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class menuShaker : MonoBehaviour
{
    Vector3 basePos;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = basePos;

        Vector3 temp = transform.position;

        temp.x += Mathf.Sin(Time.time * speed);
        temp.y += Mathf.Cos(Time.time * (speed / 4f));

        transform.position = temp;

    }
}
