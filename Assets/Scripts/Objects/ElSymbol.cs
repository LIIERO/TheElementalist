using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalTypes;

public class ElSymbol : MonoBehaviour
{
    [SerializeField] Sprite air_sprite;
    [SerializeField] Sprite water_sprite;
    [SerializeField] Sprite fire_sprite;
    [SerializeField] Sprite earth_sprite;

    SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void AssignSprite(ElementState element)
    {
        if (element == ElementState.water) sr.sprite = water_sprite;
        if (element == ElementState.fire) sr.sprite = fire_sprite;
        if (element == ElementState.air) sr.sprite = air_sprite;
        if (element == ElementState.earth) sr.sprite = earth_sprite;
    }
}
