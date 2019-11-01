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

        public DataSourceGetterService(ILogger<DataSourceGetterService> logger, IOptions<ApplicationConfiguration> _config)
        {
            _logger = logger;
            dataSourceDictionary = new Dictionary<string, DataSourceDoc>();

            var dataSourceDirectory = _config.Value.DataSourceFilePath;
            if (Directory.Exists(dataSourceDirectory))
            {
                string[] fileEntries = Directory.GetFiles(dataSourceDirectory);
                foreach (string filePath in fileEntries)
                {
                    try
                    {
                        dataSourceDictionary.Add(Path.GetFileName(filePath), new DataSourceDoc(filePath));
                    }
                    catch(Exception ex)
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
            return $"Файл не найден";
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
    }
}
