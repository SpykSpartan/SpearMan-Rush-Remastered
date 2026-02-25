using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodOrbController : MonoBehaviour
{
    public GameObject bloodOrb;
    public float bloodOrbForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            spawnBloodOrb();
        }
    }

    public void spawnBloodOrb()
    {
        for(float i = 1; i < 1.5f; i += 0.1f)
        {
            GameObject newOrb = Instantiate(bloodOrb.gameObject, transform.position + transform.forward * 2, Quaternion.identity);

            Rigidbody ORB = newOrb.GetComponent<Rigidbody>();

            ORB.AddExplosionForce(bloodOrbForce * i, transform.position + (Random.onUnitSphere), 5f);
        }
        
    }
}
