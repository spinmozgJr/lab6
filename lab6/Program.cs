using System;
using System.IO;

namespace lab6
{
    class Program
    {
        private enum Errors : int
        {
            noError = 0,
            rootOfNegativeNumber = 1,
            overflow = 2,
            error = 3
        }
        static Errors f(double x, double y, ref double result)
        {
            try
            {
                if (x > double.MaxValue || y > double.MaxValue)
                    return Errors.overflow;

                double problemPlace = Math.Cos(x + y);
                if (problemPlace < 0)
                    return Errors.rootOfNegativeNumber;

                result = Math.Sqrt(problemPlace);
                return Errors.noError;
            }
            catch (Exception)
            {
                return Errors.error;
            }
        }

        static void Main(string[] args)
        {
            //начальная точка, конечная точка, количество шагов
            double[][] x = {
                new double[] { 1, 2, 4 },
                new double[] { 2, 6, 10 },
                new double[] { -10, 10, 10 },
                new double[] { -1, 10, 20 },
            };

            //начальная точка, конечная точка, количество шагов
            double[][] y = {
                new double[] { 1, 3, 4 },
                new double[] { 1, 10, 10 },
                new double[] { -10, -5, 6 },
                new double[] { 50, 100, 5 },
            };

            //запись исходных данных
            for (int i = 0; i < x.GetLength(0); i++)
            {
                string path = "Calc.ini";
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    double stepX = (x[i][1] - x[i][0]) / x[i][2];
                    double stepY = (y[i][1] - y[i][0]) / y[i][2];
                    int countOfIterationsX = (int)x[i][2] + 1;
                    int countOfIterationsY = (int)y[i][2] + 1;
                    double valueX = x[i][0];
                    double valueY = y[i][0];

                    for (int j = 0; j < countOfIterationsX; j++)
                    {
                        valueY = y[i][0];
                        for (int k = 0; k < countOfIterationsY; k++)
                        {
                            writer.Write(valueX);
                            writer.Write(valueY);
                            valueY = y[i][0] + stepY * k;
                        }
                        valueX = x[i][0] + stepX * j;
                    }
                }
            }

            //запись результатов вычисления в файл
            int cnt = 0;
            using (StreamWriter errorsWriter = new StreamWriter("myErrors.log", false))
            {
                for (int i = 0; i < x.GetLength(0); i++)
                {
                    string path = $"G{i + 1:0000}.rez";
                    using (BinaryWriter valuesWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                    {
                        double stepX = (x[i][1] - x[i][0]) / x[i][2];
                        double stepY = (y[i][1] - y[i][0]) / y[i][2];
                        int countOfIterationsX = (int)x[i][2] + 1;
                        int countOfIterationsY = (int)y[i][2] + 1;
                        double valueX = x[i][0];
                        double valueY = y[i][0];

                        valuesWriter.Write(countOfIterationsX);
                        valuesWriter.Write(countOfIterationsY);

                        for (int j = 0; j < countOfIterationsX; j++)
                        {
                            valueY = y[i][0];
                            for (int k = 0; k < countOfIterationsY; k++)
                            {
                                double result = double.NaN;
                                cnt++;
                                switch (f(valueX, valueY, ref result))
                                {
                                    case Errors.noError:
                                        valuesWriter.Write(result);
                                        break;
                                    case Errors.rootOfNegativeNumber:
                                        errorsWriter.WriteLine($"X:{valueX} Y:{valueY} корень из отрицательного числа");
                                        valuesWriter.Write(double.NaN);
                                        break;
                                    case Errors.overflow:
                                        errorsWriter.WriteLine($"X:{valueX} Y:{valueY} переполнение");
                                        valuesWriter.Write(double.NaN);
                                        break;
                                    default:
                                        errorsWriter.WriteLine($"X:{valueX} Y:{valueY} ошибка");
                                        valuesWriter.Write(double.NaN);
                                        break;
                                }
                                valueY = y[i][0] + stepY * (k + 1);
                            }
                            valueX = x[i][0] + stepX * (j + 1);
                        }
                        Console.WriteLine(cnt);
                    }
                }

                Console.WriteLine("Проверка записи данных в файл");

                for (int i = 0; i < x.GetLength(0); i++)
                {
                    string path = $"G{i + 1:0000}.rez";
                    using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                    {
                        Console.WriteLine($"\n\n\n{path}");
                        int sizeX = reader.ReadInt32();
                        int sizeY = reader.ReadInt32();

                        double stepX = (x[i][1] - x[i][0]) / x[i][2];
                        double stepY = (y[i][1] - y[i][0]) / y[i][2];
                        int countOfIterationsX = (int)x[i][2] + 1;
                        int countOfIterationsY = (int)y[i][2] + 1;
                        double valueX = x[i][0];
                        double valueY = y[i][0];

                        for (int j = 0; j < countOfIterationsX; j++)
                        {
                            valueY = y[i][0];
                            for (int k = 0; k < countOfIterationsY; k++)
                            {
                                double result = double.NaN;
                                f(valueX, valueY, ref result);
                                Console.WriteLine($"X:{valueX} Y:{valueY} f(x,y): {result}");
                                Console.WriteLine(reader.ReadDouble());
                                valueY += stepY;
                            }
                            Console.WriteLine("---------------------------------------");
                            valueX += stepX;
                        }
                    }
                }

                var reader1 = new BinaryFileReader.Reader();
                Console.WriteLine(reader1.GetValues($"G0001.rez", 4, 4)); 
                Console.WriteLine(reader1.GetValues($"G0003.rez", 0, 0)); 
            }
        }
    }
}

