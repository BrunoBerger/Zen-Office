using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishCircle : MonoBehaviour
{
    //public ParticleSystem particleSystem0;
    //public ParticleSystem particleSystem1;
    public AudioSource audioSource;

    [HideInInspector]
    public bool emit = false;
    float radius;
    float circlesPerSecond;
    Vector3 startPos;

    float circleTime = 0;
    float speed = 0.02f;

    bool initialized = false;
    bool prepareHopSound = false;

    YAction yAction = YAction.Paused;
    float actionTime = 1f;

    float usedYRange = 0;

    float usedYActionDuration = 0.1f;
    //float pauseDuration = 0.1f;
    readonly float shortDuration = 2.8f;
    readonly float longDuration = 5.3f;
    readonly float hopDuration = 1.0f;

    readonly float waterLevelDif = 0.01f;

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

            actionTime +=  Time.deltaTime/ usedYActionDuration;
            if (actionTime > 1) StartRandomNextYAction();

            float yOff = (-Mathf.Cos(actionTime * Mathf.PI * 2) + 1f) * usedYRange;
            float ySpeed = -Mathf.Sin(actionTime * Mathf.PI * 2) *usedYRange/ usedYActionDuration;

            float rotAroundX = Mathf.Atan2(ySpeed, speed)*Mathf.Rad2Deg;

            transform.position = startPos + new Vector3(Mathf.Sin(circleTime * Mathf.PI * 2) * radius, yOff, Mathf.Cos(circleTime * Mathf.PI * 2) * radius);
            transform.rotation = Quaternion.Euler(rotAroundX, circleTime * 360 +90, 0);

            if(Mathf.Abs(yOff - waterLevelDif) < 0.003f)
            {
                emit = true;
                
            }
            else
            {
                emit = false;
            }

            if (Mathf.Abs(yOff - waterLevelDif) < 0.008f)
            {
                if (prepareHopSound && actionTime > 0.5f)
                {
                    prepareHopSound = false;
                    audioSource.pitch = Random.Range(0.8f, 1.0f);
                    audioSource.Play();
                }
            }
        }
    }



    void StartRandomNextYAction()
    {
        float selector = Random.value;

        actionTime = 0;
        if(selector <= 0.25f)//Pause
        {
            yAction = YAction.Paused;
            usedYActionDuration = Random.Range(0.5f, 3.2f);
            usedYRange = 0;
        }
        else if(selector <= 0.5f)//longRise
        {
            yAction = YAction.longRise;
            usedYActionDuration = longDuration;
            usedYRange = 0.005f;
        }
        else if (selector <= 0.5f)//shortRise
        {
            yAction = YAction.shortRise;
            usedYActionDuration = shortDuration;
            usedYRange = 0.005f;
        }
        else//jump
        {
            yAction = YAction.jump;
            usedYActionDuration = hopDuration;
            usedYRange = 0.03f;
            prepareHopSound = true;
        }



    }

    enum YAction
    {
        Paused, //25%
        longRise, //25%
        shortRise, //25%
        jump, //25%
    }

}
