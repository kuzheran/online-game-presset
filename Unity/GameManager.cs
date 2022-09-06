using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField _chatInputField;
    [SerializeField] private Transform _chatContent;
    [SerializeField] private SetMessageString _messagePrefab;
    [SerializeField] private RectTransform _content;
    [SerializeField] private Text _bobikCountText;
    [SerializeField] private GameObject _deathPanel;
    [SerializeField] private GameObject _immortalModeText;
    [SerializeField] private InputField _debugInput;
 
    private GameObject _localPlayer;
    private PhotonView _photonView;
    private bool _isImmortal = false;
    private bool _isDied = false;
    private int _oldMoneyAmmount = 0;

    private void Update()
    {
        print(_isDied);
    }
    private void Start()
    {
        if (CheckBan())
        {
            QuitRoom();
        }
        UpdateBobikAmmountText();
        _photonView = gameObject.GetComponent<PhotonView>();
        _localPlayer = PhotonNetwork.Instantiate("Player" + PlayerPrefs.GetInt("CurrentSkin").ToString(), new Vector3(-27.45f, 3.66f, -37.00f), Quaternion.identity);
        if (IsMasterClient())
        {
            SpawnBoss();
        }
        CheckSameNicknames();
    }
    private void AddChatMessage(string message)
    {
        string[] messages = SplitString(message, 28);
        foreach (string result in messages)
        {
            SetMessageString listMember = Instantiate(_messagePrefab, _chatContent);
            if (listMember != null)
            {
                listMember.SetString(result);
            }
        }
        StartCoroutine(MoveChat());
    }
    private IEnumerator MoveChat()
    {
        yield return new WaitForSeconds(0.1f);
        _content.anchoredPosition = new Vector3(0, 99999999, 0);
    }
    public void SendChatMessageButton()
    {
        if (_chatInputField.text != "")
        {
            SendChatMessage(_chatInputField.text);
            _chatInputField.text = "";
        }
    }
    public void GetBobikButton()
    {
        var mainCamera = _localPlayer.transform.GetChild(1).gameObject.GetComponent<Camera>();
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)).origin, mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0)).direction, out hit, 100, 1 << 6))
        {
            print(hit.transform.gameObject.name);
            if (hit.distance < 10.0f)
            {
                if (hit.transform.gameObject.name.Contains("Бобик"))
                {
                    DestroyBobik(hit.transform.gameObject);
                }
            }
        }
    }
    private void SendChatMessage(string message)
    {
        string resultMessage = PhotonNetwork.NickName + ": " + message;
        _photonView.RPC("RPC_AddChatMessage", RpcTarget.All, resultMessage);
    }
    public void DeathAction()
    {
        if (!_isDied && !_isImmortal)
        {
            _isDied = true;
            _oldMoneyAmmount = PlayerPrefs.GetInt("Money");
            print("Saved ammount: " + _oldMoneyAmmount);
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") / 2);
            print("Leave case ammount: " + PlayerPrefs.GetInt("Money"));
            _deathPanel.SetActive(true);
        }
    }
    public void RebornAction()
    {
        _deathPanel.SetActive(false);

        _isImmortal = true;
        _isDied = false;
        _immortalModeText.SetActive(true);
        StartCoroutine(DeactivateImmortalMode());
    }
    public void RebornForBobiksButton()
    {
        if (_oldMoneyAmmount >= 40)
        {
            PlayerPrefs.SetInt("Money", _oldMoneyAmmount - 40);
            UpdateBobikAmmountText();
            RebornAction();
        }
    }
    public void RebornForAdButton()
    {
        //ShowAd();
        PlayerPrefs.SetInt("Money", _oldMoneyAmmount);
        RebornAction();
    }
    private IEnumerator DeactivateImmortalMode()
    {
        yield return new WaitForSeconds(10);
        _isImmortal = false;
        _immortalModeText.SetActive(false);
    }
    public void QuitRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }
    private void CheckSameNicknames()
    {
        int sameNames = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == PhotonNetwork.NickName)
            {
                sameNames++;
            }
        }
        if (sameNames > 1)
        {
            LeaveRoom();
        }
    }
    private void LeaveRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }
    private bool IsMasterClient()
    {
        return PhotonNetwork.MasterClient.NickName == PhotonNetwork.NickName;
    }
    private void IncreaseBobik()
    {
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money") + 1);
        UpdateBobikAmmountText();
    }
    private void DestroyBobik(GameObject bobik)
    {
        IncreaseBobik();
        if (IsMasterClient())
        {
            PhotonNetwork.Destroy(bobik);
        }
        else
        {
            Destroy(bobik);
            _photonView.RPC("RPC_DestroyObject", RpcTarget.MasterClient, bobik.GetComponent<PhotonView>().ViewID);
        }
    }
    private void BanPlayer(string name, string reason)
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "ban.php?name=" + name + "&reason=" + reason);
            print(result);
        }
        _photonView.RPC("RPC_Kick", RpcTarget.All, name);
    }
    private void UpdateBobikAmmountText()
    {
        int bobikAmmount = PlayerPrefs.GetInt("Money");
        _bobikCountText.text = bobikAmmount.ToString() + " " + GetDeclension(bobikAmmount, "БОБИК", "БОБИКА", "БОБИКОВ");
    }
    private string GetDeclension(int number, string nominativ, string genetiv, string plural)
    {
        number = number % 100;
        if (number >= 11 && number <= 19)
        {
            return plural;
        }

        var i = number % 10;
        switch (i)
        {
            case 1:
                return nominativ;
            case 2:
            case 3:
            case 4:
                return genetiv;
            default:
                return plural;
        }

    }   
    private string[] SplitString(string input, int maxStringLength)
    {
        var result = (from Match m in Regex.Matches(input, @".{1," + maxStringLength.ToString() + "}") select m.Value).ToArray();
        return result;
    }
    private void SpawnBoss()
    {
        PhotonNetwork.Instantiate("boss", new Vector3(-5.24f, 3.815f, -30.98f), Quaternion.identity);
    }
    private bool CheckBan()
    {
        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "checkban.php?name=" + PlayerPrefs.GetString("Nickname") + "&id=" + SystemInfo.deviceUniqueIdentifier);
            if (result.Contains("501") || result.Contains("502"))
            {
                return true;
            }
        }
        return false;
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        AddChatMessage("Создатель комнаты вышел, Пёс переродился. Новый создатель: " + newMasterClient.NickName);
        if (newMasterClient.NickName == PhotonNetwork.NickName)
        {
            SpawnBoss();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddChatMessage("В игру вошел " + newPlayer.NickName + ". Общий онлайн: " + PhotonNetwork.PlayerList.Length.ToString());
    }
    [PunRPC] public void RPC_AddChatMessage(string message)
    {
        AddChatMessage(message);
    }
    [PunRPC] public void RPC_DestroyObject(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }
    [PunRPC] public void RPC_Kick(string playerName)
    {
        if (playerName == PhotonNetwork.NickName)
        {
            QuitRoom();
        }
        AddChatMessage("Игрок " + playerName + " был кикнут с сервера.");
    }
    [PunRPC] public void RPC_TeleportAll(string playerName)
    {
        if (_localPlayer != null)
        {
            _localPlayer.transform.position = GameObject.Find(playerName).transform.position;
        }
    }
    [PunRPC] public void RPC_Teleport(string victim, string playerName)
    {
        if (_localPlayer != null && PhotonNetwork.NickName == victim)
        {
            _localPlayer.transform.position = GameObject.Find(playerName).transform.position;
        }
    }
    [PunRPC] public void RPC_IncreaseMoney(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            IncreaseBobik();
        }
    }
    public void Debug_TeleportAllPlayers()
    {
        _photonView.RPC("RPC_TeleportAll", RpcTarget.All, PhotonNetwork.NickName);
    }
    public void Debug_TeleportPlayer()
    {
        _photonView.RPC("RPC_Teleport", RpcTarget.All, _debugInput.text, PhotonNetwork.NickName);
    }
    public void Debug_SpawnEnemy()
    {
        SpawnBoss();
    }
    public void Debug_SendChatMessage()
    {
        _photonView.RPC("RPC_AddChatMessage", RpcTarget.All, _debugInput.text);
    }
    public void Debug_Kick()
    {
        _photonView.RPC("RPC_Kick", RpcTarget.All, _debugInput.text);
    }
    public void Debug_Ban()
    {
        BanPlayer(_debugInput.text, "бан");
    }
    public void Debug_Airdrop()
    {
        _photonView.RPC("RPC_IncreaseMoney", RpcTarget.All, int.Parse(_debugInput.text));
    }
}
