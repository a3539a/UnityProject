using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text goldDisplayer;
    public Text goldPerClickDisplayer;
    public Text goldPerSecDisplayer;

    private void Update()
    {
        goldDisplayer.text = "GOLD : <color=#F5F563>" + DataController.Instance.gold + "</color>";
        goldPerClickDisplayer.text = "GOLD PER CLICK : <color=#F5F563>" + DataController.Instance.goldPerClick + "</color>";
        goldPerSecDisplayer.text = "GOLD PER SEC : <color=#F5F563>" + DataController.Instance.GetGoldPerSec() + "</color>";
    }
}
