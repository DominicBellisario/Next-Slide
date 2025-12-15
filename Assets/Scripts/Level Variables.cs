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
        // save fastest time if last level and if time is faster
        if (!finalLevel) return;

        string key = "FastestTime_L" + levelNumber;
        float savedTime = PlayerPrefs.GetFloat(key, float.MaxValue);

        if (gVar.CurrentTimeInLevel < savedTime)
        {
            PlayerPrefs.SetFloat(key, gVar.CurrentTimeInLevel);
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
