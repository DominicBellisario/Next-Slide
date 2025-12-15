using UnityEngine;

public class LVar : MonoBehaviour
{
    [SerializeField] string slideName;
    public string SlideName { get { return slideName; } }

    [SerializeField] float slideNumber;
    public float SlideNumber { get { return slideNumber; } }

    [SerializeField] float levelNumber;
    public float LevelNumber { get { return levelNumber; } }
    [SerializeField] bool finalLevel;

    [SerializeField] float timeSpent;
    [SerializeField] int deaths;
    //[SerializeField] bool foundSecret;

    GVar gVar;

    void OnEnable()
    {
        PlayerEffects.ShatterPlayer += AddToDeaths;
        Helper.LoadingScene += SaveData;
    }
    void OnDisable()
    {
        PlayerEffects.ShatterPlayer -= AddToDeaths;
        Helper.LoadingScene -= SaveData;
    }

    void Start()
    {
        gVar = GVar.Instance;
        if (finalLevel && PlayerPrefs.GetFloat("FastestTime_L" + levelNumber, 0f) > gVar.CurrentTimeInLevel)
        {
            PlayerPrefs.SetFloat("FastestTime_L" + levelNumber, gVar.CurrentTimeInLevel);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        timeSpent += Time.deltaTime;
    }

    private void AddToDeaths() { deaths++; }

    private void SaveData()
    {
        // set total time
        AddToFloat("TotalTime_L" + levelNumber, timeSpent);
        // set current run time
        gVar.CurrentTimeInLevel += timeSpent;
        // set deaths
        AddToInt("Deaths_L" + levelNumber, deaths);
        PlayerPrefs.Save();
    }

    private void AddToFloat(string tag, float num)
    {
        if (num == 0) return;
        float currentValue = PlayerPrefs.GetFloat(tag, 0f);
        PlayerPrefs.SetFloat(tag, currentValue + num);
    }

    private void AddToInt(string tag, int num)
    {
        if (num == 0) return;
        int currentValue = PlayerPrefs.GetInt(tag, 0);
        PlayerPrefs.SetInt(tag, currentValue + num);
    }
}
