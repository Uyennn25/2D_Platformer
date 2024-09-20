using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   private float movementInputDirection;

   private bool isFacingRight = true; //hướng mặc định của player đang hướng về bên phải.
   private Rigidbody2D rb;

   public float movementSpeed = 10.0f;

   public float jumpForce = 16.0f;
   private void Start()
   {
      rb = GetComponent<Rigidbody2D>();
   }

   private void Update()
   {
      CheckInput();
      CheckMomentDirection();
   }

   private void FixedUpdate()
   {
      ApplyMovement();
    
   }

   private void CheckMomentDirection()// kiểm tra và chuyển hướng cho player cùng hướng với hướng chuyển động đầu vào.
   {
      if (isFacingRight && movementInputDirection < 0 )
      {
         Flip();
      }

      if (!isFacingRight && movementInputDirection > 0)
      {
         Flip();
      }
   }
   private void CheckInput()
   {
      movementInputDirection = Input.GetAxisRaw("Horizontal");

      if (Input.GetButtonDown("Jump"))
      {
         Jump();
      }
   }

   private void ApplyMovement()
   {
      rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
   }

   private void Flip() // hàm lật player.
   {
      isFacingRight = !isFacingRight;
      transform.Rotate(0.0f, 180.0f,0.0f);
      
   }

   private void Jump()
   {
      rb.velocity = new Vector2(rb.velocity.x, jumpForce);
   }
}
