using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGrab : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Instantiate and add grabbable
        gameObject.AddComponent<NearInteractionGrabbable>();
        // Add ability to drag by re-parenting to pointer object on pointer down
        var pointerHandler = gameObject.AddComponent<PointerHandler>();
        pointerHandler.OnPointerDown.AddListener((e) =>
        {
            if (e.Pointer is SpherePointer)
                transform.parent = ((SpherePointer)(e.Pointer)).transform;
        });
        pointerHandler.OnPointerUp.AddListener((e) =>
        {
            if (e.Pointer is SpherePointer)
                transform.parent = null;
        });
    }
}
