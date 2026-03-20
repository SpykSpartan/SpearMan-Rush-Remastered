using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headController : MonoBehaviour
{
    public Material headMaterial;
    public float headscale = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 headTransform = new Vector4(transform.position.x, transform.position.y, transform.position.z, headscale);
        headMaterial.SetVector("_sphere", transform.position);
    }
}
