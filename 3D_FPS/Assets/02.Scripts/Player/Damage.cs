using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    float iniHp = 100f;
    float currHp;

    // delegate ����
    // ��������Ʈ Ÿ���� void ���ΰ��� EnemyAI�� �ִ�
    // void OnPlayerDie() �Լ��� Ÿ���� ���߷��� �ϴ°�
    public delegate void PlayerDieHandler();
    // ��������Ʈ ������� �� ������ ������
    // �� �� �̺�Ʈ ���� ��������Ʈ�� ����Ͽ� �̺�Ʈ�� �����
    // �ش� �̺�Ʈ�� ȣ���� �޼ҵ带 ������� ȣ���ϵ��� ��
    public static event PlayerDieHandler OnPlayerDieEvent;

    // UI ��ҵ�
    public Image bloodScreen; // �ǰ�ȿ��
    public Image hpBar; // �÷��̾� ü��
    readonly Color iniColor = new Vector4(0, 1f, 0, 1f);
    Color currColor;

    void Start()
    {
        currHp = iniHp;

        hpBar.color = iniColor; // hpBar ���� ����
        currColor = iniColor; // ���� ü�� ���� ����
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.3f, 0.6f));

        yield return new WaitForSeconds(0.1f);

        bloodScreen.color = Color.clear; // ���� �ʱ�ȭ
    }

    void DisplayHpBar()
    {
        // ���� ü���� ��ġ�� 50%���� ���� ��
        float hpPercent = currHp / iniHp;
        if (hpPercent > 0.5f)
        {
            // ���� ü���� ������ �����ϸ鼭
            // ��������� hp������ �������� �������Ѽ�
            // ��� > ��������� �����ϴ� �κ�
            currColor.r = (1 - hpPercent) * 2f;
        }
        else // ���� ü���� 50% ������ ���
        {
            // ����� ������ ���� ����
            // ����� > ������
            currColor.g = hpPercent * 2f;
        }

        hpBar.color = currColor; // ü�¹ٿ� ���� ����
        hpBar.fillAmount = hpPercent; // ü�¹� �������� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BULLET"))
        {
            // �ǰ�ȿ�� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(ShowBloodScreen());

            Destroy(other.gameObject);

            currHp -= 5f;

            DisplayHpBar();

            if (currHp <= 0f)
            {
                // �÷��̾� ���� �Լ� ȣ��
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        // �̺�Ʈ �Ҵ�
        OnPlayerDieEvent();
        // ���ӸŴ������� ���ӿ����� �˸�
        GameManager.instance.isGameOver = true;

        //print("�÷��̾� ���");
        // ENEMY �±׸� ���� ��� ������Ʈ ã�Ƽ� �迭�� ����
        /*
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("ENEMY");
        foreach (GameObject enemy in enemies)
        {
                // enemy.GetComponent<EnemyAI>().OnPlayerDie(); �Ϲ����� �Լ� ȣ��
                // SendMessage �Լ��� Ư�� ���ӿ�����Ʈ�� ���Ե� ��ũ��Ʈ�� �ڵ����� �Ⱦ
                // SendMessage("�Լ��̸�", ��������)
                // �ش� �Լ� �̸��� ������ �Լ� ȣ���ϴ� ���
                enemy.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        // ���� : ������ ȣ���̱� ������ ó���� ���� �����ð��� �߻�
        // ��, ���ÿ� �ൿ�� ���� �� ���̰� �߻��Ѵ�.
        }
        */
    }

    void Update()
    {
        
    }
}
