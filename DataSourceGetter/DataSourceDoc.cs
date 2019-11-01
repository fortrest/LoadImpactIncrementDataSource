using System.IO;
using System.Linq;
using System.Threading;

namespace DataSourceGetter
{
    public class DataSourceDoc
    {
        public DataSourceDoc(string filePath)
        {
            Name = filePath;
            SourceLines = File.ReadAllLines(filePath).ToArray();
            currentIncrement = -1;
            maxRowsCount = SourceLines.Length;
        }

        public string Name { get; private set; }
        private string[] SourceLines { get; }
        private int currentIncrement;
        private int maxRowsCount;

        public string GetRowData
        {
            get
            {
                var rowNumber = Interlocked.Increment(ref currentIncrement);
                if (rowNumber > maxRowsCount) return "ERROR: file reach the end";

                return SourceLines[rowNumber];
            }
        }

        public string GetDataSourceState
        {
            get
            {
                return $"File {Name} now in row {currentIncrement}, totalrows {maxRowsCount}";
            }
        }
    }
}
