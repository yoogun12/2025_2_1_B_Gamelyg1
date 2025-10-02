using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("���� ����")]
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;                                  //�߷� �ӵ� �߰�
    public float landingDuration = 3.0f;                            //���� �� ���� ��� ���� �ð� ( �ش� ���� �ð� ���Ŀ� ĳ���Ͱ� ������ �� �ְ�)

    [Header("���� ����")]
    public float attackDuration = 0.8f;                             //���� ���� �ð�
    public bool canMoveWhileAttacking = false;                      //������ �̵� ���� ���� �Ǵ� bool

    [Header("Ŀ����Ʈ")]
    public Animator animator;                                       //������Ʈ ������ animator �� �����ϱ� ������

    private CharacterController controller;
    private Camera playerCamera;

    //���� ���� ����
    private float currentSpeed;
    private bool isAttacking = false;
    private bool isLanding = false;                                 //���� ������ Ȯ��
    private float landingTimer;                                     //���� Ÿ�̸�

    private Vector3 velocity;
    private bool isGrounded;                                        //���� �ִ��� �Ǻ�
    private bool wasGrounded;                                       //���� �����ӿ� ���� �־����� �Ǵ�
    private float attackTimer; 
    
    private bool isUIMode = false;                                  //UI ��� ����

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

    void HandleMovement()                       //�̵� �Լ� ����
    {

        //���� ���̰ų� ���� ���� �� ������ ����
        if((isAttacking && !canMoveWhileAttacking) || isLanding)
        {
            currentSpeed = 0f;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float verical = Input.GetAxis("Vertical");

        if(horizontal !=0 || verical !=0)                       //���߿� �ϳ��� �Է��� ���� ��
        {
            //ī�޶� ���� ������ �������� ����
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * verical + cameraRight * horizontal;             //�̵� ���� ����

            if(Input.GetKey(KeyCode.LeftShift))                     //���� Shift �� ������ Run ���� ����
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }

            controller.Move(moveDirection * currentSpeed * Time.deltaTime);             //ĳ���� ��Ʈ�ѷ��� �̵� �Է�

            //�̵� ���� ������ �ٶ󺸸鼭 �̵�
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
        //��ü �ִ�ӵ� (runSpeed) �������� 0 ~ 1 ���
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("speed",animatorSpeed);
        animator.SetBool("isGrounded",isGrounded);

        bool isFalling = !isGrounded && velocity.y < -0.1f;                 //ĳ���Ͱ� Y�� �ӵ��� ������ �Ѿ�� �������� �ִٰ� �Ǵ�
        animator.SetBool("isFalling",isFalling);
        animator.SetBool("isLanding",isLanding);
    }

    void CheckGrounded()
    {
        wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;                     //ĳ���� ��Ʈ�ѷ����� ���� ���� �޾ƿ´�.

        if (!isGrounded && wasGrounded)                         //���� �������� ���� �ƴϰ�, ���� �������� ��
        {
            Debug.Log("�������� ����");
        }
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;

            if(!wasGrounded && animator != null)                //������ ����
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

        if (!isGrounded)                                             //������ ���� ������� �߷� ����
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLanding()
    {
        if (isLanding)
        {
            landingTimer -= Time.deltaTime;                           //���� Ÿ�̸� �ð� ��ŭ �� �����̰�

            if(landingTimer <=0)
            {
                isLanding = false;                                    //���� �Ϸ� ó��
            }
        }
    }

    void HandleAttack()
    {
        if(isAttacking)                                               //���� ���϶�
        {
            attackTimer -= Time.deltaTime;                            //���� Ÿ�̸Ӹ� ����

            if(attackTimer <=0)
            {
                isAttacking = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking)            //���� ���� �ƴҶ� Ű�� ������ ����
        {
            isAttacking = true;                                         //������  ǥ��
            attackTimer = attackDuration;                               //Ÿ�̸� ����

            if(animator != null)
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    public void SetCursorLock(bool lockCursor)                          //���콺 �� ���� �Լ�
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
