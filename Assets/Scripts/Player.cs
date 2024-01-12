using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public AudioClip[] footstepClip;
    public AudioClip[] jumpClip;
    public AudioClip[] moanClip;
    public AudioSource footstepsSource;
    public AudioSource jumpSource;
    public AudioSource moanSource;
    private bool inAir = false;

    public float goon = 0;
    private bool inGoo = false;
    public Material goonScreenColor;

    private float fov = 0f;
    private bool isSprinting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        ChangeMaterialColor(0);

        fov = Camera.main.fieldOfView;

        readyToJump = true;
    }

    private void Update()
    {
        int maxMoveSpeed = isSprinting ? 10 : 5;
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            if (inAir) PlayJumpSound(1);
            inAir = false;
            rb.drag = groundDrag;
        }
        else
        {
            inAir = true;
            rb.drag = 0;
        }

        // audio footstep
        if ((horizontalInput != 0 || verticalInput != 0) && grounded)
        {
            if (!footstepsSource.isPlaying) footstepsSource.Play();
        }
        else footstepsSource.Stop();

        // goo consequences
        if (inGoo) goon += Time.deltaTime;
        else goon -= Time.deltaTime;
        moveSpeed = Mathf.Clamp(maxMoveSpeed - (goon / (isSprinting ? 1:2)), 0, maxMoveSpeed);
        goon = Mathf.Clamp(goon, 0, 10);
        ChangeMaterialColor(goon / 10);
        if (goon>=10) Death();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded && !inGoo)
        {
            readyToJump = false;

            Jump();
            PlayJumpSound(0);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if (Input.GetKeyDown(jumpKey) && inGoo)
        {
            goon++;
            PlayJumpSound(2);
            PlayMoanSound(2);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // while sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            footstepsSource.clip = footstepClip[1];
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + Time.deltaTime * 100, fov, fov+7);
        }
        else
        {
            isSprinting = false;
            footstepsSource.clip = footstepClip[0];
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - Time.deltaTime * 100, fov, fov+7);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void PlayJumpSound(int i)
    {
        jumpSource.PlayOneShot(jumpClip[i]);
    }
    
    private void PlayMoanSound(int i)
    {
        moanSource.PlayOneShot(moanClip[i]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goo")
        {
            inGoo = true;
            PlayMoanSound(0);
            goon += 5;
        }
        else if (other.tag == "Cleanse")
        {
            goon = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Goo")
        {
            inGoo = false;
        }
    }

    private void ChangeMaterialColor(float a)
    {
        Color color = goonScreenColor.color;
        color.a = a;
        goonScreenColor.color = color;
    }

    private void Death()
    {
        footstepsSource.Stop();
        PlayJumpSound(3);
        PlayMoanSound(1);
        enabled = false;
    }
}