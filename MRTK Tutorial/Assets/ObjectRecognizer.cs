//changed, original from https://localjoost.github.io/using-azure-custom-vision-object/ (31.10.2022)

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomVison;
//using HoloToolkitExtensions.Messaging; //ME
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectRecognizer : MonoBehaviour
{
    [SerializeField]
    private string _liveDataUrl = "<your custom vision app url here>";

    [SerializeField]
    private string _predictionKey = "<your prediction key here>";

    private ObjectLocalizer localizer;
    private void Start()
    {
        //Messenger.Instance.AddListener<PhotoCaptureMessage>(p => RecognizeObjects(p.Image, p.CameraResolution, p.CameraTransform)); //ME
        localizer = transform.GetComponent<ObjectLocalizer>();
    }

    public virtual void RecognizeObjects(IList<byte> image, Resolution cameraResolution, Transform cameraTransform)
    {
        StartCoroutine(RecognizeObjectsInternal(image, cameraResolution, cameraTransform));
    }

    private IEnumerator RecognizeObjectsInternal(IEnumerable<byte> image,
        Resolution cameraResolution, Transform cameraTransform)
    {
        var request = UnityWebRequest.Post(_liveDataUrl, string.Empty);
        request.SetRequestHeader("Prediction-Key", _predictionKey);
        request.SetRequestHeader("Content-Type", "application/octet-stream");
        request.uploadHandler = new UploadHandlerRaw(image.ToArray());
        yield return request.SendWebRequest();
        var text = request.downloadHandler.text;
        var result = JsonConvert.DeserializeObject<CustomVisionResult>(text);
        if (result != null)
        {
            result.Predictions.RemoveAll(p => p.Probability < 0.2);
            Debug.Log("#Predictions = " + result.Predictions.Count);

            //ME
            foreach(Prediction p in result.Predictions)
            {
                Debug.Log("found prediction of tag: " + p.TagName+"   its relative center is: "+ new Vector2((float)(p.BoundingBox.Left + (0.5 * p.BoundingBox.Width)),
            (float)(p.BoundingBox.Top + (0.5 * p.BoundingBox.Height))));
            }
            //Messenger.Instance.Broadcast( //ME
            //    new ObjectRecognitionResultMessage(result.Predictions, cameraResolution, cameraTransform));
            localizer.LabelObjects(result.Predictions, cameraResolution, cameraTransform);
        }
        else
        {
            Debug.Log("Predictions is null");
        }
    }
}
