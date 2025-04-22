using System.Collections;
using UnityEngine;

public abstract class TileBehavior : MonoBehaviour
{
    public virtual IEnumerator Step() { yield return null; }
}
