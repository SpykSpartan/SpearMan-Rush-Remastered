using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnshifter : MonoBehaviour
{
    public RespawnManager RS;
    public CharacterController CC;

    // Start is called before the first frame update
    void Start()
    {
        if (RS.RD.spawnPos == "ForgeSpawn")
        {
            CC.transform.position = new Vector3(757f, 24f, 136);
        }
        else if (RS.RD.spawnPos == "BossSpawn")
        {
            CC.transform.position = new Vector3(1004, 13, -55);
        }
        else
        {
            CC.transform.position = new Vector3(681, -33, 471);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
