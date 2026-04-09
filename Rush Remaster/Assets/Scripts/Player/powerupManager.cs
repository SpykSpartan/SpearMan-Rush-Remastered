using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerupManager : MonoBehaviour
{
    public bool hasPower = false;
    public int power = 0;
    public powerups player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Spearman").GetComponent<powerups>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void assignPowerup()
    {
        power = Random.Range(1, 2);
        hasPower = true;
        player.powerupIndex = power;
    }
}
