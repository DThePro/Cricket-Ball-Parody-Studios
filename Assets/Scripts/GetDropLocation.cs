using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class GetDropLocation : MonoBehaviour
{
    public GameObject pointerPrefab;
    public Transform groundPlane;
    public GameObject ConfirmCanvas, BowlingCanvas;
    public CinemachineCamera confirmCam, bowlingCam;
    public BowlingController bowlingController;

    private GameObject pointerInstance;

    // World space boundaries
    private readonly Vector3 minBounds = new Vector3(-2f, -2.15f, 12f);
    private readonly Vector3 maxBounds = new Vector3(2f, -2.15f, 25f);
    private bool locationConfirmed = false;

    void Start()
    {
        
        ConfirmCanvas.SetActive(true);
        BowlingCanvas.SetActive(false);
        confirmCam.Priority = 1;
        bowlingCam.Priority = 0;

        if (pointerPrefab != null)
        {
            pointerInstance = Instantiate(pointerPrefab);
        }
        
    }

    void Update()
    {
        if (!locationConfirmed && !(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
            MovePointer();
    }

    void MovePointer()
    {
        if (Camera.main == null || groundPlane == null || pointerInstance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(groundPlane.up, groundPlane.position + new Vector3(0f, 0.1f, 0f)); // Offset for accuracy

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);

            // Clamp within the defined world-space bounds
            worldPos.x = Mathf.Clamp(worldPos.x, minBounds.x, maxBounds.x);
            worldPos.y = minBounds.y; // Keep Y constant
            worldPos.z = Mathf.Clamp(worldPos.z, minBounds.z, maxBounds.z);

            pointerInstance.transform.position = worldPos;
        }
    }

    public void ConfirmPosition()
    {
        Debug.Log("Pointer clicked at: " + pointerInstance.transform.position);
        ConfirmCanvas.SetActive(false);
        BowlingCanvas.SetActive(true);
        confirmCam.Priority = 0;
        bowlingCam.Priority = 1;
        locationConfirmed = true;
        bowlingController.dropLocation = pointerInstance.transform.position;
        // Destroy(pointerInstance);
    }
}
