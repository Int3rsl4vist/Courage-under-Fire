using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI Elements:")]
    public TextMeshProUGUI objectiveText;
    public GameObject missionCompleteScreen;
    public GameObject missionFailedScreen;

    [Header("HUD:")]
    public GameObject hud;

    [Header("Player Controller:")]
    public MonoBehaviour playerController;

    [Header("Settings:")]
    public string currentObjective = "";

    private bool gameOver = false;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        UpdateObjectiveText();

        if(missionCompleteScreen != null)
            missionCompleteScreen.SetActive(false);
        if(missionFailedScreen != null)
            missionFailedScreen.SetActive(false);
        if(hud != null) 
            hud.SetActive(true);
    }
    private void Update()
    {
        if(gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void UpdateObjective(string newObjective)
    {
        currentObjective = newObjective;
        UpdateObjectiveText();
    }
    public void CompleteMission()
    {
        if (gameOver)
            return;
        //Debug.Log("CODE_LOG: Mission Complete");
        EndGame(true);
    }
    public void FailMission(string reason)
    {
        if (gameOver)
            return;
        //Debug.Log($"CODE_LOG: Mission failed: {reason}");
        if(objectiveText != null)
            objectiveText.text = $"MISSION FAILED: {reason}";
        EndGame(false);
    }
    private void EndGame(bool hasPlayerWon)
    {
        Debug.Log($"CODE_LOG: Game Over. Won: {hasPlayerWon}");
        gameOver = true;

        if(hud != null)
            hud.SetActive(false);
        if(playerController != null)
            playerController.enabled = false;
        if(hasPlayerWon && missionCompleteScreen != null)
            missionCompleteScreen.SetActive(true);
        else if(!hasPlayerWon && missionFailedScreen != null)
            missionFailedScreen.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void UpdateObjectiveText()
    {
        if (objectiveText != null)
            objectiveText.text = $"Objective: {currentObjective}";
    }
}
