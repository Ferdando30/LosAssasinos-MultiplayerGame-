using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Cutscene : MonoBehaviour
{
    public GameObject VideoPlayer;
    public GameObject JoyStick;
    public Image TaskScreen;
    public TMP_Text TaskTxt;

    private void Awake()
    {
        VideoPlayer.SetActive(true);
        JoyStick.SetActive(false);
        TaskScreen.enabled = false;
        TaskTxt.enabled = false;

        StartCoroutine(FinishCinematic());
    }

    IEnumerator FinishCinematic()
    {
        yield return new WaitForSeconds(7f);
        VideoPlayer.SetActive(false);
        JoyStick.SetActive(true);
        TaskScreen.enabled = true;
        TaskTxt.enabled = true;
    }
}
