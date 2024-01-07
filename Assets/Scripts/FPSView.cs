using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSView : MonoBehaviour
{
    public float sensitivity;
    private Transform orientation;
    private Vector2 turn;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        orientation = transform.parent;
    }

    private void Update()
    {
        turn.x += Input.GetAxisRaw("Mouse X") * sensitivity;
        turn.y += Input.GetAxisRaw("Mouse Y") * sensitivity;

        turn.y = Mathf.Clamp(turn.y, -90f, 90f);

        transform.rotation = Quaternion.Euler(-turn.y, turn.x, 0);
        orientation.rotation = Quaternion.Euler(0, turn.x, 0);
    }
}