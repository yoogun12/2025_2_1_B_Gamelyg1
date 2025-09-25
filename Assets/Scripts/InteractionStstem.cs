using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractionStstem : MonoBehaviour
{
    [Header("상호 작용 설정")]
    public float interactionRange = 2.0f;                           //상호 작용 범위
    public LayerMask interactionLayerMask = 1;                      //상호 작용할 레이어
    public KeyCode interactionKey = KeyCode.E;                      //상호 작용 키 (E키)

    [Header("UI 설정")]
    public Text interactionText;                                    //상호 작용 UI 텍스트
    public GameObject interactioneUI;                               //상호 작용 UI 텍스트 객체

    private Transform playerTransform;
    private InteractableObject currentInteractiable;                //감지된 오브젝트를 담는 클래스

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        HideInteractionUI();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteractables();
        HandleInteractionInput();
    }

    void HandleInteractionInput()                                                           //인터랙션 입력 함수
    {
        if(currentInteractiable != null && Input.GetKeyDown(interactionKey))                //인터랙션 키 값을 눌렀을 때
        {
            currentInteractiable.Interact();                                                //행동을 한다.
        }
    }

    void ShowInteractionUI(string text)                                                     //인터랙션 UI 창을 연다
    {
        if(interactioneUI != null)
        {
            interactioneUI.SetActive(true);
        }

        if(interactionText != null)
        {
            interactionText.text = text;
        }
    }

    void HideInteractionUI()                                                                //인터랙션 UI 창을 닫는다.
    {
        if(interactioneUI != null)
        {
            interactioneUI.SetActive(false);
        }
    }

    void CheckForInteractables()
    {
        Vector3 checkPosition = playerTransform.position + playerTransform.forward * (interactionRange * 0.5f);

        Collider[] hitColliders = Physics.OverlapSphere(checkPosition, interactionRange, interactionLayerMask);     //구체와 충돌한 모든 콜라이더 배열

        InteractableObject closestInteractable = null;                  //가장 가까운 물체 선언
        float closestDistance = float.MaxValue;                         //거리 설정

        
        foreach (Collider collider in hitColliders)                                         //가장 가까운 물체 판별
        {
            InteractableObject interactable = collider.GetComponent<InteractableObject>();
            if(interactable != null)
            {
                float distance = Vector3.Distance(playerTransform.position, collider.transform.position);

                //플레이어가 바라보는 방향에 있는지 확인 (각도 체크)
                Vector3 directionToObject = (collider.transform.position - playerTransform.position).normalized;
                float angle = Vector3.Angle(playerTransform.forward, directionToObject);

                if (angle < 90f && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        //상호 작용 오브젝트 변경 체크
        if(closestInteractable != currentInteractiable)
        {
            if(currentInteractiable != null)
            {
                currentInteractiable.OnPlayerExit();                            //이전 오브젝트에서 나감
            }

            currentInteractiable = closestInteractable;


            if(currentInteractiable != null)
            {
                currentInteractiable.OnPlayerEnter();                           //새 오브젝트 선택
                ShowInteractionUI(currentInteractiable.GetInteractionText());
            }
            else
            {
                HideInteractionUI();
            }
                
        }
    }



}
