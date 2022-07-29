using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public Text itemDisplayer;

    public CanvasGroup canvasGroup;

    public Slider slider;

    public string itemName;

    public int level = 0;

    [HideInInspector]
    public int currentCost;
    public int startCurrentCost = 1;

    [HideInInspector]
    public int goldPerSec = 0;
    public int startGoldPerSec = 1;

    public float costPow = 1.32f;

    public float upgradePow = 3.4f;

    [HideInInspector]
    // ������ ���� ���� üũ
    public bool isPurchased = false;

    private void Start()
    {
        DataController.Instance.LoadItemButton(this);

        StartCoroutine(AddGoldLoop()); //StartCoroutine(�Լ�)
        // IEnumerator�� ����� �Լ��� ���

        UpdateUI();
    }

    public void PurchaseItem()
    {
        if (DataController.Instance.gold >= currentCost)
        {
            isPurchased = true;
            DataController.Instance.gold -= currentCost;
            level++;
        }
        UpdateItem();
        UpdateUI();

        DataController.Instance.SaveItemButton(this);
    }

    IEnumerator AddGoldLoop() // �ݺ������� ���ð� �ɰ� ������ IEnumerator�� ����
    {
        while (true)
        {
            if (isPurchased)
            {
                DataController.Instance.gold += goldPerSec;
            }

            yield return new WaitForSeconds(1.2f); // ���ð� 1.2�� // yield�� ���
        }
    }

    public void UpdateItem()
    {
        goldPerSec = goldPerSec + startGoldPerSec * (int)Mathf.Pow(upgradePow, level);
        currentCost = startCurrentCost * (int)Mathf.Pow(costPow, level);
    }

    public void UpdateUI()
    {
        itemDisplayer.text = itemName + "\nLevel : " + level + "\nCost : " + currentCost +
            "\nGold Per Sec : " + goldPerSec + "\nIsPurchased : " + isPurchased;

        slider.minValue = 0;
        slider.maxValue = currentCost;

        slider.value = DataController.Instance.gold;

        if (isPurchased)
        {
            canvasGroup.alpha = 1.0f;
        }
        else
        {
            canvasGroup.alpha = 0.6f;
        }
    }

    private void Update()
    {
        UpdateUI();
    }
}
