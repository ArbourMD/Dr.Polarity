using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/FX/AimLineFX", order = 1)]
public class AimLineFX : ScriptableObject
{
    [System.Serializable]
    public struct LineColorSwap
    {
        public LayerMask mask;
        public Gradient color;
    }
    public LineRenderer linePreview;
    public Gradient defaultColor;
    public LineColorSwap[] colorSwaps;
}
