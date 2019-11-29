using UnityEngine;

public class Plane2 : MonoBehaviour
{
    public float maxThrottle = 100.0F;
    public float minThrottle = 10F;
    protected float currentThrottle;

    public float throttleSpeed = 10f;
    public float turnSpeed = 80f;

    public Rigidbody rb;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        currentThrottle += throttleSpeed * vertical * Time.deltaTime;
        float turnAmount = turnSpeed * horizontal * Time.deltaTime;

        if (currentThrottle > maxThrottle)
            currentThrottle = maxThrottle;
        else if (currentThrottle < minThrottle)
            currentThrottle = minThrottle;

        Vector3 velocity = transform.up * currentThrottle * throttleSpeed;

        transform.Rotate(Vector3.back, turnAmount);
        rb.velocity = velocity;

        Debug.Log("Final Throttle: " + currentThrottle + ", Vertical Input: " + vertical);
    }
}