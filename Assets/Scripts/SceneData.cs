using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneDataInstance", menuName = "Scene Data")]
public class SceneData : ScriptableObject
{
    public int level;
    public int score;
    public int state;
}
