using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerups : MonoBehaviour
{
    //1 = Shriek 2 = Roar
    public bool powerupUnlocked = false;
    public int powerupIndex;
    public PlayerStat stats;
    public CharacterController controller;
    public Rigidbody rb;
    public powerupManager powerupManager;

    bool isBuffed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (powerupManager.PD.powered)
        {
            if (Input.GetKeyDown(KeyCode.F) && !isBuffed)
            {
                StartCoroutine(StartRegen());
            }
        }
        
    }

    IEnumerator StartRegen()
    {
        isBuffed = true;
            Debug.Log("buffed");
        if (powerupManager.PD.powerupIndex == 1)
        {
            stats.damageMultiplier = 2f;
            stats.healthMultiplier = 2f;
        }
        else if (powerupManager.PD.powerupIndex == 2)
        {
            stats.speedMultiplier = 2f;
            stats.dashDistanceMultiplier = 2f;

        }
        yield return new WaitForSeconds(30f);

        stats.speedMultiplier = 1f;
        stats.dashDistanceMultiplier = 1f;
        stats.damageMultiplier = 1f;
        stats.healthMultiplier = 1f;

        isBuffed = false;
    }
}
