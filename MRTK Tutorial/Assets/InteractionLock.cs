using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEditorInternal;
using UnityEngine;

public class InteractionLock : MonoBehaviour
{
    public bool lockInteraction = false;
    private bool lastLockState = false;
    public GameObject IntroCup;
    public GameObject DirLight;
    public DrawMasks DrawMasks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockInteraction == lastLockState) return;
        lastLockState = lockInteraction;
        if (lockInteraction)
        {
            IntroCup.SetActive(false);
            DirLight.transform.GetChild(0).gameObject.SetActive(false);
            DrawMasks.enabled = false;
        }
        else
        {
            IntroCup.SetActive(true);
            DirLight.transform.GetChild(0).gameObject.SetActive(true);
            DrawMasks.enabled = true;
        }
    }
}
