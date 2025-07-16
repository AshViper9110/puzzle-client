using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMane : MonoBehaviour
{
  public void SelectScene()
    {
        SceneManager.LoadScene("StageSelectScene");
    }
}
