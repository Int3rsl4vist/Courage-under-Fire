using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Generic;

[System.Serializable]
public class MissionStep
{
    public string stepName;
    public string stepDescription;
    public bool isCompleted;
}
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI Elements:")]
    public TextMeshProUGUI objectiveText;
    public GameObject missionCompleteScreen;
    public GameObject missionFailedScreen;

    [Header("HUD & Player:")]
    public GameObject gameHUD;
    public MonoBehaviour playerController;

    [Header("Mission Config:")]
    public List<MissionStep> missionSteps = new();

    [SerializeField]
    AudioSource audioSource;
    AudioClip stepCompleteAudio;

    public bool enforceOrder = true;

    private bool isGameOver = false;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        UpdateObjectiveUI();

        if(missionCompleteScreen != null)
            missionCompleteScreen.SetActive(false);
        if(missionFailedScreen != null)
            missionFailedScreen.SetActive(false);
        if(gameHUD != null) 
            gameHUD.SetActive(true);
    }
    private void Update()
    {
        if(isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void CompleteStep(string nameOfStep)
    {
        if (isGameOver)
            return;

        MissionStep stepToComplete = missionSteps.Find(x => x.stepName == nameOfStep);

        if (stepToComplete != null)
        {
            if (enforceOrder)
            {
                int i = missionSteps.IndexOf(stepToComplete);

                if (i > 0 && !missionSteps[i - 1].isCompleted)
                {
                    Debug.Log($"CODE_LOG: {nameOfStep} can't be completed until the previous objective is completed");
                    return;
                }
            }
            if (!stepToComplete.isCompleted)
            {
                stepToComplete.isCompleted = true;
                Debug.Log($"CODE_LOG: Objective '{stepToComplete.stepDescription}' complete");

                bool allStepsFinished = true;
                foreach (var step in missionSteps)
                {
                    if(!step.isCompleted) allStepsFinished = false;
                }
                if (allStepsFinished)
                {
                    EndGame(true);
                }
                else
                {
                    AudioManager.Instance?.PlayObjectiveComplete();
                    UpdateObjectiveUI();
                }
            }
        }
        else
            Debug.LogWarning($"CODE_WARNING: This objective is null");
    }
    public void FailMission(string reason)
    {
        if (isGameOver)
            return;
        //Debug.Log($"CODE_LOG: Mission failed: {reason}");
        if(objectiveText != null)
            objectiveText.text = $"MISSION FAILED: {reason}";
        EndGame(false);
    }
    private void EndGame(bool hasPlayerWon)
    {
        isGameOver = true;

        if(gameHUD != null)
            gameHUD.SetActive(false);
        if(playerController != null)
            playerController.enabled = false;
        if (hasPlayerWon)
        {
            AudioManager.Instance?.PlayMissionComplete();
            if(missionCompleteScreen != null)
                missionCompleteScreen.SetActive(true);
        }
        else if (!hasPlayerWon)
        {
            AudioManager.Instance?.PlayMissionFail();
            if(missionFailedScreen != null)
                missionFailedScreen.SetActive(true);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void UpdateObjectiveUI()
    {
        if (objectiveText == null)
            return;
        MissionStep curStep = missionSteps.Find(x => !x.isCompleted);

        if (curStep != null)
            objectiveText.SetText($"Objective: \n {curStep.stepDescription}");
        else
            objectiveText.SetText("Objective \n Escape!");
    }
}
