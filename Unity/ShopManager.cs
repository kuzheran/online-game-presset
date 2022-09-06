using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject[] _checkMarks;
    [SerializeField] Text _moneyText;

    private void Start()
    {
        UpdateMoney();

        if (PlayerPrefs.GetInt("CurrentSkin") == 0)
        {
            PlayerPrefs.SetInt("HasSkin1", 1);
            PlayerPrefs.SetInt("CurrentSkin", 1);
        }

        SetCheckMark(PlayerPrefs.GetInt("CurrentSkin"));
    }
    private void SetCheckMark(int skinId)
    {
        for (int i = 0; i < 6; i++)
        {
            _checkMarks[i].SetActive(false);
        }
        _checkMarks[skinId - 1].SetActive(true);
        PlayerPrefs.SetInt("CurrentSkin", skinId);
    }
    private void BuySkin(int skinId, int cost)
    {
        PlayerPrefs.SetInt("HasSkin" + skinId.ToString(), 1);
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") - cost);
        UpdateMoney();
        Hosting.UpdateStats();
    }
    public void SetSkinButton(int skinId)
    {
        int[] costTable = {0, 0, 20, 150, 300, 500, 1000};
        int cost = costTable[skinId];
        if (PlayerPrefs.GetInt("HasSkin" + skinId.ToString()) == 1)
        {
            SetCheckMark(skinId);
        }
        else
        {
            if (PlayerPrefs.GetInt("Money") >= cost)
            {
                BuySkin(skinId, cost);
                SetCheckMark(skinId);
            }
        }
    }
    private void UpdateMoney()
    {
        _moneyText.text = "Ваши бобики: " + PlayerPrefs.GetInt("Money").ToString();
    }
}
