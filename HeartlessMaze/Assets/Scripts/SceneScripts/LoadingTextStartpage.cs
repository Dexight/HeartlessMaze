using TMPro;
using UnityEngine;
using System.Collections;
public class LoadingTextStartpage : MonoBehaviour
{
    public TMP_Text loading_text;
    public float dotDelay = 0.5f;
    int dot_count = 0;

    void Start()
    {
        loading_text.text = " Загрузка";
        StartCoroutine(UpdateDots());
    }

    IEnumerator UpdateDots()
    {
        while (true)
        {
            ++dot_count;
            if (dot_count == 4)
            {
                loading_text.text = " Загрузка";
                dot_count = 0;
            }
            else
                loading_text.text += ".";

            yield return new WaitForSeconds(dotDelay);
        }
    }
}
