using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FPSView : MonoBehaviour
{
    public float sensitivity;
    private Transform orientation;
    private Vector2 turn;

    private float horizontalInput;
    private float verticalInput;

    [Range(0, 50)] public float bobSpeed;
    [Range(0, 500)] public float bobDist;
    private float baseBobSpeed;
    private float baseBobDist;
    private float baseY;

    public Player player;

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        orientation = transform.parent;
        baseY = transform.position.y;
        baseBobSpeed = bobSpeed;
        baseBobDist = bobDist;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        bool moving = horizontalInput != 0 || verticalInput != 0;

        if (player.grounded)
        {
            if (moving) HeadBob();
            else transform.position = new Vector3(transform.position.x,
                                                  baseY,
                                                  transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            bobSpeed *= 1.1f;
            bobDist /= 1.2f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            bobSpeed = baseBobSpeed;
            bobDist = baseBobDist;
        }

        turn.x += Input.GetAxisRaw("Mouse X") * sensitivity;
        turn.y += Input.GetAxisRaw("Mouse Y") * sensitivity;

        turn.y = Mathf.Clamp(turn.y, -90f, 90f);

        transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
        orientation.rotation = Quaternion.Euler(0, turn.x, 0);
    }

    private void HeadBob()
    {
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y - Mathf.Sin(Time.time * bobSpeed)/bobDist,
                                         transform.position.z);
    }
}