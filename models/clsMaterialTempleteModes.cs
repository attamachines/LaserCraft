using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laserCraft_Control
{
    [Serializable]
    public class clsMaterialTempleteModes
    {
        public static clsMaterialTempleteModes Instance { get; set; }
       

        public List<clsMaterialTempleteMode> Data { get; set; }
    }
}
