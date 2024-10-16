using UnityEngine;

public class PlayerController : MonoBehaviour
{
   private float movementInputDirection;
   private int amountOfJumpLeft;
   private int facingDirection = 1;

   private bool isFacingRight = true; //hướng mặc định của player đang hướng về bên phải.
   private bool isWalking;
   private bool isGround;
   private bool isTouchingWall;
   private bool isWallSliding;
   private bool canJump;
  
   private Rigidbody2D rb;
   private Animator anim;

   public float movementSpeed = 10.0f;
   public float jumpForce = 16.0f;
   public float groundCheckRadius;
   public float wallCheckDistance;
   public float wallSlideSpeed;
   public float movementForceInAir;
   public float airDragMultiplier = 0.95f;
   public float variableJumpHeightMultiplier = 0.5f;
   public float wallHopForce;
   public float wallJumpForce;

   public int amountOfJump = 1;

   public Vector2 wallHopDirection;
   public Vector2 wallJumpDirection;
   

   public Transform groundCheck;
   public Transform wallCheck;

   public LayerMask whatIsGround;
   private void Start()
   {
      rb = GetComponent<Rigidbody2D>();
      anim = GetComponent<Animator>();
      amountOfJumpLeft = amountOfJump;
      wallHopDirection.Normalize();
      wallJumpDirection.Normalize();
   }

   private void Update()
   {
      CheckInput();
      CheckMomentDirection();
      AppdateAnimations();
      CheckIfCanJump();
      CheckIfWallSliding();
   }

   private void FixedUpdate()
   {
      ApplyMovement();
      CheckSurroundings();
    
   }

   private void CheckIfWallSliding()
   {
      if (isTouchingWall && !isGround && rb.velocity.y < 0)
      {
         isWallSliding = true;
      }
      else
      {
         isWallSliding = false;
      }
   }
   private void CheckIfCanJump()
   {
      if ((isGround && rb.velocity.y <= 0) || isWallSliding )
      {
         amountOfJumpLeft = amountOfJump;
      }

      if (amountOfJumpLeft <= 0)
      {
         canJump = false;
      }
      else
      {
         canJump = true;
      }
   }
   private void CheckSurroundings()
   {
      isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
      isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
      /*
       Physics2D.OverlapCircle():
         Đây là một hàm của Unity dùng để kiểm tra xem có bất kỳ collider nào của các đối tượng khác chồng lấn
         với một hình tròn đã cho hay không.
         Các tham số của hàm:
         groundCheck.position: Vị trí tâm của hình tròn kiểm tra.
         groundCheckRadius: Bán kính của hình tròn kiểm tra.
         whatIsGround: Layer mask chỉ định các layer mà hình tròn sẽ kiểm tra sự
       */
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

      if (movementInputDirection != 0)
      {
         isWalking = true;
        
      }
      else
      {
         isWalking = false;
        

      }
      
     
   }

   private void AppdateAnimations()
   {
      
      anim.SetBool("isWalking", isWalking);
      anim.SetBool("isGround", isGround);
      anim.SetFloat("yVelocity", rb.velocity.y);
      anim.SetBool("isWallSliding", isWallSliding);
   }
   private void CheckInput()
   {
      movementInputDirection = Input.GetAxisRaw("Horizontal");
      
      if (Input.GetButtonDown("Jump"))
      {
         Jump();
         
      }

      if (Input.GetButtonUp("Jump"))
      {
         rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
      }
   }

   private void Jump()
   {
      if (canJump && !isWallSliding)
      {
         
             rb.velocity = new Vector2(rb.velocity.x, jumpForce);
             amountOfJumpLeft--;
      }
      else if(isWallSliding && movementInputDirection == 0 && canJump)
      {
         isWallSliding = false;
         amountOfJumpLeft--;
         Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.x);
         rb.AddForce(forceToAdd, ForceMode2D.Impulse);
         
      }
      else if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump)
      {
         isWallSliding = false;
         amountOfJumpLeft--;
         Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.x);
         rb.AddForce(forceToAdd, ForceMode2D.Impulse);
         
      }
   }
   private void ApplyMovement()
   {
      if (isGround)
      {
         rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
      }
      else if (!isGround && !isWallSliding && movementInputDirection != 0)
      {
         Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
         rb.AddForce(forceToAdd);
         
         if(Mathf.Abs(rb.velocity.x) < movementSpeed)
         {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection , rb.velocity.y);
         }
      }
      else if (!isGround && !isWallSliding && movementInputDirection == 0)
      {
         rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
      }
      
      if (isWallSliding)
      {
         if (rb.velocity.y < -wallSlideSpeed)
         {
            rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
         }
      }
      
      
   }

   private void Flip() // hàm lật player.
   {
      if (!isWallSliding)
      {
         facingDirection *= -1;
         isFacingRight = !isFacingRight;
         transform.Rotate(0.0f, 180.0f,0.0f); 
      }
     
      
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
      Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
      /*
        hàm OnDrawGizmos là một hàm có sẵn trong Unity, được cung cấp bởi lớp MonoBehaviour. 
        sử dụng hàm này trong các script của mình để vẽ các gizmos.
        Các phương thức thường dùng của lớp Gizmos
         Gizmos.DrawSphere: Vẽ một hình cầu.
         Gizmos.DrawLine: Vẽ một đường thẳng.
         Gizmos.DrawCube: Vẽ một hình hộp.
         Gizmos.DrawWireSphere: Vẽ một hình cầu chỉ có đường viền.
         Gizmos.DrawWireCube: Vẽ một hình hộp chỉ có đường viền.
         Gizmos.color: Thiết lập màu sắc cho các hình dạng.
         
          Gizmos.DrawLine (..):sẽ vẽ một đường thẳng bắt đầu từ vị trí của đối tượng "wallCheck" 
          và kéo dài theo hướng trục x dương một khoảng cách bằng wallCheckDistance.
       */
   }
}
