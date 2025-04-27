using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GUI_manager : MonoBehaviour
{
    [ReadOnlyProperty][SerializeField] private MicrophoneListener microphoneListener;
    public TMP_Dropdown dropdown;
    public Image indicator;
    void Start()
    {

        microphoneListener = MicrophoneListener.Instance;
        microphoneListener.setDropdown(dropdown);
        microphoneListener.setIndicator(indicator);
    }
}
