using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPlayersListManager : MonoBehaviour
{
    [SerializeField] private Transform _topListContent;
    [SerializeField] private SetMessageString _topListItemPrefab;

    public void GetTopPlayersButton()
    {
        GetTopPlayers();
    }
    private void GetTopPlayers()
    {
        foreach (Transform child in _topListContent)
        {
            GameObject.Destroy(child.gameObject);
        }

        using (var wc = new WebClient())
        {
            string result = wc.DownloadString(Hosting.Adress() + "top-players.php");
            string[] topPlayers = result.Split(new char[]{';'});

            foreach (string player in topPlayers)
            {
                SetMessageString listMember = Instantiate(_topListItemPrefab, _topListContent);
                if (listMember != null && player.Length > 1)
                {
                    listMember.SetString(player);
                }
            }
        }
    }
}
