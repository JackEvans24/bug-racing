using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Track")]
public class Track : ScriptableObject
{
    public string Name;
    public Scenes Scene;
    public Sprite Image;
}
