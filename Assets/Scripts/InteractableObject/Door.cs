using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    [Header("�� ����")]
    public bool isOpen = false;
    public Vector3 openPosition;
    public float openSpeed = 2f;

    private Vector3 closedPosition;

    protected override void Start()
    {
        base.Start();                                       //���� ��� ���� ��ŸƮ �Լ��� �ѹ� ���� ��Ų��.
        objectName = "��";
        interactionText = "[E] �� ����";
        interactionType = InteractionType.Building;

        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.right * 3f;
    }

    protected override void AccessBuilding()
    {
        isOpen = !isOpen;
        if( isOpen )
        {
            interactionText = "[E] �� �ݱ�";
            StartCoroutine(MoveDoor(closedPosition));
        }
        else
        {
            interactionText = "[E] �� ����";
            StartCoroutine(MoveDoor(openPosition));
        }
    }

    IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

    }
}
