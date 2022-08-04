using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    public class objEllipse: ObjectBase
    {
        public PointF Center { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double Rotation { get; set; }
        public double MajprAxis { get; set; }
        public double MinorAxis { get; set; }
        public bool IsFullEllipse { get; set; }
    }
}
