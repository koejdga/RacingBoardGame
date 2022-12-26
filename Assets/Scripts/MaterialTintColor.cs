using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTintColor : MonoBehaviour
{
    private Material material;
    private Color materialTintColor;
    private float tintFadeSpeed;

    private void Awake()
    {
        materialTintColor = new Color(1, 0, 0, 0);
        material = GetComponent<MeshRenderer>().material;
        tintFadeSpeed = 6;
    }

    private void Update()
    {
        if (materialTintColor.a > 0)
        {
            materialTintColor.a = Mathf.Clamp01(materialTintColor.a - tintFadeSpeed * Time.deltaTime);
            material.SetColor("_Color", materialTintColor);
        }
    }

    public void SetTintColor(Color color)
    {
        materialTintColor = color;
        material.SetColor("_Color", materialTintColor);
    }

    public void SetTintFadeSpeed(float tintFadeSpeed)
    {
        this.tintFadeSpeed = tintFadeSpeed;
    }
}
