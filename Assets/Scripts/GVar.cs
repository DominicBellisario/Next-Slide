using UnityEngine;

public class GVar : MonoBehaviour
{
    [SerializeField] int numOfSlidesInLevel;
    public int NumOfSlidesInLevel { get { return numOfSlidesInLevel; } set { numOfSlidesInLevel = value; } }

    [SerializeField] float currentTimeInLevel;
    public float CurrentTimeInLevel { get { return currentTimeInLevel; } set { currentTimeInLevel = value; } }

    public static GVar Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
