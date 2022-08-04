namespace netDxf.Tables
{
    using netDxf.Collections;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Linetype : TableObject
    {
        private string description;
        private readonly List<double> segments;
        public const string ByLayerName = "ByLayer";
        public const string ByBlockName = "ByBlock";
        public const string DefaultName = "Continuous";

        public Linetype(string name) : this(name, null, string.Empty, true)
        {
        }

        public Linetype(string name, IEnumerable<double> segments) : this(name, segments, string.Empty, true)
        {
        }

        public Linetype(string name, string description) : this(name, null, description, true)
        {
        }

        public Linetype(string name, IEnumerable<double> segments, string description) : this(name, segments, description, true)
        {
        }

        internal Linetype(string name, IEnumerable<double> segments, string description, bool checkName) : base(name, "LTYPE", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The line type name should be at least one character long.");
            }
            base.IsReserved = (name.Equals("ByLayer", StringComparison.OrdinalIgnoreCase) || name.Equals("ByBlock", StringComparison.OrdinalIgnoreCase)) || name.Equals("Continuous", StringComparison.OrdinalIgnoreCase);
            this.description = string.IsNullOrEmpty(description) ? string.Empty : description;
            this.segments = (segments == null) ? new List<double>() : new List<double>(segments);
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new Linetype(newName, this.segments, this.description);

        public static Linetype FromFile(string file, string linetypeName)
        {
            Linetype linetype = null;
            using (StreamReader reader = new StreamReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), true))
            {
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (str == null)
                    {
                        throw new FileLoadException("Unknown error reading .lin file.", file);
                    }
                    if (!str.StartsWith(";") && str.StartsWith("*"))
                    {
                        int index = str.IndexOf(',');
                        string name = str.Substring(1, index - 1);
                        string description = str.Substring(index + 1, (str.Length - index) - 1).Trim();
                        if (name.Equals(linetypeName, StringComparison.OrdinalIgnoreCase))
                        {
                            str = reader.ReadLine();
                            if (str == null)
                            {
                                throw new FileLoadException("Unknown error reading .lin file.", file);
                            }
                            linetype = new Linetype(name, description);
                            char[] separator = new char[] { ',' };
                            string[] strArray = str.Split(separator);
                            for (int i = 1; i < strArray.Length; i++)
                            {
                                if (double.TryParse(strArray[i], out double num3))
                                {
                                    linetype.Segments.Add(num3);
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            return linetype;
                        }
                    }
                }
            }
            return linetype;
        }

        public double Length()
        {
            double num = 0.0;
            foreach (double num2 in this.segments)
            {
                num += Math.Abs(num2);
            }
            return num;
        }

        public static Linetype ByLayer =>
            new Linetype("ByLayer");

        public static Linetype ByBlock =>
            new Linetype("ByBlock");

        public static Linetype Continuous =>
            new Linetype("Continuous", "Solid line");

        public static Linetype Center
        {
            get
            {
                Linetype linetype = new Linetype("Center") {
                    Description = "Center, ____ _ ____ _ ____ _ ____ _ ____ _ ____"
                };
                linetype.Segments.AddRange(new double[] { 1.25, -0.25, 0.25, -0.25 });
                return linetype;
            }
        }

        public static Linetype DashDot
        {
            get
            {
                Linetype linetype = new Linetype("Dashdot") {
                    Description = "Dash dot, __ . __ . __ . __ . __ . __ . __ . __"
                };
                linetype.Segments.AddRange(new double[] { 0.5, -0.25, 0.0, -0.25 });
                return linetype;
            }
        }

        public static Linetype Dashed
        {
            get
            {
                Linetype linetype = new Linetype("Dashed") {
                    Description = "Dashed, __ __ __ __ __ __ __ __ __ __ __ __ __ _"
                };
                double[] collection = new double[] { 0.5, -0.25 };
                linetype.Segments.AddRange(collection);
                return linetype;
            }
        }

        public static Linetype Dot
        {
            get
            {
                Linetype linetype = new Linetype("Dot") {
                    Description = "Dot, . . . . . . . . . . . . . . . . . . . . . . . ."
                };
                double[] collection = new double[2];
                collection[1] = -0.25;
                linetype.Segments.AddRange(collection);
                return linetype;
            }
        }

        public string Description
        {
            get => 
                this.description;
            set => 
                (this.description = value);
        }

        public List<double> Segments =>
            this.segments;

        public Linetypes Owner
        {
            get => 
                ((Linetypes) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

