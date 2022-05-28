using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerController PlayerController;
    public bool isOnGround;
    public bool isJumping;

    private void Awake()
    {
        PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground")){
            //isOnGround = true;
            //isJumping = false;
            PlayerController.isOnGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            //isOnGround = true;
            //isJumping = true;
            PlayerController.isOnGround = true;
            PlayerController.LandOnGround();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            //isOnGround = false;
            //isJumping = true;
            PlayerController.isOnGround = false;
        }
    }
}
