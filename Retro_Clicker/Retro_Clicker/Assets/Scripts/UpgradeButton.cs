using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Text upgradeDisplayer;

    public string upgradeName;

    [HideInInspector]
    // 업그레이드 1회당 클릭시 골드증가량
    public int goldByUpgrade;
    // 처음 시작할때 처음 구매할때 골드증가량
    public int startGoldByUpgrade = 1;

    [HideInInspector]
    // 업그레이드 1회당 가격
    public int currentCost = 1;
    public int startCurrentCost = 1; // 처음 가격

    [HideInInspector]
    // 업그레이드 단계
    public int level = 1;

    public float upgradePow = 1.07f; // 클릭시 골드 증가폭
    public float costPow = 1.32f; // 업그레이드시 업그레이드 가격 증가폭

    private void Start()
    {
        DataController.Instance.LoadUpgradeButton(this);
        UpdateUI();
    }

    public void PurchaseUpgrade()
    {
        if (DataController.Instance.gold >= currentCost) // 싱글톤 사용, 현재가진 골드가 요구 골드량 보다 많다면
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
        goldByUpgrade = startGoldByUpgrade * (int)Mathf.Pow(upgradePow, level); // Mathf.Pow(플롯f, 플롯p) f의 p승 제곱 리턴 
        currentCost = startCurrentCost * (int)Mathf.Pow(costPow, level);
    }

    public void UpdateUI()
    {
        upgradeDisplayer.text = upgradeName + "\nCost: " + currentCost + "\nLevel : " + level +
            "\nNext New GoldPerClick : " + goldByUpgrade;
    }
}
