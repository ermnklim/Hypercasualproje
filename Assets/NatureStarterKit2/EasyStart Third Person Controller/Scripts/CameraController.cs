using UnityEngine;

public class CameraControllerMobile : MonoBehaviour
{

    [Tooltip("The higher it is, the faster the camera moves.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up, and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    [Header("Dynamic Joystick")]
    [Tooltip("Dynamic joystick used for camera movement on mobile.")]
    public DynamicJoystick cameraJoystick;

    private float mouseX;
    private float mouseY;
    private float offsetDistanceY;

    private Transform player;

    void Start()
    {
        // Find the player and set the camera's initial offset
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = transform.position.y;
    }

    void Update()
    {
        // Follow player with an offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Camera rotation using joystick or mouse
        if ( cameraJoystick != null)
        {
            // Use joystick input for camera movement
            mouseX += cameraJoystick.Horizontal * sensitivity;
            mouseY += cameraJoystick.Vertical * sensitivity; // Invert for natural movement
        }


        // Clamp vertical rotation
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // Apply rotation to the camera
        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

    // Utility function to detect if the platform is mobile

}
