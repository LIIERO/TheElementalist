using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTypes : MonoBehaviour
{
    public enum ZoeColor { red, blue, white, brown, green }; // Global color set
    public enum ElementState { normal, water, fire, air, earth };
    public enum InteractableType { teleport, checkpoint }; // checkpoint is also a teleport that brings you back
}
