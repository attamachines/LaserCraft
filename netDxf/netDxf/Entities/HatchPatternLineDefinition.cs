namespace netDxf.Entities
{
    using netDxf;
    using System;
    using System.Collections.Generic;

    public class HatchPatternLineDefinition : ICloneable
    {
        private double angle = 0.0;
        private Vector2 origin = Vector2.Zero;
        private Vector2 delta = Vector2.Zero;
        private readonly List<double> dashPattern = new List<double>();

        public object Clone()
        {
            HatchPatternLineDefinition definition = new HatchPatternLineDefinition {
                Angle = this.angle,
                Origin = this.origin,
                Delta = this.delta
            };
            foreach (double num in this.dashPattern)
            {
                definition.DashPattern.Add(num);
            }
            return definition;
        }

        public double Angle
        {
            get => 
                this.angle;
            set => 
                (this.angle = MathHelper.NormalizeAngle(value));
        }

        public Vector2 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public Vector2 Delta
        {
            get => 
                this.delta;
            set => 
                (this.delta = value);
        }

        public List<double> DashPattern =>
            this.dashPattern;
    }
}

