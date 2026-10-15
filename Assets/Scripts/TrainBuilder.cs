using UnityEngine;
using System.Collections.Generic;

public class TrainBuilder : MonoBehaviour
{
    public GameObject[] coachPrefabs;
    public float coachLength = 22f;

    void Awake()
    {
        Dictionary<CoachType, int> counts = new Dictionary<CoachType, int>();

        for (int i = 0; i < coachPrefabs.Length; i++)
        {
            if (coachPrefabs[i] == null) continue;

            Vector3 pos = new Vector3(0f, 0f, i * coachLength);
            GameObject c = Instantiate(coachPrefabs[i], pos, Quaternion.identity, transform);

            CoachInfo info = c.GetComponent<CoachInfo>();
            if (info != null)
            {
                info.ApplyType();

                int n;
                counts.TryGetValue(info.type, out n);
                n++;
                counts[info.type] = n;

                info.coachCode = info.CodePrefix() + n;
                c.name = "Coach_" + info.coachCode;
            }
            else
            {
                c.name = coachPrefabs[i].name + "_" + (i + 1);
            }
        }
    }
}