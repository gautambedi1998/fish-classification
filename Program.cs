using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishClassifier
{
    class Program
    {

        //Main dataset
        private static List<string> dataSet = new List<string>();
        private const int dataColumns = 3;

        //Input: Takes string collection
        //Output: return int matrix in r,g,b
        //Logic: converts string r,g,b to int r,g,b value
        private static int[,] ConvertStringToIntegerArray(List<string> fields)
        {
            int[,] data = new int[fields.Count, dataColumns];
            for (int i = 0; i < fields.Count; i++)
            {
                string[] field = fields[i].Split(',');
                for (int j = 0; j < dataColumns; j++)
                {
                    data[i, j] = Convert.ToInt32(field[j]);
                }
            }

            return data;
        }


        //Input: Takes string value
        //Output: return int array in r,g,b
        //Logic: converts string r,g,b to int r,g,b value
        private static int[] ConvertStringToIntegerArray(string fields)
        {
            //int[,] data = new int[1, dataColumns];
            //for (int i = 0; i < 1; i++)
            //{
            //    string[] field = fields.Split(',');
            //    for (int j = 0; j < dataColumns; j++)
            //    {
            //        data[i, j] = Convert.ToInt32(field[j]);
            //    }
            //}
            int[] data = new int[3]; 
            string[] field = fields.Split(',');
            for (int j = 0; j < dataColumns; j++)
            {
                data[j] = Convert.ToInt32(field[j]);
            }

            return data;
        }

        //Input: Takes only file Name
        //Output: array of integers, filled with csv file data.
        //Logic: extract data from csv and slice into r,g,b only, and converted into int array
        private static int[,] loadCSV(string fileName)
        {
         
            string[] linesInCSV = File.ReadAllLines(fileName);
            int numSamples = linesInCSV.Length;

            const int numFields = 3; // The three fields are  r, g, b
            int[,] data = new int[numSamples, numFields];
            for (int i = 0; i < numSamples; i++)
            {
                string[] fields = linesInCSV[i].Split(',');
                for (int j = 0; j < numFields; j++)
                {
                    data[i, j] = Convert.ToInt32(fields[j+2]);
                }
            }
            return data;
        }
        //Input: Takes int array
        //Output: one string r,g,b value
        //Logic: compute average of r ,g and b of each int array. seprated by ,
        private static string ComputeAvgRGBFromEachFile(int[,] data)
        {
            int[] r = new int[data.GetLength(0)];
            for (int i = 0; i < data.GetLength(0); i++)
            {     
                  r[i] = data[i, 0];           
            }

            double rAvg = r.Average();

            int[] g = new int[data.GetLength(0)];
            for (int i = 1; i < data.GetLength(0); i++)
            {
                g[i] = data[i,1];
            }

            double gAvg = g.Average();

            int[] b = new int[data.GetLength(0)];
            for (int i = 2; i < data.GetLength(0); i++)
            {
                b[i] = data[i,2];    
            }

            double bAvg = b.Average();

            Console.WriteLine("R avg = " + rAvg);
            Console.WriteLine("G avg = " + gAvg);
            Console.WriteLine("B avg = " + bAvg);
            Console.WriteLine("===================");

            string dataPoint = Convert.ToInt32(rAvg) + "," + Convert.ToInt32(gAvg) + "," + Convert.ToInt32(bAvg);
            return dataPoint;
            
        }


        //Input: Folder Paths of fishes and non-fishes
        //Output: Dataset Populated
        //Logic: Gets Files from directory and passing a helper function called ExtractAndPrepareDataFromFiles()
        private static void InitilizeDataSet()
        {

            string folderPathForFish = "../../DataSet/Training/Fish";
            string folderPathForNoFish = "../../DataSet/Training/NoFish";

            DirectoryInfo d = new DirectoryInfo(folderPathForFish); 

            FileInfo[] fishFiles = d.GetFiles("*.csv"); //Getting csv files

            DirectoryInfo d2 = new DirectoryInfo(folderPathForNoFish);

            FileInfo[] noFishFiles = d2.GetFiles("*.csv"); //Getting csv files


            ExtractAndPrepareDataFromFiles(fishFiles);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("No fish starts from here");
            ExtractAndPrepareDataFromFiles(noFishFiles);

           
          

        }

        //Input: File Paths of fishes and nonfishes
        //Output: Populated dataSet property
        //Logic: Gets Files  and passing a helper function called loadCSV(), then the result will be converted and add the dataSet collection.
        private static void ExtractAndPrepareDataFromFiles(FileInfo[] files)
        {
            foreach (var file in files)
            {
                var data = loadCSV(file.FullName);
                dataSet.Add(ComputeAvgRGBFromEachFile(data)); 
            }

        }


        //Input: int array and k value, which is centroid of dataset
        //Output: write the output
        //Logic: when data is populated, kmeans algo takes data and process its centroid, after that we predict (PredictClass()) by giving sample data using GetPredictionDataFromFolder() function
        private static void ApplyKMeans(int[,] data, int k)
        {
            
            KMeans kmeans = new KMeans(data, k, 30);
            
            // [12,3,4]
            var result = kmeans.PredictClass(GetPredictionDataFromFolder());

            if(result.Key == 0)
            {
                Console.WriteLine("Fish Detected");
                           
            }
            else
            {
                Console.WriteLine("No Fish Detected");
            }
        }


        //Input: full path of sample data
        //Output: int array of average sample data
        //Logic: Gets File from directory and passing a helper function called ComputeAvgRGBFromEachFile() which will be converted to int array using ConvertStringToIntegerArray()
        private static int[] GetPredictionDataFromFolder()
        {
            string testData = "../../DataSet/Testing/nofish.csv";

            string data = ComputeAvgRGBFromEachFile(loadCSV(testData));

            return ConvertStringToIntegerArray(data);
        }


        static void Main(string[] args)
        {
            //Step1 : Populate Dataset
            InitilizeDataSet();

            if (dataSet == null && dataSet.Count <= 0)
            {
                Console.WriteLine("No training dataset found.");
                return;
            }

            //Step 2: Learn and Predict fish
            ApplyKMeans(ConvertStringToIntegerArray(dataSet), 2);


            Console.ReadKey();
        }
    }
}
