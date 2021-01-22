using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flipbook", menuName = "Game/Flipbook")]
public class Flipbook : ScriptableObject
{
    [SerializeField] private string flipbookName;
    [SerializeField] private bool loop;
    [Min(1)]
    [SerializeField] private int fps;
    [SerializeField] private List<Sprite> frames = new List<Sprite>();

    public string FlipbookName => flipbookName;
    public List<Sprite> Frames => frames;
    public bool Loop => loop;
    public int FPS => fps;
}
