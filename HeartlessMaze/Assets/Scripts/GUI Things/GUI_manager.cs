using TMPro;
using UnityEditor.Experimental.GraphView;
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

        if (dropdown)
            microphoneListener.setDropdown(dropdown);

        if (indicator)
            microphoneListener.setIndicator(indicator);
    }
}
