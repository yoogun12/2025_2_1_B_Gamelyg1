using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : InteractableObject
{
    [Header("동전 설정")]
    public int coinValue = 10;
    public string questTag = "Coin";                                //퀘스트에서 사용할 태그


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        objectName = "동전";
        interactionText = "[E] 동전 획득";
        interactionType = InteractionType.item;
    }

   
    protected override void CollectItem()
    {
        if(QuestManager.instance != null)
        {
            QuestManager.instance.AddcollectProgrees(questTag);
        }
        Destroy(gameObject);
    }
  
}
