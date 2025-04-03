using UnityEngine;
using UnityEngine.UI;

public class WaitCircle : MonoBehaviour
{
    private Transform imageTransform;
    private Image image; 
    
    private float speed = 150f;
    private float amount = 0.1f;
    private float step = 0.025f;
    public bool isWorked = false;
    void Start()
    {
        image = GetComponent<Image>();
        image.fillAmount = amount;
        imageTransform = GetComponent<Transform>();
        image.enabled = false;
    }

    void Update()
    {
        if (image.enabled)
        {
            imageTransform.Rotate(0f, 0f, speed * Time.deltaTime);
            amount += step;
            image.fillAmount = amount;

            if (amount > 1f || amount < 0f)
            {
                step = -step;
                image.fillClockwise = !image.fillClockwise;
            }
        }
    }

    public void startWaitCircle()
    {
        image.enabled = isWorked = true;
    }

    public void stopWaitCircle()
    {
        image.enabled = isWorked = false;
        Debug.Log("circleStopped");
    }
}
