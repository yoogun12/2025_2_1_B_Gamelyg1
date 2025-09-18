using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("���� ����")]
    public int playerScore = 0;
    public int itemsColledted = 0;

    [Header("UI ����")]
    public Text ScoreText;
    public Text itemCountText;
    public Text gameStatusText;

    public static GameManager Instance;                 //�̱��� ����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem()
    {
        itemsColledted++;
        Debug.Log($"������ ����! (��: {itemsColledted} ��");
    }

    public void UpdateUI()
    {
        if(ScoreText != null)
        {
            ScoreText.text = "���� : " + playerScore;
        }

        if(itemCountText != null)
        {
            itemCountText.text = "������ : " + itemsColledted;
        }
    }
}
