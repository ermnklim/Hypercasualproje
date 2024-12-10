using UnityEngine;
public class JoystickPlayerExample : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 5f;
    public Joystick joystick;
    public Rigidbody rb;
    public Animator animator;
    public Transform cameraTransform;
    public bool isGroundBoundaryActive = false;
    public Transform groundTransform;
    private Vector2 groundBoundarySize;
    private Vector3 groundCenter;
    private MeshCollider groundCollider;
    private bool isBoundaryInitialized = false;

    private void Start()
    {
        InitializeGroundBoundary();
    }

    private void InitializeGroundBoundary()
    {
        if (isGroundBoundaryActive && groundTransform != null)
        {
            groundCollider = groundTransform.GetComponent<MeshCollider>();
            if (groundCollider == null)
            {
                Debug.LogError("Ground object must have a MeshCollider component!");
                isGroundBoundaryActive = false;
                return;
            }

            // Calculate boundary size using mesh bounds
            Bounds meshBounds = groundCollider.sharedMesh.bounds;
            groundBoundarySize = new Vector2(
                meshBounds.size.x * groundTransform.lossyScale.x,
                meshBounds.size.z * groundTransform.lossyScale.z
            );
            
            // Calculate world space center
            groundCenter = groundTransform.TransformPoint(meshBounds.center);
            isBoundaryInitialized = true;

            Debug.Log($"Ground Boundaries Initialized: Size({groundBoundarySize.x}, {groundBoundarySize.y}), Center({groundCenter})");
        }
    }

    private void FixedUpdate()
    {
        if (isGroundBoundaryActive && !isBoundaryInitialized)
        {
            InitializeGroundBoundary();
        }

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        if (horizontal == 0 && vertical == 0)
        {
            if (animator != null)
                animator.SetBool("isRunning", false);

            rb.velocity = Vector3.zero;
        }
        else
        {
            Vector3 direction = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            direction.y = 0;

            Vector3 velocity = direction.normalized * speed;
            
            if (isGroundBoundaryActive && isBoundaryInitialized)
            {
                // Check and adjust current position
                Vector3 currentPosition = ClampPositionToGround(transform.position);
                if (currentPosition != transform.position)
                {
                    transform.position = currentPosition;
                    rb.position = currentPosition;
                }

                // Predict and check next position
                Vector3 nextPosition = transform.position + new Vector3(velocity.x, 0, velocity.z) * Time.fixedDeltaTime;
                if (!IsPositionWithinBounds(nextPosition))
                {
                    Vector3 clampedNextPosition = ClampPositionToGround(nextPosition);
                    velocity = (clampedNextPosition - transform.position) / Time.fixedDeltaTime;
                }
            }

            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

            if (animator != null)
                animator.SetBool("isRunning", true);
        }
    }

    private bool IsPositionWithinBounds(Vector3 position)
    {
        if (!isBoundaryInitialized) return true;

        float margin = 0.1f;
        float halfWidth = groundBoundarySize.x * 0.5f - margin;
        float halfLength = groundBoundarySize.y * 0.5f - margin;

        return position.x >= (groundCenter.x - halfWidth) &&
               position.x <= (groundCenter.x + halfWidth) &&
               position.z >= (groundCenter.z - halfLength) &&
               position.z <= (groundCenter.z + halfLength);
    }

    private Vector3 ClampPositionToGround(Vector3 position)
    {
        if (!isBoundaryInitialized) return position;

        float margin = 0.1f;
        float halfWidth = groundBoundarySize.x * 0.5f - margin;
        float halfLength = groundBoundarySize.y * 0.5f - margin;

        float clampedX = Mathf.Clamp(position.x, 
            groundCenter.x - halfWidth, 
            groundCenter.x + halfWidth);
            
        float clampedZ = Mathf.Clamp(position.z, 
            groundCenter.z - halfLength, 
            groundCenter.z + halfLength);

        return new Vector3(clampedX, position.y, clampedZ);
    }

    private void OnValidate()
    {
        isBoundaryInitialized = false;
    }
}