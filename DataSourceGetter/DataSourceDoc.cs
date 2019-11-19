using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace DataSourceGetter
{
    public class DataSourceDoc
    {
        private readonly ILogger _logger;
        public DataSourceDoc(string filePath, ILogger logger)
        {
            FilePath = filePath;
            SourceLines = File.ReadAllLines(filePath).ToArray();
            currentIncrement = -1;
            maxRowsCount = SourceLines.Length;
            _logger = logger;
            filename = Path.GetFileName(filePath);
        }

        public string FilePath { get; private set; }
        private string filename;
        private string[] SourceLines { get; }
        private int currentIncrement;
        private int maxRowsCount;

        public string GetRowData
        {
            get
            {
                var rowNumber = Interlocked.Increment(ref currentIncrement);
                if (rowNumber >= maxRowsCount)
                {
                    _logger.LogError($"В файле {FilePath} закончились доступные строки");
                    return "ERROR: file reach the end";
                }
                _logger.LogTrace($"Успешно возвращена строка {rowNumber+1} для файла {filename}");
                return SourceLines[rowNumber];
            }
        }

        public string DataSourceState
        {
            get
            {
                return $"#{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} # File {FilePath} now in row {currentIncrement + 1}, totalrows {maxRowsCount}";
            }
        }

        public string[] GetUnUsedLines()
        {
            if (currentIncrement < 0)
            {
                return SourceLines;
            }

            return SourceLines.Skip(currentIncrement + 1).ToArray();
        }
    }
}
