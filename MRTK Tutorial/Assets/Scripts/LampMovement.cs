using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampMovement : MonoBehaviour
{
    readonly float maxRotSpeed = 10;
    readonly float maxRotPhase = 1;
    float rotDuration;
    float myRotationStart;
    float rotSpeed;

    readonly float maxYSpeed = 0.030f;
    float ySpeed;
    float phase;

    // Start is called before the first frame update
    void Start()
    {
        ySpeed = Random.Range(-maxYSpeed, maxYSpeed);
        phase = Random.Range(0f, 2f);

        rotDuration = Random.Range(maxRotPhase/2, maxRotPhase) * (Random.Range(0, 2) - 0.5f) * 2f;
        myRotationStart = Random.Range(0, 360);
        rotSpeed = Random.Range(maxRotSpeed/2, maxRotSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        float yChange = Mathf.Sin(phase + Time.time) * Time.deltaTime * ySpeed;
        transform.position += new Vector3(0, yChange, 0);

        float rotChange = Mathf.Cos(Time.time * rotDuration) * rotSpeed;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x, 
            myRotationStart + rotChange, 
            transform.rotation.eulerAngles.z);
    }
}
