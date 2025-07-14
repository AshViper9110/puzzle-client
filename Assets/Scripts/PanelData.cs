using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelData : MonoBehaviour
{
    public int id;

    public TextMeshProUGUI idText;  // TextMeshProUGUI�Ȃ�ύX

    void Start()
    {
        if (idText != null)
        {
            idText.text = "Stage" + id.ToString();
        }
    }

    // ID���ォ��Z�b�g�����ꍇ�͐�p���\�b�h������Ă����Ɨǂ�
    public void SetId(int newId)
    {
        id = newId;
        if (idText != null)
        {
            idText.text = id.ToString();
        }
    }
}
