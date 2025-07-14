using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �X�^�[�g�{�^���������ꂽ���̏������s���N���X
/// �I�����ꂽ�X�e�[�WID���擾���A���̃V�[���ɑJ�ڂ���
/// </summary>
public class StartButtonHandler : MonoBehaviour
{
    // �X�e�[�W�I���p�l���Ǘ��X�N���v�g�i�C���X�y�N�^�[�Ŏw��܂��͎����擾�j
    public SelectMane selectMane;

    void Awake()
    {
        // selectMane �����w��Ȃ�V�[�������玩���擾
        if (selectMane == null)
        {
            selectMane = FindObjectOfType<SelectMane>();
            if (selectMane == null)
            {
                Debug.LogError("SelectMane ���V�[���Ɍ�����܂���I");
            }
        }
    }

    /// <summary>
    /// �X�^�[�g�{�^���������ꂽ�Ƃ��ɌĂ΂�郁�\�b�h
    /// �I�����ꂽ�X�e�[�WID��ۑ����AGameScene �ɑJ�ڂ���
    /// </summary>
    public void OnStartButtonPressed()
    {
        // ���ݑI������Ă���p�l���� ID ���擾
        int selectedId = selectMane.GetCurrentPanelId();
        Debug.Log("[StartButton] �I��ID: " + selectedId);

        // PlayerPrefs �ɕۑ��i���̃V�[���ŃX�e�[�WID���g�����߁j
        PlayerPrefs.SetInt("SelectedPanelId", selectedId);
        PlayerPrefs.Save();
        Debug.Log("[StartButton] PlayerPrefs�ɕۑ������B�V�[���J�ڊJ�n�B");

        // �Q�[���V�[���֑J�ځi�V�[�����͐��m�ɐݒ肳��Ă���K�v����j
        SceneManager.LoadScene("GameScene");
    }
}
