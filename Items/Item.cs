using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField]
    public string Name;

    [SerializeField]
    public Sprite Icon;
}