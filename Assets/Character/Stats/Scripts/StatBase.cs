using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StatSystem
{
    [Serializable]
    public class StatBase
    {

        public float BaseValue;


        public virtual float Value {
            get {
                if (isDirty || BaseValue != lastBaseValue)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            } 
        }

        protected bool isDirty = true;
        protected float _value;
        protected float lastBaseValue = float.MinValue;

        protected readonly List<StatModification> statModifications;
        public ReadOnlyCollection<StatModification> Statmodifications;


        public StatBase()
        {
            statModifications = new List<StatModification>();
            Statmodifications = statModifications.AsReadOnly();
        }
        public StatBase(float baseValue) :this()
        {
            BaseValue = baseValue;
        }

        public virtual void AddModification(StatModification mod)
        {
            isDirty = true;
            statModifications.Add(mod);
            statModifications.Sort(CompareModificationOrder);
        }

        protected virtual int CompareModificationOrder(StatModification a, StatModification b)
        {
            if(a.Order < b.Order)
            {
                return -1;
            } else if (a.Order > b.Order)
            {
                return 1;
            }
            return 0; //if (a.Order == b.Order)
        }

        public virtual bool RemoveModification(StatModification mod)
        {
            isDirty = true;
            if (statModifications.Remove(mod))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = statModifications.Count - 1; i > 0; i--)
            {
                if(statModifications[i].Source == source)
                {
                    isDirty = true;
                    didRemove = true;
                    statModifications.RemoveAt(i);
                }
            }
            return didRemove;
        }

        protected virtual float CalculateFinalValue()
        {
        
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < statModifications.Count; i++)
            {
                StatModification mod = statModifications[i];

                if(mod.Type == StatModificationType.Flat)
                {
                    finalValue += mod.Value;
                } else if(mod.Type == StatModificationType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;

                    if(i+1 >= statModifications.Count || statModifications[i + 1].Type != StatModificationType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }

                }
                else if(mod.Type == StatModificationType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }

    }
}
