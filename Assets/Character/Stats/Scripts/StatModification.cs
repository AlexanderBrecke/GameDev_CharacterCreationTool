namespace StatSystem
{
    public enum StatModificationType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }

    public class StatModification
    {

        public readonly float Value;
        public readonly StatModificationType Type;
        public readonly int Order;
        public readonly object Source;

        public StatModification(float value, StatModificationType type, int order, object source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public StatModification(float value, StatModificationType type) : this(value, type, (int)type, null) { }
        public StatModification(float value, StatModificationType type, int order) : this(value, type, order, null) { }
        public StatModification(float value, StatModificationType type, object source) : this(value, type, (int)type, source) { }


    }
}
