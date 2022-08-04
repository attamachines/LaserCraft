namespace netDxf.Entities
{
    using System;

    public class DatumReferenceValue : ICloneable
    {
        private string value;
        private ToleranceMaterialCondition materialCondition;

        public DatumReferenceValue()
        {
            this.value = string.Empty;
            this.materialCondition = ToleranceMaterialCondition.None;
        }

        public DatumReferenceValue(string value, ToleranceMaterialCondition materialCondition)
        {
            this.value = value;
            this.materialCondition = materialCondition;
        }

        public object Clone() => 
            new DatumReferenceValue { 
                Value = this.value,
                MaterialCondition = this.materialCondition
            };

        public string Value
        {
            get => 
                this.value;
            set => 
                (this.value = value);
        }

        public ToleranceMaterialCondition MaterialCondition
        {
            get => 
                this.materialCondition;
            set => 
                (this.materialCondition = value);
        }
    }
}

