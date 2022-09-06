using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [SerializeField] GameObject _chooseAuthMethode;

    [SerializeField] Text _registerErrorText;
    [SerializeField] Text _loginErrorText;

    [SerializeField] InputField _registerNicknameField;
    [SerializeField] InputField _registerPasswordField;

    [SerializeField] InputField _loginNicknameField;
    [SerializeField] InputField _loginPasswordField;

    [SerializeField] GameObject _registerPanel;
    [SerializeField] GameObject _loginPanel;

    [SerializeField] Text _moneyText;

    private class Account
    {
        public string uid;
        public int id;
        public string name;
        public string password;
        public int money;
        public bool ban;
        public string ban_reason;
        public string ip;
        public bool skin1;
        public bool skin2;
        public bool skin3;
        public bool skin4;
        public bool skin5;
        public bool skin6;
    }
    private void Start()
    {
        if (PlayerPrefs.GetString("Nickname") == "")
        {
            _chooseAuthMethode.SetActive(true);
        }
    }
    public void RegisterButton()
    {
        string nickname = _registerNicknameField.text;
        string password = _registerPasswordField.text;
        if (CheckString(nickname) && CheckString(password))
        {
            Register(nickname, password);
        }
    }
    private void Register(string nickname, string password)
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "register.php?name=" + nickname + "&password=" + password + "&id=" + SystemInfo.deviceUniqueIdentifier);
            if (result == "(100) Аккаунт создан")
            {
                PlayerPrefs.SetString("Nickname", nickname);
                _registerPanel.SetActive(false);
            }
            else
            {
                _registerErrorText.text = "Ошибка: " + result;
            }
        } 
    }
    public void LoginButton()
    {
        string nickname = _loginNicknameField.text;
        string password = _loginPasswordField.text;
        if (CheckString(nickname) && CheckString(password))
        {
            Login(nickname, password);
        }
    }
    private void Login(string nickname, string password)
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "login.php?name=" + nickname + "&password=" + password + "&id=" + SystemInfo.deviceUniqueIdentifier);
            if (result == "(200) Успешный логин")
            {
                PlayerPrefs.SetString("Nickname", nickname);
                _loginPanel.SetActive(false);

                string json = Hosting.GetStats(nickname);
                Account account = (Account) JsonUtility.FromJson(json, typeof(Account));

                PlayerPrefs.SetInt("Money", account.money);
                PlayerPrefs.SetInt("HasSkin1", account.skin1 == true ? 1 : 0);
                PlayerPrefs.SetInt("HasSkin2", account.skin2 == true ? 1 : 0);
                PlayerPrefs.SetInt("HasSkin3", account.skin3 == true ? 1 : 0);
                PlayerPrefs.SetInt("HasSkin4", account.skin4 == true ? 1 : 0);
                PlayerPrefs.SetInt("HasSkin5", account.skin5 == true ? 1 : 0);
                PlayerPrefs.SetInt("HasSkin6", account.skin6 == true ? 1 : 0);
                _moneyText.text = "Ваши бобики: " + PlayerPrefs.GetInt("Money").ToString();
            }
            else
            {
                _loginErrorText.text = "Ошибка: " + result;
            }
        } 
    }
    public void LogOut()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }
    private bool CheckString(string str)
    {
        return (str.Length > 2 && str.Length < 21);
    }
}
