using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class bloodCleanup : MonoBehaviour
{
    public float size;
    public float fadeSpeed;
    public float delay;
    DecalProjector DP;
    // Start is called before the first frame update
    void Start()
    {
        DP = GetComponent<DecalProjector>();
        StartCoroutine(cleanup());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator cleanup()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {

            DP.size = new Vector3(size, size, 100f);

            size -= Time.deltaTime * fadeSpeed;

            if (size <= 0f)
            {
                Destroy(gameObject);
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
