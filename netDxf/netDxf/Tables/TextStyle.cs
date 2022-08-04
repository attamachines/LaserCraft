namespace netDxf.Tables
{
    using netDxf.Collections;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Windows.Media;

    public class TextStyle : TableObject
    {
        private string font;
        private string bigFont;
        private double height;
        private bool isBackward;
        private bool isUpsideDown;
        private bool isVertical;
        private double obliqueAngle;
        private double widthFactor;
        private System.Windows.Media.GlyphTypeface glyphTypeface;
        private string fontFamilyName;
        public const string DefaultName = "Standard";

        public TextStyle(string font) : this(Path.GetFileNameWithoutExtension(font), font)
        {
        }

        public TextStyle(string name, string font) : this(name, font, true)
        {
        }

        internal TextStyle(string name, string font, bool checkName) : base(name, "STYLE", checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "The text style name should be at least one character long.");
            }
            if (string.IsNullOrEmpty(font))
            {
                throw new ArgumentNullException("font");
            }
            base.IsReserved = name.Equals("Standard", StringComparison.OrdinalIgnoreCase);
            this.font = font;
            this.bigFont = null;
            this.widthFactor = 1.0;
            this.obliqueAngle = 0.0;
            this.height = 0.0;
            this.isVertical = false;
            this.isBackward = false;
            this.isUpsideDown = false;
            this.glyphTypeface = null;
            this.fontFamilyName = Path.GetFileNameWithoutExtension(font);
            this.TrueTypeFontCheck(font);
        }

        public override object Clone() => 
            this.Clone(base.Name);

        public override TableObject Clone(string newName) => 
            new TextStyle(newName, this.font) { 
                Height = this.height,
                IsBackward = this.isBackward,
                IsUpsideDown = this.isUpsideDown,
                IsVertical = this.isVertical,
                ObliqueAngle = this.obliqueAngle,
                WidthFactor = this.widthFactor
            };

        private void TrueTypeFontCheck(string ttfFont)
        {
            if (string.IsNullOrEmpty(ttfFont))
            {
                throw new ArgumentNullException("ttfFont");
            }
            if (Path.GetExtension(ttfFont).Equals(".ttf", StringComparison.OrdinalIgnoreCase))
            {
                string fullPath;
                if (File.Exists(ttfFont))
                {
                    fullPath = Path.GetFullPath(ttfFont);
                }
                else
                {
                    string fileName = Path.GetFileName(ttfFont);
                    fullPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Fonts)}{Path.DirectorySeparatorChar}{fileName}";
                    if (!File.Exists(fullPath))
                    {
                        return;
                    }
                    this.font = fileName;
                }
                this.glyphTypeface = new System.Windows.Media.GlyphTypeface(new Uri(fullPath));
                this.fontFamilyName = this.glyphTypeface.FamilyNames[CultureInfo.GetCultureInfo(0x409)];
                if (string.IsNullOrEmpty(this.fontFamilyName))
                {
                    IEnumerator<string> enumerator = this.glyphTypeface.FamilyNames.Values.GetEnumerator();
                    enumerator.MoveNext();
                    this.fontFamilyName = enumerator.Current;
                }
            }
        }

        public static TextStyle Default =>
            new TextStyle("Standard", "simplex.shx");

        public string FontFile
        {
            get => 
                this.font;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                this.glyphTypeface = null;
                this.fontFamilyName = Path.GetFileNameWithoutExtension(value);
                this.TrueTypeFontCheck(value);
                if (!Path.GetExtension(value).Equals(".shx", StringComparison.OrdinalIgnoreCase))
                {
                    this.bigFont = null;
                }
                this.font = value;
            }
        }

        public string BigFont
        {
            get => 
                this.bigFont;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.bigFont = null;
                }
                else
                {
                    if (string.IsNullOrEmpty(this.font))
                    {
                        throw new ArgumentNullException("font");
                    }
                    if (!Path.GetExtension(this.font).Equals(".shx", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("The Big Font is only applicable for SHX Asian fonts.", "font");
                    }
                    if (!Path.GetExtension(value).Equals(".shx", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException("Only SHX files are valid file types.", "value");
                    }
                    this.bigFont = value;
                }
            }
        }

        public string FontFamilyName =>
            this.fontFamilyName;

        public System.Windows.Media.GlyphTypeface GlyphTypeface =>
            this.glyphTypeface;

        public double Height
        {
            get => 
                this.height;
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The TextStyle height must be equals or greater than zero.");
                }
                this.height = value;
            }
        }

        public double WidthFactor
        {
            get => 
                this.widthFactor;
            set
            {
                if ((value < 0.01) || (value > 100.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The TextStyle width factor valid values range from 0.01 to 100.");
                }
                this.widthFactor = value;
            }
        }

        public double ObliqueAngle
        {
            get => 
                this.obliqueAngle;
            set
            {
                if ((value < -85.0) || (value > 85.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "The TextStyle oblique angle valid values range from -85 to 85.");
                }
                this.obliqueAngle = value;
            }
        }

        public bool IsVertical
        {
            get => 
                this.isVertical;
            set => 
                (this.isVertical = value);
        }

        public bool IsBackward
        {
            get => 
                this.isBackward;
            set => 
                (this.isBackward = value);
        }

        public bool IsUpsideDown
        {
            get => 
                this.isUpsideDown;
            set => 
                (this.isUpsideDown = value);
        }

        public TextStyles Owner
        {
            get => 
                ((TextStyles) base.Owner);
            internal set => 
                (base.Owner = value);
        }
    }
}

