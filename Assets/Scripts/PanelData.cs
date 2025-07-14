using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelData : MonoBehaviour
{
    public int id;

    public TextMeshProUGUI idText;  // TextMeshProUGUIなら変更

    void Start()
    {
        if (idText != null)
        {
            idText.text = "Stage" + id.ToString();
        }
    }

    // IDが後からセットされる場合は専用メソッドを作っておくと良い
    public void SetId(int newId)
    {
        id = newId;
        if (idText != null)
        {
            idText.text = id.ToString();
        }
    }
}
