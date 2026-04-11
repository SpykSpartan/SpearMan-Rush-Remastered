using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerupManager : MonoBehaviour
{
    public bool hasPower = false;
    public int power = 0;
    public powerups player;
    public Powerupdata PD;

    public GameObject screechTxt;
    public GameObject creechTxt;
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
        PD.powerupIndex = Random.Range(1, 3);

        if(PD.powerupIndex == 1)
        {
            screechTxt.SetActive(true);
        }
        else 
        { 
            creechTxt.SetActive(true);
        }
            PD.powered = true;
        //player.powerupIndex = power;
    }
}
