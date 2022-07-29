using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    // 싱글톤 객체 생성
    // 매번 데이터컨트롤러 객체 생성해서 스크립트 넣지 않아도
    // 전역에서 사용가능
    static DataController instance; // 전역으로 데이터컨트롤러 생성

    // instance 객체로 DataController 찾아서 넣기
    public static DataController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataController>();

                if (instance == null) // 없으면 만들기
                {
                    GameObject container = new GameObject("DataController");

                    instance = container.AddComponent<DataController>();
                }
            }
            return instance;
        }
    }

    ItemButton[] itemButtons;

    // 프로퍼티로 PlayerPrefs 에 자료를 바로 저장
    public long gold
    {
        get 
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }

            string tmpGold = PlayerPrefs.GetString("Gold"); // 저장된 값이 없으면 null 반환
            return long.Parse(tmpGold);
        }
        set
        {
            PlayerPrefs.SetString("Gold", value.ToString());
        }
    }
    public int goldPerClick
    {
        get 
        {
            return PlayerPrefs.GetInt("GoldPerClick", 1);
        }
        set
        {
            PlayerPrefs.SetInt("GoldPerClick", value);
        }
    }

    // 게임시작시 골드와 클릭시 골드 정보를 불러온다
    private void Awake()
    {
        itemButtons = FindObjectsOfType<ItemButton>();
    }

    // 업그레이드 상태 불러오기
    public void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName; // 키값에 업그레이드네임 할당

        upgradeButton.level = PlayerPrefs.GetInt(key + "_level", 1); // 레벨에 붙여넣기, 저장된값 없으면 1
        upgradeButton.goldByUpgrade = PlayerPrefs.GetInt(key + "_goldByUpgrade", upgradeButton.startGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefs.GetInt(key + "_currentCost", upgradeButton.startCurrentCost);
    }
    // 업그레이드 상태 저장
    public void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName; // 키값에 업그레이드네임 할당

        // 현재 업그레이드 정보 저장
        PlayerPrefs.SetInt(key + "_level", upgradeButton.level);
        PlayerPrefs.SetInt(key + "_goldByUpgrade", upgradeButton.goldByUpgrade);
        PlayerPrefs.SetInt(key + "_currentCost", upgradeButton.currentCost);
    }

    public void LoadItemButton(ItemButton itemButton)
    {
        string key = itemButton.itemName;

        itemButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        itemButton.goldPerSec = PlayerPrefs.GetInt(key + "_goldPerSec", itemButton.goldPerSec);
        itemButton.currentCost = PlayerPrefs.GetInt(key + "_currentCost", itemButton.currentCost);

        if (PlayerPrefs.GetInt(key + "_isPurchased") == 1)
        {
            itemButton.isPurchased = true;
        }
        else
        {
            itemButton.isPurchased = false;
        }
    }

    public void SaveItemButton(ItemButton itemButton)
    {
        string key = itemButton.itemName;

        PlayerPrefs.SetInt(key + "_level", itemButton.level);
        PlayerPrefs.SetInt(key + "_goldPerSec", itemButton.goldPerSec);
        PlayerPrefs.SetInt(key + "_currentCost", itemButton.currentCost);

        if (itemButton.isPurchased == true)
        {
            PlayerPrefs.SetInt(key + "_isPurchased", 1);
        }
        else
        {
            PlayerPrefs.SetInt(key + "_isPurchased", 0);
        }
    }

    public int GetGoldPerSec()
    {
        int goldPerSec = 0;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            goldPerSec += itemButtons[i].goldPerSec;
        }

        return goldPerSec;
    }
}
