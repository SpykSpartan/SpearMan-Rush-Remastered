using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    public GameObject bloodVortex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(bloodVortex, transform.position + new Vector3(0f, 5f, 0f), Quaternion.Euler(new Vector3(90, 0, 0)));
        Destroy(gameObject);
    }
}
