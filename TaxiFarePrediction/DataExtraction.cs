using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Runtime.Data;

namespace TaxiFarePrediction
{
    public class DataExtraction
    {
        public List<TextLoader.Column> TextLoaderColumns { get; set; }
        public TextLoader textLoader { get; set; }
        private string pathToTrainingData;
        private MLContext MLContext;

        public DataExtraction(MLContext mlContext, string path)
        {
            textLoader = null;
            this.pathToTrainingData = path;
            this.MLContext = mlContext;
        }

        private DataKind GetDataType(string str)
        {
            // Place checks higher in if-else statement to give higher priority to type.
            if (bool.TryParse(str, out bool _))
                return DataKind.BL;
            else if (float.TryParse(str, out float _))
                return DataKind.R4;
            else if (DateTime.TryParse(str, out DateTime _))
                return DataKind.DateTime;
            else return DataKind.Text;
        }
        private TextLoader.Column[] ReadColumnsAndDataTypes(string Separator, List<string> header = null)
        {
            TextLoaderColumns = new List<TextLoader.Column>();
            using(var stream = new StreamReader(pathToTrainingData))
            {
                TextInfo pascalTI = new CultureInfo("en-US", false).TextInfo;
                List<string> textColumns = header;
                if (header == null)
                {
                    textColumns = stream.ReadLine()
                        .Split(Separator)
                        .Select(col => Regex.Replace(pascalTI.ToTitleCase(col), "[-_@,\\.\";'\\\\]", string.Empty))
                        .ToList();
                }
                string[] secondRow = stream.ReadLine().Split(Separator);
                for (int index = 0; index < secondRow.Length; index++)
                {
                    DataKind dataType = GetDataType(secondRow[index]);
                    TextLoaderColumns.Add(new TextLoader.Column(
                        textColumns[index],
                        dataType,
                        index
                    ));
                }
            }
            return TextLoaderColumns.ToArray();
        }

        public void ConstructDataStructure(string Separator, bool HasHeader = true)
        {
            textLoader = MLContext.Data.TextReader(new TextLoader.Arguments()
            {
                TrimWhitespace = true,
                    UseThreads = true,
                    Separator = Separator,
                    HasHeader = HasHeader,
                    Column = ReadColumnsAndDataTypes(Separator)
            });

        }
    }
}