using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Testify
{

    public class TestSaver
    {

        private readonly string _directory;
        private string _testName;
        private readonly Test _test;

        public TestSaver(string directory)
        {
            var frame = new StackFrame(1);
            SetTestName(frame.GetMethod());
            _directory = directory;
            _test = new Test
            {
                Name = _testName,
                Results = new List<TestResult>()
            };
        }

        private void SetTestName(MemberInfo method)
        {
            try
            {
                _testName = method.DeclaringType.Name;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("[Testify] Could not determine test name.. Setting default..");
                _testName = "TESTIFY" + DateTime.Now;
            }
        }

        public void Put(string expected, string actual)
        {
            var stackTrace = new StackTrace();

            var result = new TestResult
            {
                Name = stackTrace.GetFrame(1).GetMethod().Name,
                Expected = expected,
                Actual = actual,
                Result = expected == actual ? "Success" : "Fail"
            };

            _test.Results.Add(result);
        }

        public void Put(int expected, int actual)
        {
            var stackTrace = new StackTrace();

            var result = new TestResult
            {
                Name = stackTrace.GetFrame(1).GetMethod().Name,
                Expected = expected.ToString(),
                Actual = actual.ToString(),
                Result = expected == actual ? "Success" : "Fail"
            };

            _test.Results.Add(result);
        }

        public void Save()
        {
            VerifyDirectory();
            var stringify = JsonConvert.SerializeObject(_test);
            
            if(_directory.EndsWith("/"))
                File.WriteAllText(_directory + _testName + ".json", stringify);
            else
                File.WriteAllText(_directory + "/" + _testName + ".json", stringify);
        }

        private void VerifyDirectory()
        {
            if (Directory.Exists(_directory)) return;

            Console.WriteLine("[Testify] Directory does not exist. Creating..");
            Directory.CreateDirectory(_directory);
        }
    }

}
