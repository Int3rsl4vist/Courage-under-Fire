using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI Elements:")]
    public TextMeshProUGUI objectiveText;
    public GameObject missionCompleteScreen;
    public GameObject missionFailedScreen;

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
    }
    private void Update()
    {
        if(gameOver && Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        Debug.Log("CODE_LOG: Mission Complete");
        gameOver = true;
        if(missionCompleteScreen != null)
            missionCompleteScreen.SetActive(true);
        FreezeGame();
    }
    public void FailMission(string reason)
    {
        if (gameOver)
            return;
        Debug.Log($"CODE_LOG: Mission failed: {reason}");
        if(objectiveText != null)
            objectiveText.text = $"MISSION FAILED: {reason}";
        if(missionFailedScreen != null)
            missionFailedScreen.SetActive(false);
        FreezeGame();
    }
    private void UpdateObjectiveText()
    {
        if (objectiveText != null)
            objectiveText.text = $"Objective: {currentObjective}";
    }
    private void FreezeGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
