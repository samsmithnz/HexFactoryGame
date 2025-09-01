using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 3f;
    public float maxZoom = 15f;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Set initial position to view the grid
        transform.position = new Vector3(7.5f, 8f, -2f);
        transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }
    
    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }
    
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        direction = transform.TransformDirection(direction);
        direction.y = 0; // Keep movement horizontal
        
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
    
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            Vector3 pos = transform.position;
            pos.y -= scroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }
    
    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0, Space.World);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        }
    }
}