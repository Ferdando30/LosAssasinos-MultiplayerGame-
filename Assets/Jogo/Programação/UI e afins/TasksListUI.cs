using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System.Text;

public class QuestListUI : MonoBehaviour
{
    public TaskManager taskManager;
    public TMP_Text questText;

    private void Update()
    {
        if (taskManager == null)
        {
            return;
        }

        UpdateQuestText();
    }

    private void UpdateQuestText()
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine("TASKS");

        for (int i = 0; i < taskManager.tasks.Count; i++)
        {
            bool completed = taskManager.IsTaskCompleted(i);

            string marker = completed ? "[X]" : "[ ]";

            text.AppendLine(
                marker + " " + taskManager.tasks[i]
            );
        }

        questText.text = text.ToString();
    }
}