using UnityEngine;
using System.Collections.Generic;

public class BerthNumberer : MonoBehaviour
{
    public float labelHeight = 0.14f;
    public float characterSize = 0.05f;

    void Start()
    {
        // Find all bays and sort them from the front to the back of the coach
        List<Transform> bays = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Bay"))
                bays.Add(child);
        }
        bays.Sort((a, b) => a.localPosition.z.CompareTo(b.localPosition.z));

        for (int i = 0; i < bays.Count; i++)
        {
            int first = i * 8 + 1;
            Setup(bays[i], "MainLowerA", first + 0, BerthType.Lower, true);
            Setup(bays[i], "MainMiddleA", first + 1, BerthType.Middle, true);
            Setup(bays[i], "MainUpperA", first + 2, BerthType.Upper, true);
            Setup(bays[i], "MainLowerB", first + 3, BerthType.Lower, true);
            Setup(bays[i], "MainMiddleB", first + 4, BerthType.Middle, true);
            Setup(bays[i], "MainUpperB", first + 5, BerthType.Upper, true);
            Setup(bays[i], "SideLower", first + 6, BerthType.SideLower, false);
            Setup(bays[i], "SideUpper", first + 7, BerthType.SideUpper, false);
        }
    }

    void Setup(Transform bay, string berthName, int number, BerthType type, bool mainSide)
    {
        Transform berth = bay.Find(berthName);
        if (berth == null)
        {
            Debug.LogWarning("Berth not found: " + berthName + " in " + bay.name);
            return;
        }

        Berth b = berth.gameObject.AddComponent<Berth>();
        b.number = number;
        b.type = type;

        // Create a number label at the aisle edge of the berth
        GameObject label = new GameObject("Label_" + number);
        label.transform.SetParent(bay);

        Vector3 pos = berth.localPosition;
        float halfWidth = berth.localScale.x * 0.5f;

        if (mainSide)
        {
            pos.x += halfWidth + 0.02f;
            label.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            pos.x -= halfWidth + 0.02f;
            label.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }

        pos.y += labelHeight;
        label.transform.localPosition = pos;

        TextMesh text = label.AddComponent<TextMesh>();
        text.text = number.ToString();
        text.fontSize = 64;
        text.characterSize = characterSize;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.GetComponent<MeshRenderer>().material = text.font.material;
    }
}