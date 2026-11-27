using UnityEngine;
using System.Collections;

public class Cutscene : MonoBehaviour
{
    public GameObject VideoPlayer;
    public GameObject CanvasObg;

    private void Awake()
    {
        VideoPlayer.SetActive(true);
        CanvasObg.SetActive(false);

        StartCoroutine(FinishCinematic());
    }

    IEnumerator FinishCinematic()
    {
        yield return new WaitForSeconds(7f);
        VideoPlayer.SetActive(false);
        CanvasObg.SetActive(true);
    }
}
