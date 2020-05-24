using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab2
{
    class Bin
    {
        public static int x { get; private set; }
        public static int y { get; private set; }
        public static int z { get; private set; }
        public static short[,,] array { get; private set; }
        public static void ReadBin(string path)
        {
            if(File.Exists(path))
            {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

                x = reader.ReadInt32();
                y = reader.ReadInt32();
                z = reader.ReadInt32();

                array = new short[x, y, z];
                for (int _z = 0; _z < z; _z++)
                {
                    for (int _y = 0; _y < y; _y++)
                    {
                        for (int _x = 0; _x < x; _x++)
                        {
                            array[_x, _y, _z] = reader.ReadInt16();
                        }
                    }
                }
            }
        }

    }
}
