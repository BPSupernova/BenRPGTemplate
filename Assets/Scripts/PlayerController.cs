using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float moveSpeed = 2f;

    public static PlayerController instance;
    public string areaTransitionName;

    private Vector3 bottomLeftLimit;
    private Vector3 upperRightLimit;

    public bool canMove = true;

    void Awake()
    {
        DestroyPossibleDuplicatePlayer();
    }

    void DestroyPossibleDuplicatePlayer()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this) {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (canMove) {
            playerRigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed); // Moves the player
        } else {
            playerRigidbody.velocity = Vector2.zero;
        }

        SetPlayersWalkingDirection();
        SetPlayersFacingDirection();

        // Keeping the player in the bounds of the game map
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, upperRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, upperRightLimit.y), transform.position.z);
    }

    public void SetBounds(Vector3 bottomLeft, Vector3 topRight) {
        bottomLeftLimit = bottomLeft + new Vector3(0.5f,1f,0);
        upperRightLimit = topRight + new Vector3(-0.5f,-1f,0);
    }

    void SetPlayersWalkingDirection()
    {
        playerAnimator.SetFloat("MoveX", playerRigidbody.velocity.x);
        playerAnimator.SetFloat("MoveY", playerRigidbody.velocity.y);
    }

    void SetPlayersFacingDirection()
    {
        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            if (canMove) {
                playerAnimator.SetFloat("LastXMove", Input.GetAxisRaw("Horizontal"));
                playerAnimator.SetFloat("LastYMove", Input.GetAxisRaw("Vertical"));
            }
        }
    }
}
