using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
public class GameSettings : ScriptableObject {
    public string playerName;
    public bool addition;
    public bool subtraction;
    public bool multiplication;
    public bool division;
    public int score;
    public int minValue = 2;
    public int maxValue = 10;

}