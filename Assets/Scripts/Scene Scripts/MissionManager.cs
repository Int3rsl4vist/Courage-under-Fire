using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class MissionSubStep
{
    public string subStepID;
    public string description;
    public bool isCompleted;

    [Header("Events:")]
    public UnityEvent onSubStepCompleted;
}

[System.Serializable]
public class MissionStep
{
    [Header("Main Objective:")]
    public string stepName;
    public string description;
    public bool isCompleted;

    [Header("Events:")]
    public UnityEvent onStepCompleted;

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

    [Header("Overlay Settings:")]
    [Tooltip("UI Panel contaaining objective text and other info.")]
    public CanvasGroup objectiveOverlayGroup;
    public float fadeSpeed = 5f;

    private Coroutine fadeCoroutine;


    [Header("Mission Config:")]
    public List<MissionStep> missionSteps = new();
    public bool enforceMainOrder = true;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateObjectiveUI();
        InitiateUI();
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(!isGameOver && objectiveOverlayGroup != null)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                FadePanel(1f);
            }
            else if(Input.GetKeyUp(KeyCode.Tab))
            {
                FadePanel(0f);
            }
        }
    }

    public void CompleteStep(string idToComplete)
    {
        if (isGameOver) return;

        MissionStep mainStep = missionSteps.Find(x => x.stepName == idToComplete);
        if (mainStep != null)
        {
            if (mainStep.subSteps.Count > 0)
            {
                Debug.LogWarning($"CODE_WARNING: Cannot directly complete Main Step '{idToComplete}' because it has active Sub-Steps.");
                return;
            }

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
                        Debug.LogWarning("CODE_WARNING: Blocked. You must complete the previous Main Objective first.");
                        return;
                    }
                }

                if (step.enforceSubOrder)
                {
                    int subIndex = step.subSteps.IndexOf(subStep);
                    if (subIndex > 0 && !step.subSteps[subIndex - 1].isCompleted)
                    {
                        Debug.LogWarning("CODE_WARNING: Blocked. Complete Sub-Steps in chronological order.");
                        return;
                    }
                }
                if (!subStep.isCompleted)
                {
                    subStep.isCompleted = true;
                    Debug.Log($"CODE_LOG: Sub-Step '{subStep.description}' COMPLETED.");

                    subStep.onSubStepCompleted?.Invoke();

                    CheckIfMainStepIsFinished(step);
                    UpdateObjectiveUI();
                }
                return;
            }
        }

        Debug.LogError($"CODE_ERROR: Step ID '{idToComplete}' not found anywhere.");
    }

    private void CheckIfMainStepIsFinished(MissionStep step)
    {
        bool allSubsDone = true;
        foreach (var subStep in step.subSteps)
        {
            if (!subStep.isCompleted)
            {
                allSubsDone = false;
                break;
            }
        }

        if (allSubsDone)
        {
            Debug.Log($"CODE_LOG: All Sub-Steps for '{step.stepName}' are done.");
            CompleteMainStepLogic(step);
        }
    }

    private void CompleteMainStepLogic(MissionStep step)
    {
        if (enforceMainOrder)
        {
            int i = missionSteps.IndexOf(step);
            if (i > 0 && !missionSteps[i - 1].isCompleted) return;
        }

        if (!step.isCompleted)
        {
            step.isCompleted = true;
            Debug.Log($"CODE_LOG: Main Objective '{step.description}' COMPLETED.");

            step.onStepCompleted?.Invoke();

            AudioManager.Instance.PlayObjectiveComplete(); 

            CheckWinCondition();

            if (!isGameOver)
                UpdateObjectiveUI();
        }
    }

    private void CheckWinCondition()
    {
        foreach (var step in missionSteps)
        {
            if (!step.isCompleted) return;
        }

        Debug.Log("CODE_LOG: All objectives completed. Player WINS.");
        EndGame(true);
    }

    public void FailMission(string reason)
    {
        if (isGameOver) return;

        Debug.Log($"CODE_LOG: Mission FAILED. Reason: {reason}");

        if (objectiveText != null)
            objectiveText.text = $"MISSION FAILED: \n{reason}";

        EndGame(false);
    }

    private void EndGame(bool hasPlayerWon)
    {
        isGameOver = true;

        if (gameHUD != null) gameHUD.SetActive(false);
        if (playerController != null) playerController.enabled = false;

        if (hasPlayerWon)
        {
            AudioManager.Instance.PlayMissionComplete();
            if (missionCompleteScreen != null) missionCompleteScreen.SetActive(true);
        }
        else
        {
            AudioManager.Instance.PlayMissionFail();
            if (missionFailedScreen != null) missionFailedScreen.SetActive(true);
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
        {
            objectiveText.text = "";
        }
    }
    private void InitiateUI()
    {
        if (missionCompleteScreen != null) missionCompleteScreen.SetActive(false);
        if (missionFailedScreen != null) missionFailedScreen.SetActive(false);
        if (gameHUD != null) gameHUD.SetActive(true);
        if (objectiveOverlayGroup != null) objectiveOverlayGroup.alpha = 0f;
    }
    private void FadePanel(float targetAlpha)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }
    private IEnumerator FadeRoutine(float targetAlpha)
    {
        while(Mathf.Abs(objectiveOverlayGroup.alpha - targetAlpha) > 0.01f)
        {
            objectiveOverlayGroup.alpha = Mathf.Lerp(objectiveOverlayGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        objectiveOverlayGroup.alpha = targetAlpha;
    }
}