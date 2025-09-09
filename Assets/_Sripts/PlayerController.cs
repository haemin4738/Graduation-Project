using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Diagnostics;
using System.Numerics;
using Unity.VisualScripting;

namespace TempleRun.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        // �̵�, ���� �� ������
        [SerializeField] private float initialPlayerSpeed = 4f;
        [SerializeField] private float maximumPlayerSpeed = 30f;
        [SerializeField] private float playerSpeedIncreaseRate = .1f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float initialGravityValue = -9.81f;

        // ���̾� ����
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask turnLayer;
        [SerializeField] private LayerMask obstacleLayer;

        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip slideAnimationClip;
        [SerializeField] private float scoreMultiplier = 10f;

        // ���� ������
        private float playerSpeed;
        private float gravity;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;
        private bool sliding = false;
        private float score = 0;

        // �Է� �� ��Ʈ�ѷ� ����
        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private CharacterController controller;
        private int slidingAnimationID;

        // �̺�Ʈ
        [SerializeField] private UnityEvent<Vector3> turnEvent;
        [SerializeField] private UnityEvent<int> gameOverEvent;
        [SerializeField] private UnityEvent<int> scoreUpdateEvent;

        private void Awake()
        {
            // ������Ʈ �ʱ�ȭ
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            slidingAnimationID = Animator.StringToHash("Sliding");

            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
        }

        private void OnEnable()
        {
            // �Է� �̺�Ʈ ���
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
        }

        private void OnDisable()
        {
            // �Է� �̺�Ʈ ����
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
        }

        private void Start()
        {
            playerSpeed = initialPlayerSpeed;
            gravity = initialGravityValue;
        }

        // �� �Է� ó��
        private void PlayerTurn(InputAction.CallbackContext context)
        {
            Vector3? turnPosition = CheckTurn(context.ReadValue<float>());
            if (!turnPosition.HasValue)
            {
                GameOver();
                return;
            }

            Vector3 targetDirection = Quaternion.AngleAxis(90 * context.ReadValue<float>(), Vector3.up) * movementDirection;
            turnEvent.Invoke(targetDirection);
            Turn(context.ReadValue<float>(), turnPosition.Value);
        }

        // ȸ�� ���� ���� �Ǵ�
        private Vector3? CheckTurn(float turnValue)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, .1f, turnLayer);
            if (hitColliders.Length > 0)
            {
                Tile tile = hitColliders[0].transform.parent.GetComponent<Tile>();
                TileType type = tile.type;
                if ((type == TileType.LEFT && turnValue == -1) ||
                    (type == TileType.RIGHT && turnValue == 1) ||
                    (type == TileType.SIDEWAYS))
                {
                    return tile.pivot.position;
                }
            }
            return null;
        }

        // ���� ȸ�� ����
        private void Turn(float turnValue, Vector3 turnPosition)
        {
            Vector3 newPosition = new Vector3(turnPosition.x, transform.position.y, turnPosition.z);
            controller.enabled = false;
            transform.position = newPosition;
            controller.enabled = true;

            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 90 * turnValue, 0);
            transform.rotation = rotation;
            movementDirection = transform.forward.normalized;
        }

        // �����̵� �Է� ó��
        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!sliding && IsGrounded())
                StartCoroutine(Slide());
        }

        // �����̵� �ڷ�ƾ ó��
        private IEnumerator Slide()
        {
            sliding = true;
            Vector3 originalCenter = controller.center;
            controller.height /= 2;
            controller.center = new Vector3(originalCenter.x, originalCenter.y - controller.height / 2, originalCenter.z);
            animator.Play(slidingAnimationID);
            yield return new WaitForSeconds(slideAnimationClip.length);
            controller.height *= 2;
            controller.center = originalCenter;
            sliding = false;
        }

        // ���� ó��
        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
                Debug.Log("Jump");
            }
        }

        private void Update()
        {
            // ���� ó��
            if (!IsGrounded(20f))
            {
                GameOver();
                return;
            }

            // ���� ���
            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            // �̵� ó��
            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
                playerVelocity.y = 0f;

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        // ���� ����
        private bool IsGrounded(float length = .2f)
        {
            Vector3 rayOrigin = transform.position - new Vector3(0, controller.height / 2f - 0.1f, 0);
            return Physics.Raycast(rayOrigin - transform.forward * 0.2f, Vector3.down, length, groundLayer) ||
                   Physics.Raycast(rayOrigin + transform.forward * 0.2f, Vector3.down, length, groundLayer);
        }

        // �浹 �� ��ֹ� Ȯ��
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
                GameOver();
        }

        // ���� ���� ó��
        private void GameOver()
        {
            Debug.Log("Game Over");
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
        }

        // �ܺ� ȣ��� ���� �Լ� (�Ƶ��̳뿡�� ȣ��)
        public void Jump()
        {
            if (IsGrounded())
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * gravity * -3f);
                controller.Move(playerVelocity * Time.deltaTime);
                Debug.Log("Jump from Arduino");
            }
        }
    }
}
