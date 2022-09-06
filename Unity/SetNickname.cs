using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetNickname : MonoBehaviour
{
    private GameObject parent;
    
    private void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        gameObject.GetComponent<TextMeshPro>().text = parent.name;
    }
}
