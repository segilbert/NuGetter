using System.Activities;
using System.Activities.Expressions;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsBuild.NuGetter.Activities.Tests.TestData;

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
    public class XmlGetPackageElementsTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenValuesAreValidShouldGetXmlValuesProperly()
        {
            var nuGetterElements = new XmlGetPackageElements();

            Assert.IsNotNull(nuGetterElements);
        }


        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenValuesAreValidUsingWorkflowIndex0ShouldExtractXmlValueProperly()
        {
            var packageInfoFilename = "NuGetMultiProjectPkgInfo.xml";

            var xdoc = XDocument.Load(packageInfoFilename);

            var nuGetterPackageInfoList = (from xml in xdoc.Elements("NuGetterPackages").Elements("NuGetterPackage")
                                           let name = xml.Attribute("name")
                                           let addnlOptionsElement = xml.Element("AdditionalOptions")
                                           let basePathElement = xml.Element("BasePath")
                                           let nuspecFilePath = xml.Element("NuSpecFilePath")
                                           let powerShellScriptPath = xml.Element("PowerShellScriptPath")
                                           let version = xml.Element("Version")

                                           let outputDirectory = xml.Element("OutputDirectory")
                                           let switchInvokePush = xml.Element("SwitchInvokePush")
                                           let pushDestination = xml.Element("PushDestination")
                                           let switchInvokePowerShell = xml.Element("InvokePowerShell")


                                           select new NuGetterPackageInfo
                                           {
                                               Name = name == null ? string.Empty : name.Value,
                                               AdditionalOptions = addnlOptionsElement == null ? null : addnlOptionsElement.Value,
                                               BasePath = basePathElement == null ? null : basePathElement.Value,
                                               NuSpecFilePath = nuspecFilePath == null ? null : nuspecFilePath.Value,
                                               PowerShellScriptPath = powerShellScriptPath == null ? null : powerShellScriptPath.Value,
                                               Version = version == null ? null : version.Value,

                                               OutputDirectory = outputDirectory == null ? null : outputDirectory.Value,
                                               SwitchInvokePush = switchInvokePush == null ? false : bool.Parse(switchInvokePush.Value),
                                               PushDestination = pushDestination == null ? null : pushDestination.Value,
                                               SwitchInvokePowerShell = switchInvokePowerShell == null ? false : bool.Parse(switchInvokePowerShell.Value),

                                           }).ToList();




            for (var i = 0; i < nuGetterPackageInfoList.Count; i++)
            {
                // Create an instance of our test workflow
                var workflow = new InvokeXmlGetPackageElements();

                // Create the workflow run-time environment
                var workflowInvoker = new WorkflowInvoker(workflow);

                workflow.PackageIndex = i;

                // Set the workflow arguments
                workflow.PackageInfoFilePath = packageInfoFilename;
                workflow.OutputDirectoryFromBldDef = "BldDefOutputDirectory";
                workflow.PushDestinationFromBldDef = "BldDefPushDestination";
                workflow.SwitchInvokePowerShellFromBldDef = true;
                workflow.SwitchInvokePushFromBldDef = true;
                workflow.VersionFromBldDef = "BldDefVersion";
                workflow.AdditionalOptionsFromBldDef = "BldDefAdditionalOptions";
                workflow.BasePathFromBldDef = "BldDefBasePath";
                workflow.PushDestinationFromBldDef = "BldDefPushDestination";
                workflow.PowerShellScriptPathFromBldDef = "BldDefPowerShellScriptPath";
                // Invoke the workflow and capture the outputs
                var output = workflowInvoker.Invoke();

                var nuSpecFilePathOut = output["NuSpecFilePath"];

                string expectedAdditionalOptions;
                string expectedBasePath;

                bool expectedInvokePowerShell;
                bool expectedInvokePush;
                string expectedOutputDirectory;
                string expectedPowerShellScriptPath;
                string expectedPushDestination;
                string expectedVersion;



                if (i > 0)
                {
                    expectedAdditionalOptions = "AdditionalOptions" + i;
                }
                else
                {
                    expectedAdditionalOptions = "BldDefAdditionalOptions";
                }

                if (i > 1)
                {
                    expectedBasePath = "BasePath" + i;
                }
                else
                {
                    expectedBasePath = "BldDefBasePath";
                }

                expectedInvokePowerShell = i <= 3;
                expectedInvokePush = i <= 4 || i == 9;


                
                if (i > 5)
                {
                    expectedOutputDirectory = "OutputDirectory" + i;
                }
                else
                {
                    expectedOutputDirectory = "BldDefOutputDirectory";
                }
                
                if (i > 6)
                {
                    expectedPowerShellScriptPath = "PowerShellScriptPath" + i;
                }
                else
                {
                    expectedPowerShellScriptPath = "BldDefPowerShellScriptPath";
                }

                if (i > 7)
                {
                    expectedPushDestination = "PushDestination" + i;
                }
                else
                {
                    expectedPushDestination = "BldDefPushDestination";
                }
                
                if (i > 8)
                {
                    expectedVersion = "Version" + i;
                }
                else
                {
                    expectedVersion = "BldDefVersion";
                }

                Assert.AreEqual(expectedAdditionalOptions, output["AdditionalOptions"].ToString());
                Assert.AreEqual(expectedBasePath, output["BasePath"].ToString());
                Assert.AreEqual(expectedInvokePowerShell, output["SwitchInvokePowerShell"]);
                Assert.AreEqual(expectedInvokePush, output["SwitchInvokePush"]);
                Assert.AreEqual(expectedOutputDirectory, output["OutputDirectory"].ToString());
                Assert.AreEqual(expectedPowerShellScriptPath, output["PowerShellScriptPath"].ToString());
                Assert.AreEqual(expectedPushDestination, output["PushDestination"].ToString());
                Assert.AreEqual(expectedVersion, output["Version"].ToString());
                Assert.AreEqual("NuSpecFilePath" + i, nuSpecFilePathOut.ToString());
                
            }

        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenCallingTest()
        {
            //var nuGetterElements = new XmlGetPackageElements();

            //// get the value of the FilePath
            //var packageInfoFilePath = context.GetValue(PackageInfoFilePath);
            //var packageIndex = context.GetValue(PackageIndex);

            //var outputDirectoryFromBldDef = context.GetValue(OutputDirectoryFromBldDef);
            //var switchInvokePushFromBldDef = context.GetValue(SwitchInvokePushFromBldDef);
            //var pushDestinationFromBldDef = context.GetValue(PushDestinationFromBldDef);
            //var switchInvokePowerShellFromBldDef = context.GetValue(SwitchInvokePowerShellFromBldDef);
            //var versionFromBldDef = context.GetValue(VersionFromBldDef);

            //var basePathFromBldDef = context.GetValue(BasePathFromBldDef);
            //var additionalOptionsFromBldDef = context.GetValue(AdditionalOptionsFromBldDef);
            //var powerShellScriptPathFromBldDef = context.GetValue(PowerShellScriptPathFromBldDef);

            //var nuGetterPackageInfoResult = nuGetterElements.Execute(packageInfoFilePath, packageIndex, basePathFromBldDef, additionalOptionsFromBldDef,
            //    outputDirectoryFromBldDef, switchInvokePushFromBldDef,
            //    pushDestinationFromBldDef, switchInvokePowerShellFromBldDef, powerShellScriptPathFromBldDef, versionFromBldDef);

            // Assert
            
        }

        [TestMethod]
        [DeploymentItem("TfsBuild.NuGetter.Activities.Tests\\TestData\\NuGetMultiProjectPkgInfo.xml")]
        public void XmlGetPackageElementsTests_WhenCallingElementsCountShouldReturnNumberOfNuGetterPackageElements()
        {
            var nuGetterElements = new XmlGetPackageElementsCount();

            var count = nuGetterElements.Execute("NuGetMultiProjectPkgInfo.xml");

            Assert.AreEqual(10, count);
        }

    }

}
