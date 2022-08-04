using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    public class objLayer
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public bool Lock { get; set; }

        public ObjectType Type { get; set; }
    }
}
