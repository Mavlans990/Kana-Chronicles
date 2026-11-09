using UnityEngine;

namespace Platformer.Mechanics
{
    public class PlayerControllerMenu : KinematicObject
    {
        public float maxSpeed = 3f;
        public float jumpTakeOffSpeed = 7f;

        public bool moveRight = true;

        [Header("Auto Jump Detect")]
        public LayerMask enemyLayer;
        public float detectDistance = 1.5f;
        public Vector2 rayOffset = new Vector2(0.3f, 0f);

        Vector2 move;
        SpriteRenderer spriteRenderer;
        Animator animator;

        bool jumpRequested;

        protected void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            // Jalan otomatis
            move.x = moveRight ? 1f : -1f;

            // Cek musuh di depan, lalu loncat otomatis
            if (IsGrounded && IsEnemyInFront())
            {
                jumpRequested = true;
            }

            // Agar gravity dan collision dari KinematicObject tetap berjalan
            base.Update();
        }

        bool IsEnemyInFront()
        {
            float direction = moveRight ? 1f : -1f;

            Vector2 origin = (Vector2)transform.position + new Vector2(rayOffset.x * direction, rayOffset.y);
            Vector2 rayDirection = Vector2.right * direction;

            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, detectDistance, enemyLayer);

            Debug.DrawRay(origin, rayDirection * detectDistance, Color.red);

            return hit.collider != null;
        }

        protected override void ComputeVelocity()
        {
            // Loncat otomatis
            if (jumpRequested && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed;
                jumpRequested = false;
            }

            // Arah hadap karakter
            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            // Animasi
            if (animator != null)
            {
                animator.SetBool("grounded", IsGrounded);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            }

            // Gerak otomatis
            targetVelocity = move * maxSpeed;
        }
    }
}