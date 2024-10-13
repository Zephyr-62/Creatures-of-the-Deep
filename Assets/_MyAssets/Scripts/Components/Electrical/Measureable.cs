using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Measureable : MonoBehaviour
{
    public abstract float Measure();
    public abstract Vector2 GetRange();
}
