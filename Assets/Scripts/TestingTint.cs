using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingTint : MonoBehaviour
{
    [SerializeField] private MaterialTintColor materialTintColor;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            materialTintColor.SetTintColor(new Color(0, 1, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            materialTintColor.SetTintColor(new Color(1, 0, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            materialTintColor.SetTintColor(new Color(0, 0, 1, 1));
        }
    }
}
