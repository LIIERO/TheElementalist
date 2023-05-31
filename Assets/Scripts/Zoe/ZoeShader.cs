using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalTypes;

public class ZoeShader : MonoBehaviour
{

    Renderer rend;

    Color green_shirt = new Color(0.16f, 0.8f, 0.16f);
    Color green_sleeves = new Color(0.12f, 0.6f, 0.12f);

    Color blue_shirt = new Color(0.16f, 0.16f, 0.8f);
    Color blue_sleeves = new Color(0.12f, 0.12f, 0.6f);

    Color red_shirt = new Color(0.8f, 0.16f, 0.16f);
    Color red_sleeves = new Color(0.6f, 0.12f, 0.12f);

    Color white_shirt = new Color(0.8f, 0.8f, 0.8f);
    Color white_sleeves = new Color(0.6f, 0.6f, 0.6f);

    Color brown_shirt = new Color(0.588f, 0.353f, 0.118f);
    Color brown_sleeves = new Color(0.471f, 0.282f, 0.094f);


    private void Start()
    {
        rend = GetComponent<Renderer>();

        rend.material.SetColor("_ColorShirt", green_shirt);
        rend.material.SetColor("_ColorSleeves", green_sleeves);
    }

    public void UpdateColor(ElementState ref_typ)
    {
        if (ref_typ == ElementState.normal)
        {
            rend.material.SetColor("_ColorShirt", green_shirt);
            rend.material.SetColor("_ColorSleeves", green_sleeves);
        }
        if (ref_typ == ElementState.water)
        {
            rend.material.SetColor("_ColorShirt", blue_shirt);
            rend.material.SetColor("_ColorSleeves", blue_sleeves);
        }
        if (ref_typ == ElementState.fire)
        {
            rend.material.SetColor("_ColorShirt", red_shirt);
            rend.material.SetColor("_ColorSleeves", red_sleeves);
        }
        if (ref_typ == ElementState.air)
        {
            rend.material.SetColor("_ColorShirt", white_shirt);
            rend.material.SetColor("_ColorSleeves", white_sleeves);
        }
        if (ref_typ == ElementState.earth)
        {
            rend.material.SetColor("_ColorShirt", brown_shirt);
            rend.material.SetColor("_ColorSleeves", brown_sleeves);
        }
    }

}
