using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoeShaderAndTriggerInteract : MonoBehaviour
{
    public enum ElementState { normal, blue, red, white };
    public ElementState element_state = ElementState.normal;

    Renderer rend;

    Color red = new Color();


    private void Awake()
    {
        rend = GetComponent<Renderer>();
        //rend.material.shader = Shader.Find("ZoeColor");

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            Destroy(collision.gameObject);
            rend.material.SetColor("_ColorShirt", Color.red);
        }
    }

    private void Update()
    {
        //float col = Mathf.PingPong(Time.time, 1.0f);
        //rend.material.SetFloat("ColorShirt", col);
    }
}
