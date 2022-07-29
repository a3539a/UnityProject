using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Text upgradeDisplayer;

    public string upgradeName;

    [HideInInspector]
    // ���׷��̵� 1ȸ�� Ŭ���� ���������
    public int goldByUpgrade;
    // ó�� �����Ҷ� ó�� �����Ҷ� ���������
    public int startGoldByUpgrade = 1;

    [HideInInspector]
    // ���׷��̵� 1ȸ�� ����
    public int currentCost = 1;
    public int startCurrentCost = 1; // ó�� ����

    [HideInInspector]
    // ���׷��̵� �ܰ�
    public int level = 1;

    public float upgradePow = 1.07f; // Ŭ���� ��� ������
    public float costPow = 1.32f; // ���׷��̵�� ���׷��̵� ���� ������

    private void Start()
    {
        DataController.Instance.LoadUpgradeButton(this);
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if (DataController.Instance.gold >= currentCost) // �̱��� ���, ���簡�� ��尡 �䱸 ��差 ���� ���ٸ�
        {
            DataController.Instance.gold -= currentCost;
            level++;
            DataController.Instance.goldPerClick += goldByUpgrade;

            UpdateUpgrade();
            UpdateUI();
            DataController.Instance.SaveUpgradeButton(this);
        }
    }

    public void UpdateUpgrade()
    {
        goldByUpgrade = startGoldByUpgrade * (int)Mathf.Pow(upgradePow, level); // Mathf.Pow(�÷�f, �÷�p) f�� p�� ���� ���� 
        currentCost = startCurrentCost * (int)Mathf.Pow(costPow, level);
    }

    public void UpdateUI()
    {
        upgradeDisplayer.text = upgradeName + "\nCost: " + currentCost + "\nLevel : " + level +
            "\nNext New GoldPerClick : " + goldByUpgrade;
    }
}
