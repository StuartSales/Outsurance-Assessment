using CSVFileProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace AssessmentUnitTest
{
    [TestClass]
    public class Assessment_UnitTest
    {

        #region Variables

        readonly string FILE_NAME__IMPORT = "ImportFile.CSV";
        readonly string FILE_NAME__EXPORT__NAME_FREQUENCY = "Export_NameFrequency.txt";
        readonly string FILE_NAME__EXPORT__ADDRESS = "Export_Addresses.txt";

        /// <summary>Assert result.</summary>
        class AssertResult
        {

            #region Constructor

            public AssertResult()
            {
                Condition = true;
                Message = string.Empty;
            }

            #endregion

            #region Public Members

            public bool Condition;

            public string Message;

            #endregion

        }

        #endregion

        #region Test Members

        /// <summary>Generate the export files for the unit tests.</summary>
        [TestInitialize]
        public void TestInitialize()
        {
            var processCSVFile = new ProcessCSVFile();
            processCSVFile.ProcessFile(FILE_NAME__IMPORT, FILE_NAME__EXPORT__NAME_FREQUENCY, FILE_NAME__EXPORT__ADDRESS);
            processCSVFile = null;
        }

        /// <summary>Unit test for the "Name Frequency" export.</summary>
        [TestMethod]
        public void TestMethod_NameFrequency()
        {
            List<string> expectedDataSet = GetExpectedData_NameFrequency();
            AssertResult assertResult = ValidateExportAndExpectedData(FILE_NAME__EXPORT__NAME_FREQUENCY, expectedDataSet);
            Assert.IsTrue(assertResult.Condition, assertResult.Message);
        }

        /// <summary>Unit test for the "Address" export.</summary>
        [TestMethod]
        public void TestMethod_Address()
        {
            List<string> expectedDataSet = GetExpectedData_Address();
            AssertResult assertResult = ValidateExportAndExpectedData(FILE_NAME__EXPORT__ADDRESS, expectedDataSet);
            Assert.IsTrue(assertResult.Condition, assertResult.Message);
        }

        #endregion

        #region Private Members

        /// <summary>Gets the expected "Name Frequency" results based on the "ImportFile.CSV" file data.</summary>
        /// <returns>Returns the expected "Name Frequency" results based on the "ImportFile.CSV" file data.</returns>
        List<string> GetExpectedData_NameFrequency()
        {
            var result = new List<string>
            {
                "Johnson, 2",
                "Brown, 1",
                "Heinrich, 1",
                "Jones, 1",
                "Matt, 1",
                "Smith, 1",
                "Tim, 1",
            };
            return result;
        }

        /// <summary>Gets the expected "Address" results based on the "ImporetFile.CSV" file data.</summary>
        /// <returns>Returns the expected "Address" results based on the "ImporetFile.CSV" file data.</returns>
        List<string> GetExpectedData_Address()
        {
            var result = new List<string>
            {
                "12 Acton St",
                "95 Berger St",
                "31 Clifton Rd",
                "22 Jones Rd",
            };
            return result;
        }

        /// <summary>Validates that the exported data matches up with the expected data.</summary>
        /// <param name="fileName">File name of exported data.</param>
        /// <param name="expectedDataSet">Expected data list.</param>
        /// <returns>Returns the validation assert result.</returns>
        AssertResult ValidateExportAndExpectedData(string fileName, List<string> expectedDataSet)
        {
            var result = new AssertResult();
            if (File.Exists(fileName))
            {
                var exportedDataSet = new List<string>();
                // Load the Export Results into a list
                using (StreamReader streamReader = new StreamReader(fileName))
                {
                    while (!streamReader.EndOfStream)
                    {
                        exportedDataSet.Add(streamReader.ReadLine());
                    }
                }
                // Validate the lengths
                if (exportedDataSet.Count == expectedDataSet.Count)
                {
                    // Validate the export entries one record at a time
                    for (int i = 0; i < expectedDataSet.Count; i++)
                    {
                        if (exportedDataSet[i] != expectedDataSet[i])
                        {
                            result.Condition = false;
                            result.Message = string.Format("Entry '{0}' is invalid. '{1}' was exported and '{2}' was expected!"
                                , i, exportedDataSet[i], expectedDataSet[i]);
                            break;
                        }
                    }
                }
                else
                {
                    result.Condition = false;
                    result.Message = string.Format("Number of exported records are invalid, '{0}' records exported and '{1}' was expected!"
                        , exportedDataSet.Count, expectedDataSet.Count);
                }
            }
            else
            {
                result.Condition = false;
                result.Message = string.Format("Export file '{0}' is not found!", fileName);
            }
            return result;
        }

        #endregion

    }
}