using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject sensSliderObject;
    private Slider sensSlider;
    [SerializeField] private GameObject sensTextObject;
    private TMP_InputField sensText;
    [SerializeField] private GameObject player;
    private Player playerScript;

    private void Start()
    {
        sensSlider = sensSliderObject.GetComponent<Slider>();
        sensText = sensTextObject.GetComponent<TMP_InputField>();

        Debug.Log(sensText + "senssliderobject");

        playerScript = player.GetComponent<Player>();
        sensText.text = playerScript.GetSensitivity().ToString();
        sensSlider.value = playerScript.GetSensitivity();
    }
    public void UpdateTextSens() //if player uses slider it updates textbox
    {
        sensText.text = sensSlider.value.ToString();
    }

    public void ChangeSens() //this is what actually applies the change
    {
        playerScript.SetSensitivity(sensSlider.value);
    }
    public void UpdateSliderSens() //if player uses text box it changes the slider
    {
        if (sensText.text != "")
        {
            sensSlider.value = float.Parse(sensText.text);
        }

    }
}
