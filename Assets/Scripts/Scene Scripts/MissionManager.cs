using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MissionSubStep
{
    public string subStepID;
    public string description;
    public bool isCompleted;
}

[System.Serializable]
public class MissionStep
{
    [Header("Main Objective:")]
    public string stepName;
    public string description;
    public bool isCompleted;

    [Header("Sub-Step Settings:")]
    public bool enforceSubOrder;
    public List<MissionSubStep> subSteps = new();
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
    public bool enforceMainOrder = true;

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
    public void CompleteStep(string idToComplete)
    {
        if (isGameOver) return;
        MissionStep mainStep = missionSteps.Find(x => x.stepName == idToComplete);
        if (mainStep != null)
        {
            if (mainStep.subSteps.Count > 0)
                return;

            CompleteMainStepLogic(mainStep);
            return;
        }
        foreach (var step in missionSteps)
        {
            if (step.isCompleted) continue;

            MissionSubStep subStep = step.subSteps.Find(x => x.subStepID == idToComplete);
            if (subStep != null)
            {
                if (enforceMainOrder)
                {
                    int mainIndex = missionSteps.IndexOf(step);
                    if (mainIndex > 0 && !missionSteps[mainIndex - 1].isCompleted)
                    {
                        Debug.Log("CODE_LOG: You can't complete Sub-Steps until the prevous objective is completed");
                        return;
                    }
                }
                if (step.enforceSubOrder)
                {
                    int subIndex = step.subSteps.IndexOf(subStep);
                    if (subIndex > 0 && !step.subSteps[subIndex - 1].isCompleted)
                    {
                        Debug.Log("CODE_LOG: Complete Sub-Steps in order");
                        return;
                    }
                }
                if (!subStep.isCompleted)
                {
                    subStep.isCompleted = true;

                    Debug.Log($"CODE_LOG: Sub-Step '{subStep.description}' completed");

                    CheckIfMainStepIsFinished(step);
                    UpdateObjectiveUI();
                }
                return;
            }
        }
        Debug.LogWarning($"CODE_WARNING: Step ID '{idToComplete}' not found");
    }
    void CheckIfMainStepIsFinished(MissionStep step)
    {
        bool allSubsDone = true;
        foreach(var subStep in step.subSteps)
            if(!subStep.isCompleted) allSubsDone = false;
        if (allSubsDone)
            CompleteMainStepLogic(step);
    }
    void CompleteMainStepLogic(MissionStep step)
    {
        if (enforceMainOrder)
        {
            int i = missionSteps.IndexOf(step);
            if (i > 0 && !missionSteps[i - 1].isCompleted) return;
        }
        if (!step.isCompleted)
        {
            step.isCompleted = true;

            Debug.Log($"CODE_LOG: Main objective ({step.description}) completed");
            AudioManager.Instance?.PlayObjectiveComplete();
            CheckWinCondition();

            if (!isGameOver)
                UpdateObjectiveUI();
        }
    }
    void CheckWinCondition()
    {
        bool allDone = true;

        foreach (var step in missionSteps)
            if (!step.isCompleted) return;
        if(allDone)
            EndGame(true);
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
        if (objectiveText == null) return;

        MissionStep curStep = missionSteps.Find(x => !x.isCompleted);

        if (curStep != null)
        {
            string finalText = $"OBJECTIVE: \n {curStep.description} \n\n";

            foreach (var sub in curStep.subSteps)
            {
                if (sub.isCompleted)
                    finalText += $"<s><color=green>- {sub.description} </color></s>\n";
                else
                    finalText += $"- {sub.description} \n";
            }

            objectiveText.text = finalText;
        }
        else
            objectiveText.text = "";
    }
}
