using UnityEngine;
using System.Collections;

public class Cutscene : MonoBehaviour
{
    public GameObject VideoPlayer;

    private void Awake()
    {
        VideoPlayer.SetActive(true);

        StartCoroutine(FinishCinematic());
    }

    IEnumerator FinishCinematic()
    {
        yield return new WaitForSeconds(7f);
        VideoPlayer.SetActive(false);
    }
}
