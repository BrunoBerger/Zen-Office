using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EyeTrackTry : MonoBehaviour
{
    public Transform lookAtSphere;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Eye Calibration valid? " + CoreServices.InputSystem.EyeGazeProvider.IsEyeCalibrationValid);
        lookAtSphere.position= CoreServices.InputSystem.EyeGazeProvider.HitPosition;
    }
}
