using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //variables
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;
    private float xRotation;

    public Transform playerCamera;
    public Rigidbody playerBody;
    public float moveSpeed = 6;
    public float verticalSpeed = 10;
    public float sensitivity = 2.0f;

    void Update()
    {
        //get movement inputs
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MovePlayerCamera();
    }
/*
Function to move the player object
*/
    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * moveSpeed;
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);//calculate velocity

        //move the camera up if the space key is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            playerBody.transform.Translate(Vector3.up * verticalSpeed * Time.deltaTime);
        }

        //move the camera down if the left shift key is pressed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerBody.transform.Translate(Vector3.down * verticalSpeed * Time.deltaTime);
        }
    }
/*
Function to move the player camera.
*/
    private void MovePlayerCamera()
    {
        transform.Rotate(0, playerMouseInput.x * sensitivity, 0);

        xRotation -= playerMouseInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//clamp to avoid flipping upside down
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
