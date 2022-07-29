using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    // �̱��� ��ü ����
    // �Ź� ��������Ʈ�ѷ� ��ü �����ؼ� ��ũ��Ʈ ���� �ʾƵ�
    // �������� ��밡��
    static DataController instance; // �������� ��������Ʈ�ѷ� ����

    // instance ��ü�� DataController ã�Ƽ� �ֱ�
    public static DataController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataController>();

                if (instance == null) // ������ �����
                {
                    GameObject container = new GameObject("DataController");

                    instance = container.AddComponent<DataController>();
                }
            }
            return instance;
        }
    }

    ItemButton[] itemButtons;

    // ������Ƽ�� PlayerPrefs �� �ڷḦ �ٷ� ����
    public long gold
    {
        get 
        {
            if (!PlayerPrefs.HasKey("Gold"))
            {
                return 0;
            }

            string tmpGold = PlayerPrefs.GetString("Gold"); // ����� ���� ������ null ��ȯ
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

    // ���ӽ��۽� ���� Ŭ���� ��� ������ �ҷ��´�
    private void Awake()
    {
        itemButtons = FindObjectsOfType<ItemButton>();
    }

    // ���׷��̵� ���� �ҷ�����
    public void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName; // Ű���� ���׷��̵���� �Ҵ�

        upgradeButton.level = PlayerPrefs.GetInt(key + "_level", 1); // ������ �ٿ��ֱ�, ����Ȱ� ������ 1
        upgradeButton.goldByUpgrade = PlayerPrefs.GetInt(key + "_goldByUpgrade", upgradeButton.startGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefs.GetInt(key + "_currentCost", upgradeButton.startCurrentCost);
    }
    // ���׷��̵� ���� ����
    public void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        string key = upgradeButton.upgradeName; // Ű���� ���׷��̵���� �Ҵ�

        // ���� ���׷��̵� ���� ����
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
