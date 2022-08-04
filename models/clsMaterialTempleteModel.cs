using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    [Serializable]
    public class clsMaterialTempleteMode
    {
        private static clsMaterialTempleteMode _Instance = new clsMaterialTempleteMode();
        public static clsMaterialTempleteMode GetInstance()
        {
            return _Instance;
        }

        public int id { get; set; }
        public string Object { get; set; }
        public int FeedRate { get; set; }
        public byte LaserIntensity { get; set; }
        public byte RepeatTimes { get; set; }
        public byte Thickness { get; set; }
    }
}
