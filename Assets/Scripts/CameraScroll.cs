using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScroll : MonoBehaviour
{
    private Rigidbody2D rb;
    public PlayerControls controls;
    private InputAction move;
    private InputAction mouseScroll;
    private InputAction mouseClick;
    private InputAction mouseMove;
    private Camera camera;

    float moveSpeed = 10f;
    Vector2 moveDir;
    Vector2 mouseDir;
    Vector2 scrollDir;

    bool isClicked = false;

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();
        mouseScroll = controls.UI.ScrollWheel;
        mouseScroll.Enable();

        mouseClick = controls.UI.Click;
        mouseMove = controls.UI.MouseMove;

        mouseClick.Enable();
        mouseMove.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        GetMoveDir();
        GetScrollWheel();
        GetMouseClick();
        GetMouseMove();
    }

    private void FixedUpdate()
    {
        if (!isClicked)
            rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        else
            rb.velocity = new Vector2(-mouseDir.x * moveSpeed, -mouseDir.y * moveSpeed);
    }

    private void GetMoveDir()
    {
        moveDir = move.ReadValue<Vector2>();
    }

    private void GetScrollWheel()
    {
        scrollDir = mouseScroll.ReadValue<Vector2>();
        camera.orthographicSize -= scrollDir.y * 0.001f;
    }

    private void GetMouseClick()
    {
        isClicked = mouseClick.ReadValue<float>() == 0 ? false : true;
    }

    private void GetMouseMove()
    {
        mouseDir = mouseMove.ReadValue<Vector2>();
    }
}
