//changed, original from https://localjoost.github.io/using-azure-custom-vision-object/ (02.11.2022)

using System.Collections.Generic;
using CustomVison;
//using HoloToolkit.Unity.SpatialMapping;
//using HoloToolkit.UX.ToolTips;
//using HoloToolkitExtensions.Messaging;
using UnityEngine;

public class ObjectLocalizer: MonoBehaviour
{
    //private List<GameObject> _createdObjects = new List<GameObject>();

    //ME
    [SerializeField]
    private LayerMask meshCopyLayer;
    [HideInInspector]
    public List<TypeAtPos> detectedObjectList;
    private ObjectReplacer objReplacer;
    //[SerializeField]
    //private GameObject _labelObject;

    //[SerializeField]
    //private GameObject _labelContainer;


    //[SerializeField]
    //private string _labelText = "Toy aircraft";

    //[SerializeField]
    //private GameObject _debugObject;

    private void Start()
    {
        //ME
        detectedObjectList = new List<TypeAtPos>();
        objReplacer = transform.GetComponent<ObjectReplacer>();
        //Messenger.Instance.AddListener<ObjectRecognitionResultMessage>(
        //    p => LabelObjects(p.Predictions, p.CameraResolution, p.CameraTransform));
    }

    public virtual void LabelObjects(IList<Prediction> predictions,
        Resolution cameraResolution, Transform cameraTransform)
    {
        //ClearLabels();
        var heightFactor = cameraResolution.height / cameraResolution.width;
        var topCorner = cameraTransform.position + cameraTransform.forward -
                        cameraTransform.right / 2f +
                        cameraTransform.up * heightFactor / 2f;
        foreach (var prediction in predictions)
        {
            var center = GetCenter(prediction);//prediction.GetCenter(); //ME
            var recognizedPos = topCorner + cameraTransform.right * center.x -
                                cameraTransform.up * center.y * heightFactor;
            //#if UNITY_EDITOR
            //            _createdObjects.Add(CreateLabel(_labelText, recognizedPos));
            //#endif
            var labelPos = DoRaycastOnSpatialMap(cameraTransform, recognizedPos);
            if (labelPos != null)
            {
                //_createdObjects.Add(CreateLabel(_labelText, labelPos.Value));
                detectedObjectList.Add(new TypeAtPos(prediction.TagName, (Vector3)labelPos));//ME
                objReplacer.PlaceObjectOfTagAt(prediction.TagName, (Vector3)labelPos);
            }

        }

        //if (_debugObject != null)
        //{
        //    _debugObject.SetActive(false);
        //}

        Destroy(cameraTransform.gameObject);
    }

    private Vector3? DoRaycastOnSpatialMap(Transform cameraTransform, Vector3 recognitionCenterPos)
    {
        RaycastHit hitInfo;

        if (//SpatialMappingManager.Instance != null &&
            Physics.Raycast(cameraTransform.position, (recognitionCenterPos - cameraTransform.position),
                out hitInfo, 10, meshCopyLayer))// SpatialMappingManager.Instance.LayerMask)) //ME
        {
            return hitInfo.point;
        }
        return null;
    }

    //private void ClearLabels()
    //{
    //    foreach (var label in _createdObjects)
    //    {
    //        Destroy(label);
    //    }
    //    _createdObjects.Clear();
    //}

    //ME
    //private GameObject CreateLabel(string text, Vector3 location)
    //{
    //    var labelObject = Instantiate(_labelObject);
    //    var toolTip = labelObject.GetComponent<ToolTip>();
    //    toolTip.ShowOutline = false;
    //    toolTip.ShowBackground = true;
    //    toolTip.ToolTipText = text;
    //    toolTip.transform.position = location + Vector3.up * 0.2f;
    //    toolTip.transform.parent = _labelContainer.transform;
    //    toolTip.AttachPointPosition = location;
    //    toolTip.ContentParentTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //    var connector = toolTip.GetComponent<ToolTipConnector>();
    //    connector.PivotDirectionOrient = ConnectorOrientType.OrientToCamera;
    //    connector.Target = labelObject;
    //    return labelObject;
    //}

    public Vector2 GetCenter(Prediction p) //NOTE: this method came from another class, the "this"-Keyword was removed from "GetCenter(this Predeiction p)"
    {
        return new Vector2((float)(p.BoundingBox.Left + (0.5 * p.BoundingBox.Width)),
            (float)(p.BoundingBox.Top + (0.5 * p.BoundingBox.Height)));
    }
}



//ME
public struct TypeAtPos
{
    public string tag;
    public Vector3 pos;
    public Vector3 scale;
    public TypeAtPos(string tag, Vector3 pos)
    {
        this.tag = tag;
        this.pos = pos;
        this.scale = new Vector3(1, 1, 1);
    }
    public TypeAtPos(string tag, Vector3 pos, Vector3 scale)
    {
        this.tag = tag;
        this.pos = pos;
        this.scale = scale;
    }
    //private static ClassifyTag SetTagByString(string tagString)
    //{
    //    ClassifyTag tag;
    //    switch (tagString)
    //    {
    //        case "can":
    //            tag = ClassifyTag.can;
    //            break;
    //        case "cup":
    //            tag = ClassifyTag.cup;
    //            break;
    //        case "bowl":
    //            tag = ClassifyTag.bowl;
    //            break;
    //        default:
    //            Debug.Log("WARNING! Custom Vision detected an object tagged as: " + tagString + "  this tag is not defined in this project! Tag was saved as ClassifyTag.ERROR");
    //            tag = ClassifyTag.ERROR;
    //            break;
    //    }
    //    return tag;
    //}
}

//ME
//public enum ClassifyTag
//{
//    ERROR,
//    can,
//    cup,
//    bowl
//}


