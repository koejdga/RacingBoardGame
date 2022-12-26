using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dissolve : MonoBehaviour
{
    Material material;

    public bool isDissolving;
    bool isAppearing;
    float fade = 1;

    private void Start()
    {
        material = GetComponent<Image>().material;
        material.SetFloat("_Fade", 1);
    }

    void Update()
    {
        if (isDissolving)
        {
            fade -= Time.deltaTime;

            if (fade <= 0)
            {
                fade = 0;
                isDissolving = false;
            }

            material.SetFloat("_Fade", fade);
        }

        if (isAppearing)
        {
            fade += Time.deltaTime;

            if (fade >= 1)
            {
                fade = 1;
                isAppearing = false;
            }

            material.SetFloat("_Fade", fade);
        }
    }
}
