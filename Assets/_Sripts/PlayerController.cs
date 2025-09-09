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
        // 이동, 점프 등 설정값
        [SerializeField] private float initialPlayerSpeed = 4f;
        [SerializeField] private float maximumPlayerSpeed = 30f;
        [SerializeField] private float playerSpeedIncreaseRate = .1f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float initialGravityValue = -9.81f;

        // 레이어 지정
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask turnLayer;
        [SerializeField] private LayerMask obstacleLayer;

        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip slideAnimationClip;
        [SerializeField] private float scoreMultiplier = 10f;

        // 내부 변수들
        private float playerSpeed;
        private float gravity;
        private Vector3 movementDirection = Vector3.forward;
        private Vector3 playerVelocity;
        private bool sliding = false;
        private float score = 0;

        // 입력 및 컨트롤러 관련
        private PlayerInput playerInput;
        private InputAction turnAction;
        private InputAction jumpAction;
        private InputAction slideAction;
        private CharacterController controller;
        private int slidingAnimationID;

        // 이벤트
        [SerializeField] private UnityEvent<Vector3> turnEvent;
        [SerializeField] private UnityEvent<int> gameOverEvent;
        [SerializeField] private UnityEvent<int> scoreUpdateEvent;

        private void Awake()
        {
            // 컴포넌트 초기화
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            slidingAnimationID = Animator.StringToHash("Sliding");

            turnAction = playerInput.actions["Turn"];
            jumpAction = playerInput.actions["Jump"];
            slideAction = playerInput.actions["Slide"];
        }

        private void OnEnable()
        {
            // 입력 이벤트 등록
            turnAction.performed += PlayerTurn;
            slideAction.performed += PlayerSlide;
            jumpAction.performed += PlayerJump;
        }

        private void OnDisable()
        {
            // 입력 이벤트 제거
            turnAction.performed -= PlayerTurn;
            slideAction.performed -= PlayerSlide;
            jumpAction.performed -= PlayerJump;
        }

        private void Start()
        {
            playerSpeed = initialPlayerSpeed;
            gravity = initialGravityValue;
        }

        // 턴 입력 처리
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

        // 회전 가능 여부 판단
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

        // 실제 회전 수행
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

        // 슬라이드 입력 처리
        private void PlayerSlide(InputAction.CallbackContext context)
        {
            if (!sliding && IsGrounded())
                StartCoroutine(Slide());
        }

        // 슬라이드 코루틴 처리
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

        // 점프 처리
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
            // 낙사 처리
            if (!IsGrounded(20f))
            {
                GameOver();
                return;
            }

            // 점수 계산
            score += scoreMultiplier * Time.deltaTime;
            scoreUpdateEvent.Invoke((int)score);

            // 이동 처리
            controller.Move(transform.forward * playerSpeed * Time.deltaTime);

            if (IsGrounded() && playerVelocity.y < 0)
                playerVelocity.y = 0f;

            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }

        // 지면 감지
        private bool IsGrounded(float length = .2f)
        {
            Vector3 rayOrigin = transform.position - new Vector3(0, controller.height / 2f - 0.1f, 0);
            return Physics.Raycast(rayOrigin - transform.forward * 0.2f, Vector3.down, length, groundLayer) ||
                   Physics.Raycast(rayOrigin + transform.forward * 0.2f, Vector3.down, length, groundLayer);
        }

        // 충돌 시 장애물 확인
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0)
                GameOver();
        }

        // 게임 오버 처리
        private void GameOver()
        {
            Debug.Log("Game Over");
            gameOverEvent.Invoke((int)score);
            gameObject.SetActive(false);
        }

        // 외부 호출용 점프 함수 (아두이노에서 호출)
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
