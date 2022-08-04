using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    public class ObjectBase
    {
        public int FeedRate { get; set; }
        public byte LaserIntensity { get; set; }
        public byte RepeatTimes { get; set; }
        public string Name { get; set; }
        public ObjectType Type { get; set; }

        public object Parent { get; set; }

        public bool Lock { get; set; }
    }
}
