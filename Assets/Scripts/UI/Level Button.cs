using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] int levelNum;
    [SerializeField] int numOfSlides;
    [Header("Objects")]
    [SerializeField] CanvasGroup infoGroup;
    [SerializeField] TextMeshProUGUI numOfSlidesText;
    [SerializeField] TextMeshProUGUI totalTimeText;
    [SerializeField] TextMeshProUGUI fastestTimeText;
    [SerializeField] TextMeshProUGUI deathText;

    GVar gVar;

    void Start()
    {
        gVar = GVar.Instance;
        infoGroup.alpha = 0f;
        numOfSlidesText.text = "Slides: " + numOfSlides;
        totalTimeText.text = "Total Time: " + FormatTime(PlayerPrefs.GetFloat("TotalTime_L" + levelNum));
        fastestTimeText.text = "Fastest Time: " + FormatTime(PlayerPrefs.GetFloat("FastestTime_L" + levelNum));
        deathText.text = "Retakes: " + PlayerPrefs.GetInt("Deaths_L" + levelNum);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{minutes}:{seconds:00}";
    }

    public void ToggleInfoPanel()
    {
        if (infoGroup.alpha == 0f) 
        { 
            infoGroup.alpha = 1f; 
            infoGroup.interactable = true;
            infoGroup.blocksRaycasts = true;
        }
        else 
        { 
            infoGroup.alpha = 0f; 
            infoGroup.interactable = false;
            infoGroup.blocksRaycasts = false;
        }
    }
}
