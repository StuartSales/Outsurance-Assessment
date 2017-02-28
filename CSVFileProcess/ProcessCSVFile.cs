using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVFileProcess
{
    public class ProcessCSVFile
    {

        #region Variables

        /// <summary>Import data information.</summary>
        class ImportData
        {

            /// <summary>First Name.</summary>
            public string FirstName;

            /// <summary>Last Name.</summary>
            public string LastName;

            /// <summary>Address.</summary>
            public string Address;

        }

        #endregion

        #region Public Members

        /// <summary>Process the import CSV file and export two results to two output files.</summary>
        /// <param name="ImportCSVFileName">CSV file to be processed.</param>
        /// <param name="NameFrequencyFileName">Output file name - name frequency.</param>
        /// <param name="AddressesFileName">Output file name - addresses.</param>
        public void ProcessFile(string ImportCSVFileName, string NameFrequencyFileName, string AddressesFileName)
        {
            List<ImportData> importDataSet = null;
            // Import the CSV file data
            importDataSet = ImportCSVFileImformation(ImportCSVFileName);
            if (importDataSet.Any())
            {
                // Generate the "Name Frequency" export
                GenerateNameFrequencyExport(NameFrequencyFileName, importDataSet);
                // Generate the "Address" export
                GenerateAddressExport(AddressesFileName, importDataSet);
            }
        }

        #endregion

        #region Private Members

        /// <summary>Import the CSV file information into a class structure.</summary>
        /// <param name="fileName">CSV file name to be imported.</param>
        /// <returns>Returns the imported CSV file data.</returns>
        List<ImportData> ImportCSVFileImformation(string fileName)
        {
            List<ImportData> result = null;
            if (File.Exists(fileName))
            {
                result = new List<ImportData>();
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    ImportData importData;
                    while (!streamReader.EndOfStream)
                    {
                        importData = ProcessImportDataLine(streamReader.ReadLine());
                        if (importData != null)
                        {
                            result.Add(importData);
                        }
                    }
                }
            }
            else
            {
                throw new FileNotFoundException(string.Format("CSV import file '{0}' was nout found!" , fileName));
            }
            return result;
        }

        /// <summary>Process the import data line.</summary>
        /// <param name="dataLine">Data line to be processed.</param>
        /// <returns>Returns the imported data information.</returns>
        ImportData ProcessImportDataLine(string dataLine)
        {
            if (string.IsNullOrEmpty(dataLine)) return null;
            string[] importData = dataLine.Split(',');
            // Validate the data length and starting value
            if ((importData.Length != 3) || (importData[0].ToUpper() == "FIRSTNAME")) return null;
            // Build the resonse data
            var result = new ImportData
            {
                FirstName = importData[0],
                LastName = importData[1],
                Address = importData[2],
            };
            return result;
        }

        /// <summary>Generate the "Name Frequency" export file.</summary>
        /// <param name="exportFileName">Export file name.</param>
        /// <param name="importDataSet">Import data set.</param>
        void GenerateNameFrequencyExport(string exportFileName, List<ImportData> importDataSet)
        {
            var exportList = new List<string>();
            // Get the name frequency
            var nameFrequencySet = new Dictionary<string, int>();
            foreach (ImportData importData in importDataSet)
            {
                CheckNameFrequency(nameFrequencySet, importData.FirstName);
                CheckNameFrequency(nameFrequencySet, importData.LastName);
            }
            // Order the names
            if (nameFrequencySet.Any())
            {
                // Frequency Group
                List<int> frequencySet = nameFrequencySet.OrderByDescending(n => n.Value).Select(n => n.Value).Distinct().ToList();
                List<string> nameSet;
                foreach (int frequency in frequencySet)
                {
                    // Name Group
                    nameSet = nameFrequencySet.Where(n => n.Value == frequency).OrderBy(n => n.Key).Select(n => n.Key).ToList();
                    foreach (string name in nameSet)
                    {
                        exportList.Add(string.Format("{0}, {1}", name, frequency));
                    }
                }
            }
            // Export the ordered names
            if (exportList.Any())
            {
                GenerateExportFile(exportFileName, exportList);
            }
        }

        /// <summary>Checks the name frequency.</summary>
        /// <param name="frequencySet">Name frequency dictionary.</param>
        /// <param name="name">Name to be checked.</param>
        void CheckNameFrequency(Dictionary<string, int> frequencySet, string name)
        {
            if (frequencySet.ContainsKey(name))
            {
                frequencySet[name] += 1;
            }
            else
            {
                frequencySet.Add(name, 1);
            }
        }

        /// <summary>Generated the "Address" export file.</summary>
        /// <param name="exportFileName">Export file name.</param>
        /// <param name="importDataSet">Imported data set.</param>
        void GenerateAddressExport(string exportFileName, List<ImportData> importDataSet)
        {
            // Get the Adress List
            List<string> addressSet = importDataSet.Select(i => i.Address).ToList();
            // Generate sorting address values
            var addressSortSet = new Dictionary<string, string>();
            foreach (string address in addressSet)
            {
                GenerateSortingAdress(addressSortSet, address);
            }
            // Order the Address
            List<string> exportData = addressSortSet.OrderBy(a => a.Value).Select(a => a.Key).ToList();
            // Generate the Export File
            GenerateExportFile(exportFileName, exportData);
        }

        /// <summary>Generated the sorting address value for an address.</summary>
        /// <param name="addressSortSet">Adress sort dictionary.</param>
        /// <param name="address">Address value.</param>
        void GenerateSortingAdress(Dictionary<string, string> addressSortSet, string address)
        {
            if (string.IsNullOrEmpty(address)) return;
            if (addressSortSet.ContainsKey(address)) return;
            // Slip the address
            string[] splitData = address.Split(' ');
            if (!splitData.Any()) return;
            // Validate if the first "part" is the address number
            string sortingAddress;
            int intValue;
            if (int.TryParse(splitData[0], out intValue))
            {
                // ignore the first "part" in the sorting address.
                sortingAddress = address.Remove(0, splitData[0].Length + 1);
            }
            else
            {
                // Use the FULL address as the sorting address
                sortingAddress = address;
            }
            addressSortSet.Add(address, sortingAddress);
        }

        /// <summary>Generates the export file.</summary>
        /// <param name="exportFileName">Export file name to generate.</param>
        /// <param name="exportDataSet">Data set to be exported.</param>
        void GenerateExportFile(string exportFileName, List<string> exportDataSet)
        {
            using (var fileStream = new FileStream(exportFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (string exportData in exportDataSet)
                    {
                        streamWriter.WriteLine(exportData);
                    }
                }
            }
        }

        #endregion

    }
}