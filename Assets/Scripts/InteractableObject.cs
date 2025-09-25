using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("��ȣ �ۿ� ����")]
    public string objectName = "������ ";
    public string interactionText = "[E] ��ȣ �ۿ�";
    public InteractionType interactionType = InteractionType.item;

    [Header("���̶���Ʈ ����")]
    public Color highlightClor = Color.yellow;
    public float highlightIntensity = 1.5f;

    public Renderer objectRenderer;
    private Color originalColor;
    private bool isHighlighted = false;


    public enum InteractionType
    {
        item,                           //��Ƽ�� (����, ���� ��)
        Machine,                        //��� (����, ��ư ��)
        Building,                       //�ǹ� (��, ���� ��)
        NPC,                            //NPC
        Cellectible                     //����ǰ
    }

    protected virtual void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if(objectRenderer != null )
        {
            originalColor = objectRenderer.material.color;
        }
        gameObject.layer = 8;                             
    }

    public virtual void OnPlayerEnter()
    {
        Debug.Log($"[{objectName}]) ������");
        HighlightObject();
    }

    public virtual void OnPlayerExit()
    {
        Debug.Log($"[{objectName}]) �������� ���");
        RemoveHighlight();
    }

    protected virtual void HighlightObject()                                        //���� �Լ��� ���̶���Ʈ ����
    {
        if (objectRenderer != null && !isHighlighted)
        {
            objectRenderer.material.color = highlightClor;
            objectRenderer.material.SetFloat("_Emission" , highlightIntensity);
            isHighlighted = true;
        }
    }


    protected virtual void RemoveHighlight()                                        //���� �Լ��� ���̶���Ʈ ���� ����
    {
        if (objectRenderer != null && !isHighlighted)
        {
            objectRenderer.material.color = originalColor;
            objectRenderer.material.SetFloat("_Emission", 0f);
            isHighlighted = false;
        }
    }

    
    protected virtual void CollectItem()                                //������ ���� �����Լ�
    {
        Destroy(gameObject);                                            //�������� �ı��Ѵ�.
    }

    protected virtual void OperateMachine()                             //��� �۵� �Լ�
    {
        if(objectRenderer != null)
        {
            objectRenderer.material.color = Color.green;                //�켱 ���� �ʷ����� �����.
        }
    }

    protected virtual void AccessBuilding()                             //���� ����
    {
        transform.Rotate(Vector3.up * 90f);                             //�켱 ȸ�� �Ѵ�,
    }

    protected virtual void TalkToNPC()                                  //NPC�� ��ȭ
    {
        Debug.Log($"{objectName}�� ��ȭ�� �����մϴ�.");                //�켱 ����� �α׸� �Ѵ�.
    }

    public virtual void Interact()
    {
        //��ȣ �ۿ� Ÿ�Կ� ���� �⺻ ����
        switch(interactionType)
        {
            case InteractionType.item:
                CollectItem();
                break;
            case InteractionType.Machine:
                OperateMachine();
                break;
            case InteractionType.Building:
                AccessBuilding();
                break;
            case InteractionType.Cellectible:
                TalkToNPC();
                break;
        }
    }

    public string GetInteractionText()              //UI�� ������ Text �۾� ���� �Լ�
    {
        return interactionText;
    }
}

