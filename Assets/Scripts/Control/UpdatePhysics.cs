using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePhysics : MonoBehaviour
{
    void Update()
    {
        Physics2D.Simulate(Time.deltaTime);
    }
}
