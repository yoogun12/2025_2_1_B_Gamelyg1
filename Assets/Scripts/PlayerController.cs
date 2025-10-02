using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("점프 설정")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;                                  //중력 속도 추가
    public float landingDuration = 3.0f;                            //착지 후 착지 모션 지속 시간 ( 해당 지속 시간 이후에 캐릭터가 움직일 수 있게)

    [Header("공격 설정")]
    public float attackDuration = 0.8f;                             //공격 지속 시간
    public bool canMoveWhileAttacking = false;                      //공격중 이동 가능 여부 판단 bool

    [Header("커포넌트")]
    public Animator animator;                                       //컴포넌트 하위에 animator 가 존재하기 때문에

    private CharacterController controller;
    private Camera playerCamera;

    //현재 상태 값들
    private float currentSpeed;
    private bool isAttacking = false;
    private bool isLanding = false;                                 //착지 중인지 확인
    private float landingTimer;                                     //착지 타이머

    private Vector3 velocity;
    private bool isGrounded;                                        //땅에 있는지 판별
    private bool wasGrounded;                                       //직전 프레임에 땅에 있었는지 판단
    private float attackTimer; 
    
    private bool isUIMode = false;                                  //UI 모드 설정

    // Start is called before the first frame update
    void Start()
    { controller = GetComponent <CharacterController>();
        playerCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursorlock();
        }

        if (!isUIMode)
        {
            CheckGrounded();
            HandleLanding();
            HandleMovement();
            UpdateAnimator();
            HandleAttack();
            HandleJump();
        }

    }

    void HandleMovement()                       //이동 함수 제작
    {

        //공격 중이거나 착지 중일 때 움직임 제한
        if((isAttacking && !canMoveWhileAttacking) || isLanding)
        {
            currentSpeed = 0f;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float verical = Input.GetAxis("Vertical");

        if(horizontal !=0 || verical !=0)                       //둘중에 하나라도 입력이 있을 때
        {
            //카메라가 보는 방향의 앞쪽으로 설정
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * verical + cameraRight * horizontal;             //이동 방향 설정

            if(Input.GetKey(KeyCode.LeftShift))                     //왼쪽 Shift 를 눌러서 Run 모드로 변경
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);             //캐릭터 컨트롤러의 이동 입력

            //이동 진행 방향을 바라보면서 이동
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,rotationSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0;
        }
    }

    void UpdateAnimator()
    {
        //전체 최대속도 (runSpeed) 기준으로 0 ~ 1 계산
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("speed",animatorSpeed);
        animator.SetBool("isGrounded",isGrounded);

        bool isFalling = !isGrounded && velocity.y < -0.1f;                 //캐릭터가 Y축 속도가 음수로 넘어가면 떨어지고 있다고 판단
        animator.SetBool("isFalling",isFalling);
        animator.SetBool("isLanding",isLanding);
    }

    void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;                     //캐릭터 컨트롤러에서 상태 값을 받아온다.

        if (!isGrounded && wasGrounded)                         //지금 프레임은 땅이 아니고, 이전 프레임은 땅
        {
            Debug.Log("떨어지기 시작");
        }
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;

            if(!wasGrounded && animator != null)                //착지를 진행
            {
                isLanding = true;
                landingTimer = landingDuration;
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if(animator != null)
            {
                animator.SetTrigger("jumpTrigger");
            }
        }

        if (!isGrounded)                                             //땅위에 있지 않을경우 중력 적용
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer -= Time.deltaTime;                           //랜딩 타이머 시간 만큼 못 움직이게

            if(landingTimer <=0)
            {
                isLanding = false;                                    //착지 완료 처리
            }
        }
    }

    void HandleAttack()
    {
        if(isAttacking)                                               //공격 중일때
        {
            attackTimer -= Time.deltaTime;                            //공격 타이머를 감소

            if(attackTimer <=0)
            {
                isAttacking = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)            //공격 중이 아닐때 키를 누르면 공격
        {
            isAttacking = true;                                         //공격중  표시
            attackTimer = attackDuration;                               //타이머 리필

            if(animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    public void SetCursorLock(bool lockCursor)                          //마우스 락 설정 함수
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isUIMode = false;
        }
        else
        {
            Cursor.lockState= CursorLockMode.None;
            Cursor.visible = true;
            isUIMode = true;
        }
    }


    public void ToggleCursorlock()
    {
        bool ShouldLock = Cursor.lockState != CursorLockMode.Locked;
        SetCursorLock(ShouldLock);
    }

    public void SetUIMode(bool uiMode)
    {
        SetCursorLock(!uiMode);
    }

}
