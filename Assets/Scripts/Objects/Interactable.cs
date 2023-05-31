using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalTypes;

public class Interactable : MonoBehaviour
{
    [SerializeField] InteractableType type;
    public InteractableType Type { get { return type; } }
}
