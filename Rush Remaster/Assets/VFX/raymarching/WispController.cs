using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WispController : MonoBehaviour
{
    public float size;
    public string[] dialogue;
    public GameObject dialogueObj;
    public bool started;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                started = true;
                dialogueObj.SetActive(true);
                dialogueObj.GetComponent<TextMeshProUGUI>().text = dialogue[0];
                if (dialogue.Length > 1)
                {
                    StartCoroutine(displayDialogue());
                }
            }
        }
        
    }

    IEnumerator displayDialogue()
    {
        int i = 0;
        while(i < dialogue.Length - 1)
        {
            yield return new WaitForSeconds(5);
            i++;
            dialogueObj.GetComponent<TextMeshProUGUI>().text = dialogue[i];
        }
        
    }
}
