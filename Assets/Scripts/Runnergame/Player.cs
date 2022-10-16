using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public PlayerControls controls;
    private InputAction move;
    private InputAction fire;

    private float minX, maxX, minY, maxY;
    public float topBound, btmBound, leftBound, rightBound;

    float moveSpeed = 10f;

    Vector2 moveDir;

    LevelManager lm;

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();

        fire = controls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
    }

    private void Awake()
    {
        controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        SetBounds();
        lm = LevelManager.Instance;
        SetCharacter();
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveDir();

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    private void SetCharacter()
    {
        GetComponent<Animator>().runtimeAnimatorController = lm.charAnimList[lm.charSelected];
    }

    private void GetMoveDir()
    {
        Vector2 pos = transform.position;
        moveDir = move.ReadValue<Vector2>();
        if (pos.y <= (minY + btmBound +sr.bounds.size.y/2) && moveDir.y < 0 || pos.y >= (maxY - topBound - sr.bounds.size.y / 2) && moveDir.y > 0)
        {
            moveDir = new Vector2(moveDir.x, 0);
        }
        if (pos.x <= (minX + sr.bounds.size.x / 2) && moveDir.x < 0 || pos.x >= (maxX - rightBound - sr.bounds.size.x / 2) && moveDir.x > 0)
        {
            moveDir = new Vector2(0, moveDir.y);
        }
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        
    }

    private void SetBounds()
    {
        float camDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        minX = bottomCorner.x;
        maxX = topCorner.x;
        minY = bottomCorner.y;
        maxY = topCorner.y;
    }
}
