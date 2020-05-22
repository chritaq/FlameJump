using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControllerV2 : MonoBehaviour
{
    //private PlayerController player;
    private PlayerController player;

    private DialougeManagerV2 dialougeManager;
    [SerializeField] private DialougeV2 dialouge;

    [SerializeField] private float xDistanceBeforeDialogeCanBeActive;
    private float yDistanceBeforeDialogeCanBeActive = 0.5f;



    [Header("Camera Zoom")]
    [SerializeField] private bool zoomInDuringDialouge;
    private Camera mainCamera;
    [SerializeField] private Transform newCameraTransform;
    [SerializeField] private float zoomInTarget;


    // Start is called before the first frame update
    void Start()
    {
        dialougeManager = DialougeManagerV2.instance;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        mainCamera = Camera.main;
    }

    private float xDistance;
    private float yDistance;
    private void Update()
    {
        xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
        yDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
        if (xDistance < xDistanceBeforeDialogeCanBeActive && yDistance < yDistanceBeforeDialogeCanBeActive)
        {
            if (player.activeMiscCommand == PlayerController.PlayerMiscCommands.Dialouge && player.readCurrentPlayerState().GetType() == new PlayerIdleState().GetType())
            {
                StartDialouge();
            }
        }
    }

    public void StartDialouge()
    {
        player.GoToLockedInputState();
        if (zoomInDuringDialouge)
        {
            StartCoroutine(ZoomIn());
        }
        else
        {
            DialougeBegin();
        }
    }

    //TODO
    //This should be in some kind of cameraController instead
    private Vector3 cameraPositionAndSizeBeforeDialouge;
    private Vector3 newCameraPositionAndSizeDuringDialouge;
    [SerializeField] private float zoomSpeed = 1f;
    private IEnumerator ZoomIn()
    {
        Debug.Log("Started Zoom");
        cameraPositionAndSizeBeforeDialouge = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.orthographicSize);
        newCameraPositionAndSizeDuringDialouge = new Vector3(newCameraTransform.position.x, newCameraTransform.position.y, zoomInTarget);
        Vector3 cameraZoomLerp;
        float lerpStep = 0;
        float smoothLerpStep = 0;
        float realZoomSpeed = zoomSpeed;

        while (mainCamera.orthographicSize > zoomInTarget)
        {
            Debug.Log("Zooming");


            cameraZoomLerp = Vector3.Lerp(cameraPositionAndSizeBeforeDialouge, newCameraPositionAndSizeDuringDialouge, lerpStep);
            //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraTransform.position, Time.deltaTime * zoomSpeed);

            mainCamera.transform.position = new Vector3(cameraZoomLerp.x, cameraZoomLerp.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize = cameraZoomLerp.z;

            lerpStep += zoomSpeed * Time.deltaTime;


            yield return new WaitForEndOfFrame();
        }
        DialougeBegin();
        yield return null;
    }

    private IEnumerator WaitUntilDialougeIsDoneToZoomOut()
    {
        while (dialougeManager.CheckInDialouge())
        {
            yield return new WaitForEndOfFrame();
        }

        Vector3 cameraZoomLerp;
        float lerpStep = 0.1f;

        while (mainCamera.orthographicSize < cameraPositionAndSizeBeforeDialouge.z)
        {
            Debug.Log("Zooming");
            cameraZoomLerp = Vector3.Lerp(newCameraPositionAndSizeDuringDialouge, cameraPositionAndSizeBeforeDialouge, lerpStep);

            //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newCameraTransform.position, Time.deltaTime * zoomSpeed);

            mainCamera.transform.position = new Vector3(cameraZoomLerp.x, cameraZoomLerp.y, mainCamera.transform.position.z);
            mainCamera.orthographicSize = cameraZoomLerp.z;

            lerpStep += zoomSpeed * Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    private void DialougeBegin()
    {
        player.GoToDialougeState();
        dialougeManager.StartDialougeV2(dialouge);
        StartCoroutine(WaitUntilDialougeIsDoneToZoomOut());
    }
}
