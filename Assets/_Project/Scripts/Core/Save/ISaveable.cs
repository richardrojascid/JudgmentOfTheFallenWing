namespace JudgmentOfTheFallenWing.Core.Save
{
    public interface ISaveable
    {
        void CaptureState(SaveData data);
        void RestoreState(SaveData data);
    }
}
