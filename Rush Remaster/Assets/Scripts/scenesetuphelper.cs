using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scenesetuphelper : MonoBehaviour
{
    public RespawnManager RS;
    public string spawnpointname;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Spearman")
        {
            RS.RD.spawnPos = spawnpointname;
        }
    }
}
