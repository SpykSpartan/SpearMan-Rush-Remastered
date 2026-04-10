using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTracker : MonoBehaviour
{
    public GameObject[] enemies;
    public powerupManager PM;
    public gameManager GM;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator checkEnemies()
    {

        while (true)
        {
            float dead = 0;
            yield return new WaitForSeconds(5f);

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    dead++;
                }


            }

            if (dead == 0)
            {
                PM.assignPowerup();
                StartCoroutine(sceneswap());
            }

        }
    }

    IEnumerator sceneswap()
    {
        yield return new WaitForSeconds(10);

        GM.GoToMap();

    }
}
