using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    //Character Controller Simple Movement Script with Mouse Look
    public Camera playerCamera;
    public float walkSpeed = 6.0f;
    public float runSpeed = 12.0f;
    public float jumpPower = 7.0f;
    public float gravity = 10f;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    //(later) Added HeadBob Variables 
    public float bobFrequency = 1.5f;
    public float bobHorizontalAmplitude = 0.05f;
    public float bobVerticalAmplitude = 0.05f;
    public float bobSmoothing = 5f;

    Vector3 originalCameraPosition;
    float bobTimer = 0.5f;

    /*//even later HeadBob Landing Variables (later work)
    public float bobLandingIntensity = 0.3f;
    public float bobLandingSmoothing = 5f;
    public float landingDuration = 1f;
    private bool isGrounded;
    private bool wasGrounded;*/

    //Crouch Variuables
    public float crouchHeight = 0.25f;
    private float originalHeight;
    private float targetScale; 



    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        originalHeight = playerCamera.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        #region Handles Head Bob
        {
            // (later added) Head Bob
            if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
            {
                // Calculate head bob
                bobTimer += Time.deltaTime * bobFrequency;
                float horizontalBob = Mathf.Sin(bobTimer) * bobHorizontalAmplitude;
                float verticalBob = Mathf.Cos(bobTimer * 2) * bobVerticalAmplitude;
                // Apply head bob to camera position
                Vector3 targetPosition = originalCameraPosition + new Vector3(horizontalBob, verticalBob, 0);
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPosition, Time.deltaTime * bobSmoothing);
            }
            else
            {
                // Reset head bob
                bobTimer = 0;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCameraPosition, Time.deltaTime * bobSmoothing);
            }
        }
        #endregion

        /* #region Handles Landing Bob LATER WORK 
         {
             bool isGrounded = characterController.isGrounded;
             bool wasGrounded = false;

             if (isGrounded && !wasGrounded)
             {
                Debug.Log("Landed"); 
             }
         }

         #endregion
        */
        #region Handles Crouching
        
        /*{
           if (Input.GetKey(KeyCode.LeftControl))
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(originalHeight.x, crouchHeight, originalHeight.z), Time.deltaTime * bobSmoothing);
            }

           else
            {
               targetScale = originalHeight;

            }
            transform.localScale = Vector3.LerpUnclamped(transform.localScale, targetScale, Time.deltaTime * bobSmoothing);


        }*/
        float targetHeight;
        float currentHeight = playerCamera.transform.localPosition.y;
        //playerCamera.transform.localPosition;
        if (Input.GetKey(KeyCode.LeftControl))
         {
            targetHeight = crouchHeight;
         }
        else
         {
            targetHeight = originalHeight;
         }
        float crouchMovement = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * bobSmoothing);

       //playerCamera.transform.localPosition.y = crouchMovement; 
        Vector3 currentOffset = playerCamera.transform.localPosition;
        currentOffset.y = crouchMovement;
        playerCamera.transform.localPosition = currentOffset; 

        #endregion
    }
}
