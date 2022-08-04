namespace netDxf.Entities
{
    using netDxf;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class HatchPattern : ICloneable
    {
        private readonly string name;
        private readonly List<HatchPatternLineDefinition> lineDefinitions;
        private HatchStyle style;
        private HatchFillType fill;
        private HatchType type;
        private Vector2 origin;
        private double angle;
        private double scale;
        private string description;

        public HatchPattern(string name) : this(name, null, string.Empty)
        {
        }

        public HatchPattern(string name, IEnumerable<HatchPatternLineDefinition> lineDefinitions) : this(name, lineDefinitions, string.Empty)
        {
        }

        public HatchPattern(string name, string description) : this(name, null, description)
        {
        }

        public HatchPattern(string name, IEnumerable<HatchPatternLineDefinition> lineDefinitions, string description)
        {
            this.name = string.IsNullOrEmpty(name) ? string.Empty : name;
            this.description = string.IsNullOrEmpty(description) ? string.Empty : description;
            this.style = HatchStyle.Normal;
            this.fill = (this.name == "SOLID") ? HatchFillType.SolidFill : HatchFillType.PatternFill;
            this.type = HatchType.UserDefined;
            this.origin = Vector2.Zero;
            this.angle = 0.0;
            this.scale = 1.0;
            this.lineDefinitions = (lineDefinitions == null) ? new List<HatchPatternLineDefinition>() : new List<HatchPatternLineDefinition>(lineDefinitions);
        }

        public virtual object Clone()
        {
            HatchPattern pattern = new HatchPattern(this.name, this.description) {
                Style = this.style,
                Fill = this.fill,
                Type = this.type,
                Origin = this.origin,
                Angle = this.angle,
                Scale = this.scale
            };
            foreach (HatchPatternLineDefinition definition in this.lineDefinitions)
            {
                pattern.LineDefinitions.Add((HatchPatternLineDefinition) definition.Clone());
            }
            return pattern;
        }

        public static HatchPattern FromFile(string file, string patternName)
        {
            HatchPattern pattern = null;
            using (StreamReader reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
            {
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (str == null)
                    {
                        throw new FileLoadException("Unknown error reading pat file.", file);
                    }
                    if (!str.StartsWith(";") && str.StartsWith("*"))
                    {
                        int index = str.IndexOf(',');
                        string name = str.Substring(1, index - 1);
                        string description = str.Substring(index + 1, (str.Length - index) - 1).Trim();
                        if (name.Equals(patternName, StringComparison.OrdinalIgnoreCase))
                        {
                            str = reader.ReadLine();
                            if (str == null)
                            {
                                throw new FileLoadException("Unknown error reading pat file.", file);
                            }
                            pattern = new HatchPattern(name, description);
                            while ((!reader.EndOfStream && !str.StartsWith("*")) && !string.IsNullOrEmpty(str))
                            {
                                char[] separator = new char[] { ',' };
                                string[] strArray = str.Split(separator);
                                double num2 = double.Parse(strArray[0]);
                                Vector2 vector = new Vector2(double.Parse(strArray[1]), double.Parse(strArray[2]));
                                Vector2 vector2 = new Vector2(double.Parse(strArray[3]), double.Parse(strArray[4]));
                                HatchPatternLineDefinition item = new HatchPatternLineDefinition {
                                    Angle = num2,
                                    Origin = vector,
                                    Delta = vector2
                                };
                                for (int i = 5; i < strArray.Length; i++)
                                {
                                    item.DashPattern.Add(double.Parse(strArray[i]));
                                }
                                pattern.LineDefinitions.Add(item);
                                pattern.Type = HatchType.UserDefined;
                                str = reader.ReadLine();
                                if (str == null)
                                {
                                    throw new FileLoadException("Unknown error reading pat file.", file);
                                }
                                str = str.Trim();
                            }
                            return pattern;
                        }
                    }
                }
            }
            return pattern;
        }

        public static HatchPattern Solid =>
            new HatchPattern("SOLID", "Solid fill") { type=HatchType.Predefined };

        public static HatchPattern Line
        {
            get
            {
                HatchPattern pattern = new HatchPattern("LINE", "Parallel horizontal lines");
                HatchPatternLineDefinition item = new HatchPatternLineDefinition {
                    Angle = 0.0,
                    Origin = Vector2.Zero,
                    Delta = new Vector2(0.0, 0.125)
                };
                pattern.LineDefinitions.Add(item);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        public static HatchPattern Net
        {
            get
            {
                HatchPattern pattern = new HatchPattern("NET", "Horizontal / vertical grid");
                HatchPatternLineDefinition item = new HatchPatternLineDefinition {
                    Angle = 0.0,
                    Origin = Vector2.Zero,
                    Delta = new Vector2(0.0, 0.125)
                };
                pattern.LineDefinitions.Add(item);
                item = new HatchPatternLineDefinition {
                    Angle = 90.0,
                    Origin = Vector2.Zero,
                    Delta = new Vector2(0.0, 0.125)
                };
                pattern.LineDefinitions.Add(item);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        public static HatchPattern Dots
        {
            get
            {
                HatchPattern pattern = new HatchPattern("DOTS", "A series of dots");
                HatchPatternLineDefinition item = new HatchPatternLineDefinition {
                    Angle = 0.0,
                    Origin = Vector2.Zero,
                    Delta = new Vector2(0.03125, 0.0625)
                };
                double[] collection = new double[2];
                collection[1] = -0.0625;
                item.DashPattern.AddRange(collection);
                pattern.LineDefinitions.Add(item);
                pattern.type = HatchType.Predefined;
                return pattern;
            }
        }

        public string Name =>
            this.name;

        public string Description
        {
            get => 
                this.description;
            set => 
                (this.description = value);
        }

        public HatchStyle Style
        {
            get => 
                this.style;
            internal set => 
                (this.style = value);
        }

        public HatchType Type
        {
            get => 
                this.type;
            set => 
                (this.type = value);
        }

        public HatchFillType Fill
        {
            get => 
                this.fill;
            internal set => 
                (this.fill = value);
        }

        public Vector2 Origin
        {
            get => 
                this.origin;
            set => 
                (this.origin = value);
        }

        public double Angle
        {
            get => 
                this.angle;
            set => 
                (this.angle = MathHelper.NormalizeAngle(value));
        }

        public double Scale
        {
            get => 
                this.scale;
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The scale can not be zero or less.");
                }
                this.scale = value;
            }
        }

        public List<HatchPatternLineDefinition> LineDefinitions =>
            this.lineDefinitions;
    }
}

