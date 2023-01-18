using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 movementVelocity;
    
    [SerializeField] private Joystick joystick;   
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float elevationAmount = 10f;
    [SerializeField] private float velocityDifferenceLimit = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (movementVelocity != Vector3.zero)
        {
            Vector3 velocityDifference = (movementVelocity - rb.velocity);
            velocityDifference.x = Mathf.Clamp(velocityDifference.x, -velocityDifferenceLimit, velocityDifferenceLimit);
            velocityDifference.y = 0f;
            velocityDifference.z = Mathf.Clamp(velocityDifference.z, -velocityDifferenceLimit, velocityDifferenceLimit);
            rb.AddForce(velocityDifference, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        float verticalInput = joystick.Vertical;
        float horizontalInput = joystick.Horizontal;
        movementVelocity = (transform.right * horizontalInput + transform.forward * verticalInput).normalized * movementSpeed;
        transform.rotation = Quaternion.Euler(joystick.Vertical * movementSpeed * elevationAmount, 0f, -1 * joystick.Horizontal * movementSpeed * elevationAmount);
    }
}