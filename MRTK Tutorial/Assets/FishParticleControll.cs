using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishParticleControll : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem splash;
    bool setupDone = false;
    FishCircle Fish;
    Transform FishTransform;
    float waterHeight;
    
    void Start()
    {
        ps = transform.GetChild(0).GetComponent<ParticleSystem>();
        splash = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (setupDone)
        {
            transform.position = new Vector3(FishTransform.position.x, waterHeight-0.0001f, FishTransform.position.z);
            transform.rotation = Quaternion.Euler(0, FishTransform.rotation.y, 0);


            if (Fish.emit)
            {
                
                if (!ps.isEmitting)
                {
                    ps.enableEmission = true;
                    
                }
            }
            else
            {
                if (ps.isEmitting)
                {
                    ps.enableEmission = false;
                }
            }

            if (Fish.splashNextFrame)
            {
                splash.Play();
            }
        }
    }

    public void Setup(FishCircle fishCircel, float waterHeight)
    {
        Fish = fishCircel;
        FishTransform = fishCircel.transform;
        this.waterHeight = waterHeight;
        setupDone = true;
    }
}
