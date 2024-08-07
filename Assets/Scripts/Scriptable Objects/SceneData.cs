using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/SceneData", order = 1)]
public class SceneData : ScriptableObject
{
    [Header("Loading")]
    public string sceneName;
    public int sceneIndex;
    [Header("Loading Text")]
    public string stageString;
    public int stageNumber;
    [Header("Codes")]
    public int levelCode = 123456;
    public int numberElementToUnlock = 0;
}
