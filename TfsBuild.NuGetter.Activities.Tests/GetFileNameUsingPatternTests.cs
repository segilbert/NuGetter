﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ==============================================================================================
// http://tfsversioning.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities.Tests
{
    [TestClass]
    public class GetFileNameUsingPatternTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void GetFileNameUsingPatternTests_WhenSearchingForASingleFileShouldReturnFilePath()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            File.Copy("TestLib.nuspec", "FileSearch0.nuspec", true);

            var fileNameDefaultPattern = string.Format("{0}*.nuspec", "FileSearch0");

            var filePath = getFileNameUsingPattern.FindFile(fileNameDefaultPattern, "FileSearch0*.nuspec", TestContext.DeploymentDirectory);

            Assert.AreEqual(Path.Combine(TestContext.DeploymentDirectory, "FileSearch0.nuspec"), filePath);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        public void GetFileNameUsingPatternTests_WhenSearchingForASingleFileMissSearchButHitDefaultShouldReturnFilePath()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            File.Copy("TestLib.nuspec", "FileSearch0.nuspec",true);

            var fileNameDefaultPattern = string.Format("{0}*.nuspec", "FileSearch0");

            var filePath = getFileNameUsingPattern.FindFile(fileNameDefaultPattern, "FileSearch.0.1.0*.nuspec", TestContext.DeploymentDirectory);

            Assert.AreEqual(Path.Combine(TestContext.DeploymentDirectory, "FileSearch0.nuspec"), filePath);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameUsingPatternTests_WhenSearchingForASingleFileAndMultiplesResultShouldThrowException()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            File.Copy("TestLib.nuspec", "FileSearch1.test.nuspec",true);
            File.Copy("TestLib.nuspec", "FileSearch1.nuspec", true);

            var fileNameDefaultPattern = string.Format("{0}*.nuspec", "FileSearch");

            getFileNameUsingPattern.FindFile(fileNameDefaultPattern, "FileSearch1*.nuspec", TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\TestLib.nuspec")]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameUsingPatternTests_WhenSearchingForASingleFileOnBothChecksAndMultiplesResultShouldThrowException()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            File.Copy("TestLib.nuspec", "FileSearch1.test.nuspec", true);
            File.Copy("TestLib.nuspec", "FileSearch1.nuspec", true);

            var fileNameDefaultPattern = string.Format("{0}*.nuspec", "FileSearch1");

            getFileNameUsingPattern.FindFile(fileNameDefaultPattern, "FileSearch1*.nuspec", TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameUsingPatternTests_WhenSearchingForASingleFileAndNoneResultShouldThrowException()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            getFileNameUsingPattern.FindFile("NoWayThisWillResult.InASuccess", "NoWayThisWillResult.InASuccess", TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameUsingPatternTests_WhenPassingInEmptyValueForSearchPatternShouldThrowException()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            getFileNameUsingPattern.FindFile(string.Empty, string.Empty, TestContext.DeploymentDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFileNameUsingPatternTests_WhenPassingInEmptyValueForSearchFolderShouldThrowException()
        {
            var getFileNameUsingPattern = new GetFileNameUsingPattern();

            getFileNameUsingPattern.FindFile("file*.pattern", "file*.pattern", string.Empty);
        }


    }
}
