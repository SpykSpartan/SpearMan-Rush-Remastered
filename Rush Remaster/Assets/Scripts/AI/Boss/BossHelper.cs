using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHelper : MonoBehaviour
{
    static GameObject boss;
    static GameObject speartip;

    static bool bashnow = false;
    static bool explode = false;
    private void Start()
    {
        boss = GameObject.Find("Asian Boss V1.1");
        speartip = GameObject.Find("Speartip");
    }

    private void Update()
    {
        if (bashnow)
        {
            boss.GetComponent<IronBoss>().StartCoroutine("deployBash");
            bashnow = false;
        }
        if (explode)
        {
            boss.GetComponent<IronBoss>().StartCoroutine("deployExplosion");
            explode = false;
        }
    }

    public static void deployBash()
    {
        bashnow = true;
    }

    public static void deployExplosion()
    {
        explode = true;
    }

}
