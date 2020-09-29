using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct GridPOS
{
    public int X;
    public int Y;
    public GridPOS(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }
    public override string ToString()
    {
        return $"Grid position: ({X}, {Y})";
    }
}

public class ButtonXO : MonoBehaviour
{

    public GridPOS pos;

    public void Init(GridPOS pos, UnityAction callback)
    {
        this.pos = pos;

        Button bttn = GetComponent<Button>();

        bttn.onClick.AddListener( callback );
    }
}
