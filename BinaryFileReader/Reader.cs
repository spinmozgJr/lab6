using System;
using System.IO;
using System.Text;

namespace BinaryFileReader
{
    public class Reader
    {
        public double GetValues(string path, int x, int y)
        {
            double value = 0;
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    int sizeX = reader.ReadInt32();
                    int sizeY = reader.ReadInt32();

                    int position = sizeX * x + y;
                    int cnt = 0;
                    try
                    {
                        do
                        {
                            value = reader.ReadDouble();
                            cnt++;
                        } while (position >= cnt);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    if (cnt < position || position < 0)
                    {
                        Console.WriteLine("НЕ удалось считать заданное значение");
                        return double.NaN;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return value;
        }
    }
}
