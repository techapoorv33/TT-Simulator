using UnityEngine;

public enum CoachType
{
    Sleeper,
    ThirdAC,
    SecondAC,
    FirstAC,
    ChairCar,
    ExecutiveChair,
    General
}

public class CoachInfo : MonoBehaviour
{
    public CoachType type = CoachType.Sleeper;
    public string coachCode = "S1";
    public string className = "Sleeper";
    public string classCode = "SL";

    void Awake()
    {
        ApplyType();
    }

    public void ApplyType()
    {
        switch (type)
        {
            case CoachType.Sleeper: className = "Sleeper"; classCode = "SL"; break;
            case CoachType.ThirdAC: className = "3rd AC"; classCode = "3A"; break;
            case CoachType.SecondAC: className = "2nd AC"; classCode = "2A"; break;
            case CoachType.FirstAC: className = "1st AC"; classCode = "1A"; break;
            case CoachType.ChairCar: className = "Chair Car"; classCode = "CC"; break;
            case CoachType.ExecutiveChair: className = "Executive Chair"; classCode = "EC"; break;
            case CoachType.General: className = "General"; classCode = "GEN"; break;
        }
    }

    public string CodePrefix()
    {
        switch (type)
        {
            case CoachType.Sleeper: return "S";
            case CoachType.ThirdAC: return "B";
            case CoachType.SecondAC: return "A";
            case CoachType.FirstAC: return "H";
            case CoachType.ChairCar: return "C";
            case CoachType.ExecutiveChair: return "E";
            default: return "GS";
        }
    }
}