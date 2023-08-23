using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON_TO_CSV
{
    internal class Program
    {
        public static void Main()
        {
            Console.WriteLine("請輸入JSON檔案的路徑:");
            string inputPath = Console.ReadLine();
            inputPath = inputPath.Trim().Trim('"');

            try
            {
                #region 檢核
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("檔案不存在。");
                    Console.Read();
                    return;
                }

                // 建立輸出路徑
                string outputPath = Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + ".csv");

                // 檢查輸出檔案是否已存在
                if (File.Exists(outputPath))
                {
                    Console.WriteLine("輸出檔案已存在，是否要覆蓋? (Y/N)");
                    string answer = Console.ReadLine().ToUpper();
                    if (answer != "Y")
                    {
                        Console.WriteLine("轉換已取消。");
                        Console.Read();
                        return;
                    }
                }
                #endregion

                FileInfo fileInfo = new FileInfo(inputPath);
                int largeFileBytes = 1 * 1024 * 1024 * 1024; // 大檔定義1GB
                if (fileInfo.Length < largeFileBytes)
                {
                    ProcessSmallFile(inputPath, outputPath);
                }
                else
                {
                    ProcessLargeFile(inputPath, outputPath);
                }

                Console.WriteLine($"已將JSON轉換為CSV並保存在: {outputPath}");
                Console.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }

        #region 小檔處理
        /// <summary>
        /// 小檔處理
        /// </summary>
        /// <param name="inputPath">讀取路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        public static void ProcessSmallFile(string inputPath, string outputPath)
        {
            string json = File.ReadAllText(inputPath);
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            SaveToCsv(dataTable, outputPath);
            return;
        }
        private static void SaveToCsv(DataTable dataTable, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding("big5")))
            {
                // 寫入標題
                writer.WriteLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(col => QuoteValue(col.ColumnName))));

                // 寫入資料行
                foreach (DataRow row in dataTable.Rows)
                {
                    writer.WriteLine(string.Join(",", row.ItemArray.Select(item => QuoteValue(item?.ToString()))));
                }
            }
        }
        #endregion

        #region 大檔處理
        /// <summary>
        /// 大檔處理
        /// </summary>
        /// <param name="inputPath">讀取路徑</param>
        /// <param name="outputPath">輸出路徑</param>
        public static string ProcessLargeFile(string inputPath, string outputPath)
        {
            using (StreamReader sr = new StreamReader(inputPath))
            using (JsonTextReader reader = new JsonTextReader(sr))
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.GetEncoding("big5")))
            {
                bool isFirst = true;
                // 讀取JSON內容
                while (reader.Read())
                {
                    // 檢查開始標記物件
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        // 解析到DataTable
                        DataTable dataTable = JsonSerializer.CreateDefault().Deserialize<DataTable>(reader);

                        // 寫入標題只有在首行
                        if (isFirst)
                        {
                            writer.WriteLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(col => QuoteValue(col.ColumnName))));
                            isFirst = false;
                        }

                        // 寫入資料行
                        foreach (DataRow row in dataTable.Rows)
                        {
                            writer.WriteLine(string.Join(",", row.ItemArray.Select(item => QuoteValue(item?.ToString()))));
                        }
                    }
                }
            }

            return outputPath;
        }
        #endregion

        /// <summary>
        /// CSV轉義
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string QuoteValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            // 對包含逗號、換行符或雙引號的字符串進行引用和轉義
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
