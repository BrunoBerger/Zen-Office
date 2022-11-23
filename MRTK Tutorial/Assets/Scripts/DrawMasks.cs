using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMasks : MonoBehaviour
{

    [SerializeField] GameObject debugTarget;

    Transform leftFingerPos;
    Transform rightFingerPos;
    [SerializeField] float minTipHoldTime;
    float tipHoldTime;
    [SerializeField] float minTipDistance;

    [SerializeField] GameObject maskPrefab;
    List<GameObject> placedMasks;

    //fingertips
    //private HandJointK[] joints;


    // Start is called before the first frame update
    void Start()
    {  
        tipHoldTime = 0;
        placedMasks = new List<GameObject>();

        //debug cube
        // Instantiate and add grabbable
        debugTarget.AddComponent<NearInteractionGrabbable>();
        // Add ability to drag by re-parenting to pointer object on pointer down
        var pointerHandler = debugTarget.AddComponent<PointerHandler>();
        pointerHandler.OnPointerDown.AddListener((e) =>
        {
            if (e.Pointer is SpherePointer)
            {
                debugTarget.transform.parent = ((SpherePointer)(e.Pointer)).transform;
            }
        });
        pointerHandler.OnPointerUp.AddListener((e) =>
        {
            if (e.Pointer is SpherePointer)
            {
                debugTarget.transform.parent = null;
            }
        });

    }

    // Update is called once per frame
    void Update()
    {
        //Check if fingertips are near, if yes then create a new mask
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            Transform cameraTransf = Camera.main.transform;
            Transform leftTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Left);
            Transform rightTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);
            float distance = Vector3.Distance(leftTip.position, rightTip.position);

            if (distance < minTipDistance)
                tipHoldTime += Time.deltaTime;

            if (tipHoldTime > minTipHoldTime)
            {
                GameObject newMask = Instantiate(
                    original: maskPrefab,
                    position: leftTip.position,
                    rotation: cameraTransf.rotation,
                    parent: transform
                );
                placedMasks.Add(newMask);
                tipHoldTime = 0;
            }

        }
    }

    //public void OnManipulationStarted(ManipulationEventData manipulationEventData)
    //{

    //}
}
