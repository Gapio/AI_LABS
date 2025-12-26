using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera playerCamera;
    Vector2 input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        input = new Vector2(horizontal, vertical);

        rb.AddForce(playerCamera.transform.forward * input.y * speed);
        rb.AddForce(playerCamera.transform.right * input.x * speed);
    }
}
