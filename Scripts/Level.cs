using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level {

    [SerializeField] private string levelName;
    public string LevelName { get => levelName; }

    [SerializeField] private string sceneName;
    public string SceneName { get => sceneName; }

    [SerializeField] private bool isAvailable;
    public bool IsAvailable { get => isAvailable; }

    [SerializeField] private SpriteRenderer image;
    public SpriteRenderer Image { get => image; }
}
