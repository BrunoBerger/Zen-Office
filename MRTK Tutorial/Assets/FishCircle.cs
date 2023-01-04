using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCircle : MonoBehaviour
{
    float radius;
    float circlesPerSecond;
    Vector3 startPos;

    float circleTime = 0;
    float speed = 0.02f;

    bool initialized = false;
    
    // Start is called before the first frame update
    public void Init(float radius)
    {
        startPos = transform.position;
        this.radius = radius;
        circlesPerSecond = speed / radius;
        circleTime = Random.Range(0f, 1f);
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            circleTime += circlesPerSecond * Time.deltaTime;
            if (circleTime > 1) circleTime -= 1;

            transform.position = startPos + new Vector3(Mathf.Sin(circleTime * Mathf.PI * 2) * radius, 0, Mathf.Cos(circleTime * Mathf.PI * 2) * radius);
            transform.rotation = Quaternion.Euler(0, circleTime * 360 +90, 0);
        }
    }
}
