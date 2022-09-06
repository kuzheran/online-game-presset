using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string VERSION;
    [SerializeField] private GameObject _newVersionMenu;
    [SerializeField] private Text _warningText;
    [SerializeField] private GameObject _banMenu;
    [SerializeField] private Text _reasonText;

    private void Start()
    {
        CheckBan();
        CheckVersion();
        UpdateStats();
    }
    private void CheckBan()
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "checkban.php?name=" + PlayerPrefs.GetString("Nickname") + "&id=" + SystemInfo.deviceUniqueIdentifier);
            if (result.Contains("501") || result.Contains("502"))
            {
                print(result);
                _banMenu.SetActive(true);
                _reasonText.text = result;
            }
        }
    }
    private void CheckVersion()
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "latest.php?name=" + PlayerPrefs.GetString("Nickname"));
            if (result != VERSION)
            {
                _newVersionMenu.SetActive(true);
                _warningText.text = "Вы используете устаревшую версию игры " + VERSION + ". Новейшая версия: " + result;
            }
        }
    }
    private void UpdateStats()
    {
        if (PlayerPrefs.GetString("Nickname") != "")
        {
            string result = Hosting.UpdateStats();
            print(result);
        }
    }
    public void QuitGameButton()
    {
        Application.Quit();
    }
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
