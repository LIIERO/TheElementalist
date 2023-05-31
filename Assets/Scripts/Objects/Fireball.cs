using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float speed;
    private void FixedUpdate()
    {
        transform.position += Time.deltaTime * transform.localScale.x * speed * Vector3.right;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
