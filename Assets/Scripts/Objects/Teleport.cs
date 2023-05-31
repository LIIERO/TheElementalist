using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teleport : MonoBehaviour
{
    [SerializeField] GameObject cp_to_teleport_to;
    public GameObject Checkpoint { get { return cp_to_teleport_to; } }
}
