using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Hosting
{
    public static string Adress()
    {
        return "http://youtubekiller.ru";
    }
    public static string UpdateStats()
    {
        using (var wc = new WebClient())
        {
            if (PlayerPrefs.GetString("Nickname") != "")
            {
                return wc.DownloadString(string.Format("{0}update-stats.php?name={1}&money={2}&skin1={3}&skin2={4}&skin3={5}&skin4={6}&skin5={7}&skin6={8}", Adress(), PlayerPrefs.GetString("Nickname"), PlayerPrefs.GetInt("Money"), PlayerPrefs.GetInt("HasSkin1"), PlayerPrefs.GetInt("HasSkin2"), PlayerPrefs.GetInt("HasSkin3"), PlayerPrefs.GetInt("HasSkin4"), PlayerPrefs.GetInt("HasSkin5"), PlayerPrefs.GetInt("HasSkin6")));
            }
        }
        return "(302) Неизвестная ошибка обновления статистики";
    }
    public static string GetStats(string name)
    {
        using (var wc = new WebClient())
        {
            return wc.DownloadString(Adress() + "get-stats.php?name=" + name);
        }
    }
}
