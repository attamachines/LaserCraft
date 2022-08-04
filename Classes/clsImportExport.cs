using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laserCraft_Control
{
    public class clsImportExport
    {
        public static T Read<T> (string fileName)
        {
            object ret = null;

            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    ret = formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = null;
            }

            return (T)ret;
        }

        public static bool Write<T> (T data, string fileName)
        {
            bool ret = false;

            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }

            return ret;
        }
    }
}
