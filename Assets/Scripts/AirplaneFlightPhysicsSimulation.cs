using UnityEngine;
using UnityEngine.InputSystem;

public class AirplaneFlightPhysicsSimulation : MonoBehaviour
{
    [Header("Engine")]
    public float thrust = 30f;

    [Header("Lift")]
    public float liftCoefficient = 0.02f;
    public float stallAngle = 25f;
    public float stallLiftMultiplier = 0.3f;

    [Header("Drag")]
    public float dragCoefficient = 0.02f;
    public float sideDrag = 0.8f;

    [Header("Control")]
    public float pitchPower = 8f;
    public float rollPower = 6f;
    public float yawPower = 0f;
    public float turnStrength = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, 0f, -0.3f);
    }

    void FixedUpdate()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.spaceKey.isPressed)
        {
            rb.AddRelativeForce(Vector3.forward * thrust, ForceMode.Acceleration);
        }

        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        if (forwardSpeed > 5f)
        {
            float lift = forwardSpeed * forwardSpeed * liftCoefficient;

            float angle = Vector3.Angle(
                transform.forward,
                Vector3.ProjectOnPlane(transform.forward, Vector3.up)
            );

            if (angle > stallAngle)
            {
                lift *= stallLiftMultiplier;
            }

            rb.AddForce(transform.up * lift, ForceMode.Acceleration);
        }

        rb.AddForce(-rb.linearVelocity * dragCoefficient, ForceMode.Acceleration);

        Vector3 sideVel = Vector3.Dot(rb.linearVelocity, transform.right) * transform.right;
        rb.AddForce(-sideVel * sideDrag, ForceMode.Acceleration);

        float pitch = 0f;
        float roll = 0f;

        // S = เชิดหัวขึ้น, W = กดหัวลง
        if (kb.sKey.isPressed) pitch = -1f;
        if (kb.wKey.isPressed) pitch = 1f;

        if (kb.aKey.isPressed) roll = 1f;
        if (kb.dKey.isPressed) roll = -1f;

        rb.AddRelativeTorque(
            new Vector3(pitch * pitchPower, 0f, -roll * rollPower),
            ForceMode.Acceleration
        );
    }
}