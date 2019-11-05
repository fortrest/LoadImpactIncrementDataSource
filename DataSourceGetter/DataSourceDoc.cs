using Microsoft.Extensions.Logging;
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
        }

        public string FilePath { get; private set; }
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

                return SourceLines[rowNumber];
            }
        }

        public string GetDataSourceState
        {
            get
            {
                return $"File {FilePath} now in row {currentIncrement + 1}, totalrows {maxRowsCount}";
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
