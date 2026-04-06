using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionController : MonoBehaviour
{
    public float finalSize;
    public float timer;
    float time = 0;
    public float startScale;
    float size;
    public AnimationCurve curve;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer + 1f);

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
            size = Mathf.Lerp(startScale, finalSize, curve.Evaluate(time / timer));

            mat.SetVector("_sphere", new Vector4(0, 0, 0, size));

        }
    }
}
