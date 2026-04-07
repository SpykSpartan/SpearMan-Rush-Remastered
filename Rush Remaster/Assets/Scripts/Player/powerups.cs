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

    bool isBuffed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isBuffed)
        {
            StartCoroutine(StartRegen());
        }
    }

    bool IsNotMoving()
    {
        float threshold = 0.1f;

        bool rbStill = true;
        if (rb != null)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rbStill = flatVel.magnitude < threshold;
        }

        bool ccStill = true;
        if (controller != null)
        {
            Vector3 flatVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            ccStill = flatVel.magnitude < threshold;
        }

        return rbStill && ccStill;
    }

    IEnumerator StartRegen()
    {
        isBuffed = true;
            Debug.Log("buffed");
        if (powerupIndex == 1)
        {
            stats.damageMultiplier = 2f;
            stats.healthMultiplier = 2f;
        }
        else if (powerupIndex == 2)
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
