using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMasks : MonoBehaviour
{
    public bool currentlyScalingMask;
    float tipHoldTime;
    float tipReleasedTime=0;
   [SerializeField, Range(0, 5)] float minTipHoldTime;
    [SerializeField, Range(0f, 0.1f)] float minThumbIndexDist;
    [SerializeField, Range(0f, 0.1f)] float minDistBetweenHands;

    [SerializeField] GameObject maskPrefab;
    List<GameObject> placedMasks;

    Transform leftFingerTip;
    Transform rightFingerTip;
    Transform leftThumb;
    Transform rightThumb;


    Transform mainCamera;
    //Debug
    public float leftIndexThumbDistance;
    public float rightIndexThumbDistance;

   

    // Start is called before the first frame update
    void Start()
    {
        currentlyScalingMask = false;
        tipHoldTime = 0;
        placedMasks = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup < 3)
            return;

        mainCamera = Camera.main.transform;

        //Check if fingertips are near, if yes then create a new mask
        var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        if (handJointService != null)
        {
            leftFingerTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Left);
            rightFingerTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);
            leftThumb = handJointService.RequestJointTransform(TrackedHandJoint.ThumbTip, Handedness.Left);
            rightThumb = handJointService.RequestJointTransform(TrackedHandJoint.ThumbTip, Handedness.Right);
            leftIndexThumbDistance = Vector3.Distance(leftFingerTip.position, leftThumb.position);
            rightIndexThumbDistance = Vector3.Distance(rightFingerTip.position, rightThumb.position);
            float tipDistance = Vector3.Distance(leftFingerTip.position, rightFingerTip.position);

            Vector3 indexCenter = (leftFingerTip.position + rightFingerTip.position) *0.5f;

            Vector3 camToIndexCenter = indexCenter - mainCamera.position;

            float offAngleToLookingCenter = Vector3.Angle(mainCamera.forward, camToIndexCenter);





            if (tipDistance < minDistBetweenHands && offAngleToLookingCenter< 30)
                tipHoldTime += Time.deltaTime;
            else
            {
                tipHoldTime = 0;
            }





            if (!currentlyScalingMask && tipHoldTime > minTipHoldTime && leftIndexThumbDistance < minThumbIndexDist && rightIndexThumbDistance < minThumbIndexDist)
            {
                GameObject newMask = SpawnMask();

                newMask.GetComponent<NearInteractionGrabbable>().enabled = false;
                newMask.GetComponent<ObjectManipulator>().enabled = false;
                StartCoroutine(ScaleMask(newMask));
                tipHoldTime = 0;
            }

        }
    }
    public GameObject SpawnMask()
    {
        GameObject newMask = Instantiate(
            original: maskPrefab,
            position: leftFingerTip.position,
            rotation: new Quaternion(0, 0, 0, 0),
            parent: transform
        );
        placedMasks.Add(newMask);
        return newMask;
    }

    IEnumerator ScaleMask(GameObject mask)
    {
        currentlyScalingMask = true;
        //Vector3 startPos = leftFingerTip.position;
        //var tempGameObject = new GameObject();
        //Transform tmpParent = tempGameObject.transform;

        //tmpParent.position = startPos - mask.transform.lossyScale * 0.5f;
        //mask.transform.parent = tmpParent;
        //tmpParent.position = startPos;
        tipReleasedTime = 0;




            bool toFlipLater = false;
        while (tipReleasedTime < 0.15f)
        {

            if(leftIndexThumbDistance < minThumbIndexDist && rightIndexThumbDistance < minThumbIndexDist)
            {
                tipReleasedTime = 0;
            }
            else
            {
                tipReleasedTime += Time.deltaTime;
            }



            tipHoldTime = 0; // to keep from spawning new masks at the same time
            //Vector3 vecToLeft = leftFingerTip.position - startPos;
            //Vector3 vecToRight = rightFingerTip.position - startPos;

            //// no idea why five works perfectly  //answer: because the child has a scale of 0.2f
            //tmpParent.localScale = new Vector3(vecToRight.x, vecToRight.y, vecToRight.z) * 5;

            

            Vector3 lp = (leftFingerTip.position + leftThumb.position) / 2;
            Vector3 rp = (rightFingerTip.position + rightThumb.position) / 2;

            Vector3 center = (lp+rp)/ 2f;
            Vector3 headJoint = mainCamera.position + mainCamera.rotation * new Vector3(0, -0.10f, -0.075f);
            Vector3 centerToCamera = (headJoint - center).normalized;

            //left to right finger
            Vector3 ltrf = rightFingerTip.position - leftFingerTip.position;


            Vector3 helpDirectionRightwards = Vector3.Cross(centerToCamera, Vector3.up).normalized;

            Vector3 relativeFoward = Vector3.Cross(helpDirectionRightwards, ltrf/2);

            Vector3 usedUpward = -Vector3.Cross(helpDirectionRightwards, relativeFoward);

            Vector3 usedRightward = ltrf / 2 - usedUpward;

            //Vector3 relativeUpwards = Vector3.Cross(centerToCamera, relativeRightwards);

            //Vector3 cross = Vector3.Cross(ltrf, centerToCamera);

            if (Mathf.Abs(Vector3.SignedAngle(relativeFoward, centerToCamera, Vector3.up)) > 90)
            {
                toFlipLater = true;
            }
            else
            {
                toFlipLater = false;
            }



            mask.transform.localScale= new Vector3(usedRightward.magnitude * 2, usedUpward.magnitude * 2, 0.03f);

            mask.transform.rotation = Quaternion.LookRotation(relativeFoward, usedUpward);

            mask.transform.position = center;

            if (toFlipLater)
            {
                mask.transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            {
                mask.transform.rotation *= Quaternion.Euler(0, 0, 180);
            }


            Debug.DrawRay(center, relativeFoward, Color.blue, 0.03f);
            Debug.DrawRay(center, usedUpward, Color.green, 0.03f);
            Debug.DrawRay(center, usedRightward, Color.red, 0.03f);

            //Vector3 direction = Vector3.Cross(rightFingerTip.position, leftFingerTip.position);
            //tmpParent.LookAt(direction * 360 * Mathf.PI);
            //tmpParent.transform.eulerAngles = new Vector3(50, 50, 50);

            yield return null;
        }
        //mask.transform.parent = null;
        //Destroy(tempGameObject);
        currentlyScalingMask = false;
        //if (toFlipLater)
        //{
        //    mask.transform.rotation *= Quaternion.Euler(0, 180, 0);
        //}

        


        yield return new WaitForSeconds(1); // to not immediatly trigger a new interaktion
        mask.GetComponent<NearInteractionGrabbable>().enabled = true;
        mask.GetComponent<ObjectManipulator>().enabled = true;
    }

    public void DeleteMasks()
    {
        foreach (GameObject mask in placedMasks)
        {
            Destroy(mask);
        }
        placedMasks.RemoveAll(m => m == null);
    }
}
