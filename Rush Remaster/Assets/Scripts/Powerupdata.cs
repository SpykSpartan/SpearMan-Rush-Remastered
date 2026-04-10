using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class Powerupdata : ScriptableObject
{
    public float powerupIndex = 0;
    public bool powered = false;

}
