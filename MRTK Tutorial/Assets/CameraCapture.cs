//changed, original from https://localjoost.github.io/using-azure-custom-vision-object/ (31.10.2022)

using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
//using HoloToolkit.Unity.InputModule;
//using HoloToolkitExtensions.Messaging;
using UnityEngine;
//using UnityEngine.XR.WSA.WebCam
using UnityEngine.Windows.WebCam; //ME NOTE: Unity changed the location form UnityEngine.XR.WSA.WebCam to this

public class CameraCapture : MonoBehaviour//, IInputClickHandler  //ME
{
    public bool makePicEveryFiveSecs = false; //ME

    PhotoCapture _photoCaptureObject = null;
    private Resolution _cameraResolution;


    [SerializeField]
    private GameObject _debugPane;

    //ME
    private ObjectRecognizer objectRecognizer;
    private float picCounter = 5;

    // Use this for initialization
    void Start()
    {
        _cameraResolution =
            PhotoCapture.SupportedResolutions.OrderByDescending(res => res.width * res.height).First();

        //ME
        objectRecognizer = transform.GetComponent<ObjectRecognizer>();
    }

    public void TakePicture()
    {
        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            _photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters
            {
                hologramOpacity = 0.0f,
                cameraResolutionWidth = _cameraResolution.width,
                cameraResolutionHeight = _cameraResolution.height,
                pixelFormat = _debugPane != null ? CapturePixelFormat.BGRA32 : CapturePixelFormat.JPEG
            };

            // Activate the camera
            _photoCaptureObject.StartPhotoModeAsync(cameraParameters, p =>
            {
                _photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        var photoBuffer = new List<byte>();

        if (photoCaptureFrame.pixelFormat == CapturePixelFormat.JPEG)
        {
            photoCaptureFrame.CopyRawImageDataIntoBuffer(photoBuffer);
        }
        else
        {
            photoBuffer = ConvertAndShowOnDebugPane(photoCaptureFrame);
        }

        //ME
        //Messenger.Instance.Broadcast(
        //    new PhotoCaptureMessage(photoBuffer, _cameraResolution, CopyCameraTransForm()));
        Debug.Log("took Photo: " + CopyCameraTransform());
        objectRecognizer.RecognizeObjects(photoBuffer, _cameraResolution, CopyCameraTransform());


        // Deactivate our camera
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }

    private List<byte> ConvertAndShowOnDebugPane(PhotoCaptureFrame photoCaptureFrame)
    {
        var targetTexture = new Texture2D(_cameraResolution.width, _cameraResolution.height);
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        Destroy(_debugPane.GetComponent<Renderer>().material.mainTexture);

        _debugPane.GetComponent<Renderer>().material.mainTexture = targetTexture;

        //ME
        _debugPane.transform.localScale = new Vector3(_cameraResolution.width, 1, _cameraResolution.height)  / _cameraResolution.width * 0.03f;
        //_debugPane.transform.parent.gameObject.SetActive(true);


        return new List<byte>(targetTexture.EncodeToJPG());
    }

    //public void OnInputClicked(InputClickedEventData eventData)
    //{
    //    TakePicture();
    //}
    
    //ME
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakePicture();
        }
        if (makePicEveryFiveSecs)
        {
            picCounter -= Time.deltaTime;
            if (picCounter <= 0)
            {
                picCounter += 5;
                TakePicture();
            }
        }
    }


    private Transform CopyCameraTransform()
    {
        var g = new GameObject();

        //ME
        //g.transform.position = CameraCache.Main.transform.position;
        //g.transform.rotation = CameraCache.Main.transform.rotation;
        //g.transform.localScale = CameraCache.Main.transform.localScale;
        g.transform.position = Camera.main.transform.position;
        g.transform.rotation = Camera.main.transform.rotation;
        g.transform.localScale = Camera.main.transform.localScale;

        return g.transform;
    }

}
