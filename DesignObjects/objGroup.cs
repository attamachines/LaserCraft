using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    public class objGroup
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }

        public object Parent { get; set; }

        public bool Lock { get; set; }

        public ObjectType Type { get; set; }
    }
}
