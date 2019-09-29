using System;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    const int objectivesPerLevel = 3;
    private Objective[] Objectives = new Objective[objectivesPerLevel];
    public TextMeshProUGUI[] ObjectiveTexts = new TextMeshProUGUI[objectivesPerLevel];
    public GameObject playerNotification;
    private TextMeshProUGUI notificationText;
    private Animator notificationAnimator;
    public Animator objectiveAnimator;
    private bool pendingLevelUp;
    private int playerLevel
    {
        get => ProgressManager.runtimePlayerProgress.playerLevel;
        set
        {
            pendingLevelUp = false;

            ProgressManager.runtimePlayerProgress.playerLevel = value;

            for(int i = 0; i < Objectives.Length; i++)
            {
                Objectives[i] = new Objective(level: playerLevel, index: i, this);
            }

            ProgressManager.runtimePlayerProgress.ResetObjectiveCompletion();
        }
    }

    private void Start() {
        ProgressManager.LoadProgress();

        for(int i = 0; i < Objectives.Length; i++)
        {
            Objectives[i] = new Objective(level: playerLevel, index: i, this);
            Refresh(i);
        }

        notificationText = playerNotification.GetComponentInChildren<TextMeshProUGUI>();
        notificationAnimator = playerNotification.GetComponent<Animator>();
    }

    void EnemyDied(string[] data)
    {
        string murderWeapon = data[0];
        
        for(int i = 0; i < Objectives.Length; i++)
        {
            if(Objectives[i].type == Objective.Types.AllKills){
                IncrementObjectiveProgress(i);
            }
            else if(Objectives[i].type == Objective.Types.SwordKills && murderWeapon.Equals("SwordUpgrade")){
                IncrementObjectiveProgress(i);
            }
            else if(Objectives[i].type == Objective.Types.GunKills && murderWeapon.StartsWith("Bullet")){
                IncrementObjectiveProgress(i);
            }
        }
    }

    void IncrementObjectiveProgress(int index){
        Objectives[index].completion++;
    }

    public void Refresh(int index) {
        if(Objectives[index] == null) return;
        ObjectiveTexts[index].text = Objectives[index].description;

        if(Objectives[index].isCompleted){
            ObjectiveTexts[index].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            ObjectiveTexts[index].text += "    <sprite=0>";
        }
        else{
            ObjectiveTexts[index].color = new Color(1f, 1f, 1f, 0.7843f);
        }
    }

    public void ObjectiveComplete(int index)
    {
        playerNotification.SetActive(true);
        notificationText.text = "Objective Complete!    <sprite=0>\n" + Objectives[index].description;
        notificationAnimator.SetTrigger("notifyPlayer");

        bool allCompleted = true;
        foreach (var objective in Objectives)
        {
            allCompleted &= objective.isCompleted;
        }
        if(allCompleted) pendingLevelUp = true;
    }

    public void ConcludeRound()
    {
        if(pendingLevelUp)
        {
            playerLevel++;

            objectiveAnimator.SetTrigger("levelUp");
            Invoke("ShowNewObjectives", 2.667f);
        }

        ProgressManager.SaveProgress();
    }

    void ShowNewObjectives(){
        for(int i = 0; i < Objectives.Length; i++)
        {
            Refresh(i);
        }
    }

    void Restart(){
        foreach (var objective in Objectives)
        {
            if(objective.isVolatile && !objective.isCompleted){
                objective.completion = 0;
            }
        }
    }

    public void ResetProgress(){
        ProgressManager.ResetProgress();

        Start();
    }
}
