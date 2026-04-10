using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WispController : MonoBehaviour
{
    public float size;
    float currSize;
    public string[] dialogue;
    public Transform playerPos;
    public GameObject dialogueObj;
    public bool started;
    public MeshRenderer MR;
    Material Mat;
    // Start is called before the first frame update
    void Start()
    {
        Mat = MR.material;
        currSize = size;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerPos != null)
        {
            Vector3 facing = transform.position - playerPos.position;
            facing = facing.normalized;
            //facing *= -1f;

            transform.forward = facing;
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

            if (dialogue[i] == "Penis")
            {
                dialogueObj.SetActive(false);
                this.StopAllCoroutines();
                StartCoroutine (die());
            }
        }
        
    }

    IEnumerator die()
    {
        while(currSize / size >= 0)
        {
            Debug.Log("dying");
            yield return new WaitForSeconds(Time.deltaTime);

            currSize -= Time.deltaTime;

            Mat.SetVector("_sphere", new Vector4(0, 0, 0, Mathf.Lerp(0f, size, currSize / size)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!started)
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
}
