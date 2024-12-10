using UnityEngine;

public class TouchController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    private Camera mainCamera;

    [Header("Movement Settings")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float fixedYPosition;
    [SerializeField] private float returnSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 0.5f;

    [Header("Touch Control Settings")]
    [SerializeField] private float doubleTapMaxInterval = 0.5f;
    [SerializeField] private float controlDuration = 10f;

    // Private movement variables
    private Vector3 touchOffset;
    private bool isDragging = false;
    private Vector3 originalPosition;

    // Private rotation variables
    private float previousTwoFingerAngle;
    private bool isRotating = false;

    // Double tap tracking
    private float lastTapTime;
    private bool isControlEnabled = false;

    // Trigger tracking
    private bool isTriggered = false;

    // Timer for control duration
    private float controlEndTime;

    private void Start()
    {
        mainCamera = Camera.main;
        fixedYPosition = transform.position.y;
        originalPosition = transform.position;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("No player reference found. Please assign a player or add Player tag.");
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        HandleDoubleTap();

        if (isControlEnabled && Time.time >= controlEndTime)
        {
            DisableControl();
        }

        if (isControlEnabled)
        {
            if (Input.touchCount == 1)
            {
                HandleSingleTouchMovement();
            }
            else if (Input.touchCount == 2)
            {
                HandleTwoFingerRotation();
            }
            else
            {
                isDragging = false;
                isRotating = false;
            }

            if (!isDragging)
            {
                EnforceMaxDistance();
            }
        }
    }

    private void HandleDoubleTap()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                float currentTapTime = Time.time;

                if (currentTapTime - lastTapTime <= doubleTapMaxInterval)
                {
                    EnableControl();
                }

                lastTapTime = currentTapTime;
            }
        }
    }

    private void EnableControl()
    {
        isControlEnabled = true;
        controlEndTime = Time.time + controlDuration;
        Debug.Log("Controls Enabled");
    }

    private void DisableControl()
    {
        isControlEnabled = false;
        isDragging = false;
        isRotating = false;
        Debug.Log("Controls Disabled");
    }

    private void HandleSingleTouchMovement()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = new Vector3(touch.position.x, touch.position.y, 0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                Ray ray = mainCamera.ScreenPointToRay(touchPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    Vector3 worldPos = GetTouchWorldPosition(touchPosition);
                    touchOffset = new Vector3(
                        transform.position.x - worldPos.x,
                        0,
                        transform.position.z - worldPos.z
                    );
                }
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    Vector3 worldPosition = GetTouchWorldPosition(touchPosition);
                    Vector3 targetPosition = new Vector3(
                        worldPosition.x + touchOffset.x,
                        fixedYPosition,
                        worldPosition.z + touchOffset.z
                    );

                    Vector3 directionToTarget = targetPosition - player.position;
                    directionToTarget.y = 0;

                    if (directionToTarget.magnitude > maxDistance)
                    {
                        targetPosition = player.position + directionToTarget.normalized * maxDistance;
                        targetPosition.y = fixedYPosition;
                    }

                    transform.position = targetPosition;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;

                if (isTriggered)
                {
                    transform.position = originalPosition;
                    Debug.Log("Returned to original position due to trigger.");
                }
                break;
        }
    }

    private void HandleTwoFingerRotation()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            previousTwoFingerAngle = GetTwoFingerAngle(touch0.position, touch1.position);
            isRotating = true;
        }
        else if (isRotating && (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved))
        {
            float currentAngle = GetTwoFingerAngle(touch0.position, touch1.position);
            float angleDifference = Mathf.DeltaAngle(previousTwoFingerAngle, currentAngle);

            transform.Rotate(0, -angleDifference * rotationSpeed, 0, Space.World);

            previousTwoFingerAngle = currentAngle;
        }
    }

    private void EnforceMaxDistance()
    {
        Vector3 directionToPlayer = transform.position - player.position;
        directionToPlayer.y = 0;

        if (directionToPlayer.magnitude > maxDistance)
        {
            Vector3 targetPosition = player.position + directionToPlayer.normalized * maxDistance;
            targetPosition.y = fixedYPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * returnSpeed);
        }
    }

    private float GetTwoFingerAngle(Vector2 finger1, Vector2 finger2)
    {
        Vector2 direction = finger2 - finger1;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetTouchWorldPosition(Vector3 touchPosition)
    {
        float distanceFromCamera = Vector3.Distance(mainCamera.transform.position, transform.position);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, distanceFromCamera));
        return worldPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, maxDistance);
        }
    }
}
