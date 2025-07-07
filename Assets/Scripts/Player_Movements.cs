using UnityEngine;

namespace MyPlayer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player_Movements : MonoBehaviour
    {
        Player origin;
        Rigidbody2D rb;

        [SerializeField] float moveSpeed, jumpPower;
        [SerializeField] Transform flipPart;

        [Header("Ground Scanning")]
        [SerializeField] Vector2 groundScanOffset;
        [SerializeField] Vector2 groundScanBoxSize;
        [SerializeField] LayerMask groundScanMask;
        bool isGrounded => Physics2D.OverlapBox((Vector2)transform.position + groundScanOffset, groundScanBoxSize, 0.0f, groundScanMask) != null;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)transform.position + groundScanOffset, groundScanBoxSize);
        }
        private void Awake()
        {
            origin = GetComponent<Player>();
            rb = GetComponent<Rigidbody2D>();
        }
        private void Update()
        {
            UpdateMovements();
            UpdateRotation();
        }
        bool doubleJumped = false;
        void UpdateMovements()
        {
            rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * moveSpeed;
            if (isGrounded)
            {
                doubleJumped = false;
                if(Input.GetKeyDown(KeyCode.Space)) rb.linearVelocityY = jumpPower;
            }
            else
            {
                if(!doubleJumped && Input.GetKeyDown(KeyCode.Space))
                {
                    rb.linearVelocityY = jumpPower;
                    doubleJumped = true;
                }
            }
        }
        void UpdateRotation()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            flipPart.localScale = new Vector3(
                mousePos.x > transform.position.x ? 1.0f : -1.0f, 1.0f, 1.0f);
        }
    }
}
