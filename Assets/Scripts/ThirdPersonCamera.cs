using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Ÿ�� ����")]
    public Transform target;

    [Header("ī�޶� �Ÿ� ����")]
    public float distance = 8.0f;
    public float height = 5.0f;

    [Header("���콺 ����")]
    public float mouseSensivitiy = 2.0f;
    public float minVecticalAngle = -30.0f;
    public float maxvecticalAngle = 60.0f;

    [Header("�ε巯�� ����")]
    public float positionSmoothTime = 0.2f;
    public float rotationSmoothTime = 0.1f;

    //ȸ�� ����
    private float horizontalAngle = 0.0f;
    private float verticalAngle = 0.0f;

    //�����ӿ� ����
    private Vector3 currentVelocity;
    private Vector3 currentPosition;
    private Quaternion currentRotation;


    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        //�ʱ� ��ġ ȸ�� ����
        currentPosition = transform.position;
        currentRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) { ToggleCursor(); }                //ESC �� Ŀ�� ���
    }

    void LateUpdate()
    {
        if (target == null) return;
        HandleMouseInput();
        UpdateCameraSmooth();
    }

    void HandleMouseInput()                     //���콺 �Է����� ȭ�� ó�� �Լ�
    {
        //Ŀ���� ��� �������� ���콺 �Է� ó��
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensivitiy;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivitiy;

        horizontalAngle += mouseX;
        verticalAngle -= mouseY;

        verticalAngle = Mathf.Clamp(verticalAngle,minVecticalAngle,maxvecticalAngle);
    }

    void UpdateCameraSmooth()               //ī�޶� ��ǥ ��ġ ��� �ϴ� �Լ�
    {
        //��ǥ ��ġ ���
        Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
        Vector3 rotateOffset = rotation * new Vector3(0, height , -distance);
        Vector3 targetPosition = target.position + rotateOffset;

        //��ǥ ȸ�� ���
        Vector3 looktarget = target.position + Vector3.up * height;
        Quaternion targetRotation = Quaternion.LookRotation(looktarget - targetPosition);

        //�ε巴�� �̵�
        currentPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, positionSmoothTime);

        //�ε巴�� ȸ��
        currentRotation = Quaternion.Slerp(currentRotation,targetRotation,Time.deltaTime / rotationSmoothTime);

        //�� ����
        transform.position = currentPosition;
        transform.rotation = currentRotation;   
    }

    void ToggleCursor()
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;


        }
        else
        {
            Cursor.lockState= CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
