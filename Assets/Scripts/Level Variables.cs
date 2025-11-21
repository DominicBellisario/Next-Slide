using UnityEngine;

public class LevelVariables : MonoBehaviour
{
    [SerializeField] string slideName;
    public string SlideName { get { return slideName; } }

    [SerializeField] float slideNumber;
    public float SlideNumber { get { return slideNumber; } }

    [SerializeField] float levelNumber;
    public float LevelNumber { get { return levelNumber; } }
}
