using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData item;                               //이 슬롯에 있는 아이템
    public int amount;                                  //아이템 개수

    [Header("UI Refernce")]
    public Image itemIcon;                              //아이템 아이콘 이미지
    public Text amountText;                             //개수 텍스트
    public GameObject emptySlotImage;                   //빈 슬롯 일때 보옂둘 이미지
    // Start is called before the first frame update
    void Start()
    {
        UpdateSlotUI();
    }

    public void SetItem(ItemData newItem, int newAmount)            //슬롯에 아이템 설정 하는 함수
    {
        item = newItem;
        amount = newAmount;
        UpdateSlotUI();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //UI를 업데이트 하는 함수
    void UpdateSlotUI()
    {
        if (item != null)                          //아이템이 있으면
        {   
            itemIcon.sprite = item.itemIcon;        //아이콘 표시
            itemIcon.enabled = true;

            amountText.text = amount > 1 ? amount.ToString() : "";                  //개수가 1개 보다 많으면 숫자 표시
            if(emptySlotImage != null)
            {
                emptySlotImage.SetActive(false);                                    //빈 슬롯 이미지 숨기기
            }
        }
        else
        {
            itemIcon.enabled = false;                                               //아이콘 숨기기
            amountText.text = "";                                                   //덱스트 비우기

            if (emptySlotImage != null)
            {
                emptySlotImage.SetActive(true);                                     //빈 슬롯 이미지 표시
            }
        }
    }

    public void AddAmount(int value)                        //아이템 개수 추가하는 함수
    {
        amount += value;
        UpdateSlotUI() ;
    }

    public void RemoveAmount(int value)                     //아이템 개수 제거하는 함수
    {
        amount -= value;

        if(amount <= 0)                                     //개수가 0 이하면 슬롯 비우기
        {
            ClearSlot();
        }
        else
        {
            UpdateSlotUI();
        }
    }

    public void ClearSlot()                                 //슬롯을 비우는 함수
    {
        item = null;
        amount = 0;
        UpdateSlotUI();
    }
}
