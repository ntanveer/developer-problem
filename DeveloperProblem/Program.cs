using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using System.IO;
using System.Globalization;

namespace DeveloperProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"SampleData\input.txt";
            var contents = File.ReadAllText(filePath).Split('\n');
            var rows = from line in contents
                      select line.RemoveWhitespace().Split(',').ToArray();
            var columnHeadings = rows.FirstOrDefault();

            int productNameIndex = Array.IndexOf(columnHeadings, Constants.ColumnHeadings.Product);
            int originYearIndex = Array.IndexOf(columnHeadings, Constants.ColumnHeadings.OriginYear);
            int developmentYearIndex = Array.IndexOf(columnHeadings, Constants.ColumnHeadings.DevelopmentYear);
            int incrementalValueIndex = Array.IndexOf(columnHeadings, Constants.ColumnHeadings.IncrementalValue);

            var originYearValues = new List<string>();
            var developmentYearValues = new List<string>();
            var productNameValues = new List<string>();
            var incrementalValues = new List<string>();

            var triangles = new List<Triangle>();

            foreach (var row in rows.Skip(1))
            {
                originYearValues.Add(row[originYearIndex]);
                developmentYearValues.Add(row[developmentYearIndex]);
                productNameValues.Add(row[productNameIndex]);
                incrementalValues.Add(row[incrementalValueIndex]);

                triangles.Add(new Triangle()
                {
                    DevelopmentYear = row[developmentYearIndex],
                    IncrementalValue = row[incrementalValueIndex],
                    ProductName = row[productNameIndex],
                    OriginYear = row[originYearIndex]
                });
            }

            var earliestOriginYear = originYearValues.Min(originYear => originYear);
            var developmentYearCount = developmentYearValues.Distinct().Count();
            var distinctProductNames = productNameValues.Distinct();

            var fileStream = new FileStream("./output.txt", FileMode.Create, FileAccess.Write);
            var streamWriter = new StreamWriter(fileStream);

            var defaultOutputStream = Console.Out;

            Console.SetOut(streamWriter);
            Console.WriteLine(earliestOriginYear + "," + developmentYearCount);


            foreach (var productName in distinctProductNames)
            {
                var triangle = productName;

                var accumulatedValue = 0.0;


                foreach (var triangleObj in triangles.OrderBy(x => x.DevelopmentYear))
                {
                    triangle += ",";

                    if (triangleObj.ProductName == productName)
                    {
                        if (triangleObj.DevelopmentYear == triangleObj.OriginYear)
                        {
                            accumulatedValue = double.Parse(triangleObj.IncrementalValue);
                            triangle += accumulatedValue;
                        }
                        else
                        {
                            accumulatedValue += double.Parse(triangleObj.IncrementalValue);
                            triangle += accumulatedValue;
                        }
                    }
                    else
                    {
                        triangle += accumulatedValue.Equals(0.0) ? 0.0 : accumulatedValue;
                    }


                }

                Console.WriteLine(triangle);
            }


            Console.SetOut(defaultOutputStream);
            streamWriter.Close();
            fileStream.Close();

            Console.WriteLine("Output.txt created...");

            Console.ReadLine();
        }

    }

    public class Triangle
    {
        public string ProductName { get; set; }
        public string DevelopmentYear { get; set; }
        public string IncrementalValue { get; set; }
        public string OriginYear { get; set; }
    }

    public static class Constants
    {
        public static class ColumnHeadings
        {
            public const string Product = "Product";
            public const string OriginYear = "OriginYear";
            public const string DevelopmentYear = "DevelopmentYear";
            public const string IncrementalValue = "IncrementalValue";
        }
    }

    public static class Extensions
    {
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    }


}
