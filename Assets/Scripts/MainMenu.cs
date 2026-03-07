using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels:")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    private void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Mission_Training");
    }
    public void QuitGame()
    {
        Debug.Log("CODE_LOG: Quitting...");
        Application.Quit();
    }
    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        if(optionsPanel != null)
            optionsPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        if(mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }
}