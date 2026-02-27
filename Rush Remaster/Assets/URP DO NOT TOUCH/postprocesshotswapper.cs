using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class postprocesshotswapper : MonoBehaviour
{
    public UniversalRendererData URD;
    public List<ScriptableRendererFeature> RFs;
    public bool Dark = false;
    public bool Purple = false;
    public bool Weird = false;
    // Start is called before the first frame update
    void Start()
    {
        RFs = URD.rendererFeatures;
    }

    // Update is called once per frame
    void Update()
    {
        RFs[0].SetActive(Dark);
        RFs[1].SetActive(Purple);
        RFs[2].SetActive(Weird);
    }
}
