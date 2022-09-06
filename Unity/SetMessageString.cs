using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMessageString : MonoBehaviour
{
    [SerializeField] private Text _messageText;

    public void SetString(string text)
    {
        _messageText.text = text;
    }
}
