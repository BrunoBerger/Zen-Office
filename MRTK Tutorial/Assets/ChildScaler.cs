using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildScaler : MonoBehaviour
{

    public bool hasDecoration= false;
    bool hadDeco = false;
    Vector3 prevScale;
    // Start is called before the first frame update
    void Start()
    {
        prevScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.forward.y) < 1/Mathf.Sqrt(2))
        {
            if (!hadDeco)
            {
                hasDecoration = true;
                hadDeco = true;
                foreach (Transform holder in transform)
                {
                    holder.gameObject.SetActive(true);
                }
            }
            
        }
        else
        {
            if (hadDeco)
            {
                hasDecoration = false;
                hadDeco = false;
                foreach (Transform holder in transform)
                {
                    holder.gameObject.SetActive(false);
                }
            }
        }

        if (hasDecoration)
        {
            if (transform.localScale != prevScale)
            {
                float xs = transform.localScale.x;
                float ys = transform.localScale.y;
                float zs = transform.localScale.z;
                float scale = Mathf.Sqrt(Mathf.Sqrt(xs * ys)) * 0.5f;


                foreach(Transform holder in transform)
                {
                    holder.localScale = new Vector3(1/xs, 1/ys, 1/zs)*scale;
                }




                prevScale = transform.localScale;
            }
        }
    }
}
