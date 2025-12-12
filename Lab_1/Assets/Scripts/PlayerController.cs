using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    InputAction moveAction;
    InputAction lookAction;

    Camera playerCamera;

    float moveSpeed = 5f;
    float rotateSpeed = 31f;
    float yaw = 0f;
    float bitch = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        //move the player based on moveValue

        Vector3 playerMovement = transform.forward * (moveValue.y * moveSpeed) + transform.right * (moveValue.x * moveSpeed);



        characterController.SimpleMove(playerMovement);



        Vector2 lookValue = lookAction.ReadValue<Vector2>();
        //rotate the player based on lookValue

        yaw += lookValue.x * rotateSpeed * Time.deltaTime;
        bitch += -lookValue.y * rotateSpeed * Time.deltaTime;
        bitch = Mathf.Clamp(bitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCamera.transform.rotation = Quaternion.Euler(bitch, yaw, 0f);
    }
}
