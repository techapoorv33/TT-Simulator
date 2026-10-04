using UnityEngine;

public enum BerthType
{
    Lower,
    Middle,
    Upper,
    SideLower,
    SideUpper
}

public class Berth : MonoBehaviour
{
    public int number;
    public BerthType type;
}