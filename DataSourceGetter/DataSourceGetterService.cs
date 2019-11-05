using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataSourceGetter
{
    public class DataSourceGetterService
    {
        private static Dictionary<string, DataSourceDoc> dataSourceDictionary;
        private readonly ILogger<DataSourceGetterService> _logger;
        IOptions<ApplicationConfiguration> _config;

        public DataSourceGetterService(ILogger<DataSourceGetterService> logger, IOptions<ApplicationConfiguration> config)
        {
            _logger = logger;
            _config = config;
            dataSourceDictionary = new Dictionary<string, DataSourceDoc>();

            var dataSourceDirectory = _config.Value.DataSourceFilePath;
            if (Directory.Exists(dataSourceDirectory))
            {
                string[] fileEntries = Directory.GetFiles(dataSourceDirectory, "*.csv");
                foreach (string filePath in fileEntries)
                {
                    try
                    {
                        dataSourceDictionary.Add(Path.GetFileName(filePath), new DataSourceDoc(filePath, _logger));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error: {ex}");
                    }
                }
            }
        }

        public string GetRowData(string key)
        {
            if (dataSourceDictionary.TryGetValue(key, out DataSourceDoc dataSourceDoc))
            {
                return dataSourceDoc.GetRowData;
            }
            _logger.LogError($"DataSourceGetterService Method: GetRowData Error:не удалось получить строку из файла {key}");
            return $"не удалось получить строку из файла {key}";
        }

        public IEnumerable<string> GetCurrentState()
        {
            var result = new List<string>();
            foreach (var item in dataSourceDictionary)
            {
                result.Add(item.Value.GetDataSourceState);
            }
            return result;
        }

        public void SaveCurrentStates()
        {
            foreach (var item in dataSourceDictionary)
            {
                var fileLines = item.Value.GetUnUsedLines();
                var fileName = Path.GetFileName(item.Value.FilePath).Replace(".csv","-unused.csv");

                File.WriteAllLines(Path.Combine(_config.Value.DataSourceFilePath, fileName), fileLines);
            }
        }
    }
}
