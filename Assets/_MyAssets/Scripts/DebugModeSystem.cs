using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeSystem : MonoBehaviour
{
    [SerializeField] private ClickySwitch DebugSwitch;
    [SerializeField] private Button SkipQuestButton;
    [SerializeField] private Button FixMalfuntionsButton;

    [SerializeField] private QuestSystem QuestSys;
    [SerializeField] private MalfunctionSystem MalfuncSys;

    private void OnEnable()
    {
        DebugSwitch.onValueChanged.AddListener(ToggleDebugMode);
        SkipQuestButton.onReleased.AddListener(QuestSys.ForceCompleteQuest);
        FixMalfuntionsButton.onReleased.AddListener(MalfuncSys.FixAllMalfunctions);
    }

    private void OnDisable()
    {
        DebugSwitch.onValueChanged.RemoveListener(ToggleDebugMode);
        SkipQuestButton.onReleased.RemoveListener(QuestSys.ForceCompleteQuest);
        FixMalfuntionsButton.onReleased.RemoveListener(MalfuncSys.FixAllMalfunctions);
    }

    private void Start()
    {
        ToggleDebugMode();
    }

    public void ToggleDebugMode()
    {
        if (DebugSwitch.GetBoolValue())
        {
            SkipQuestButton.Unblock();
            FixMalfuntionsButton.Unblock();
        }
        else
        {
            SkipQuestButton.Block();
            FixMalfuntionsButton.Block();
        }
    }

}
