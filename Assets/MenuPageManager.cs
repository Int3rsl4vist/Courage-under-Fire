using System;
using Unity.VisualScripting;
using UnityEngine;

public class MenuPageManager : MonoBehaviour
{
    [Header("Pages:")]
    public GameObject mainMenuPanel;
    public GameObject singlePlayerPanel;
    public GameObject optionsPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SwitchPage(mainMenuPanel);
    }
    public void ShowSinglePlayer()
    {
        SwitchPage(singlePlayerPanel);
    }
    public void ShowOptions()
    {
        SwitchPage(optionsPanel);
    }
    private void SwitchPage(GameObject panelToShow)
    {
        if(mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if(singlePlayerPanel != null) singlePlayerPanel.SetActive(false);
        if(optionsPanel != null) optionsPanel.SetActive(false);

        if(panelToShow != null) panelToShow.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("CODE_LOG: Quitting...");
        Application.Quit();
    }
}
