using System;
using System.Activities;
using System.Activities.Debugger;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://tfsversioning.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    /// <summary>
    /// Used to search the version "seed file" and return the value
    /// </summary>
    [ToolboxBitmap(typeof(GetFileNameUsingPattern), "Resources.version.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class GetFileNameUsingPattern : CodeActivity
    {
        #region Workflow Arguments

        [RequiredArgument]
        public InArgument<string> NuSpecFilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> PackageName { get; set; }

        /// <summary>
        /// Pattern to use for retrieving an actual filename
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FileNamePattern { get; set; }

        /// <summary>
        /// Pattern to use for retrieving an actual filename
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FileNameDefaultPattern { get; set; }

        /// <summary>
        /// Folder to search for the file using the pattern
        /// text file.
        /// </summary>
        [RequiredArgument]
        public InArgument<string> SearchFolder { get; set; }

        /// <summary>
        /// The full file name and path from a successful search
        /// </summary>
        public OutArgument<string> FullFilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> Version { get; set; }

        private string _packageName = string.Empty;
        private string _version = string.Empty;


        #endregion
        
        /// <summary>
        /// Searches an XML file with an XPath expression
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(CodeActivityContext context)
        {
            // get the value of the XPathExpression
            var fileNamePattern = FileNamePattern.Get(context);

            var fileNameDefaultPattern = FileNameDefaultPattern.Get(context);

            // get the value of the FilePath
            var searchFolder = SearchFolder.Get(context);
            
            // package name
            _packageName = PackageName.Get(context);
            _version = Version.Get(context);

            // nuspec file path
            string nuSpecFilePath = NuSpecFilePath.Get(context);
            
            // build name of nuget file from nuspec naming conventions
            // override fileNamePattern
            string pattern = string.Format("{0}.*", _packageName);
            var regex = new Regex(pattern,RegexOptions.IgnoreCase);
            fileNamePattern = regex.Match(nuSpecFilePath).Value.Replace(".nuspec", ".nupkg");
            
            FileNamePattern.Set(context, fileNamePattern);

            var filePaths = FindFile(fileNameDefaultPattern, fileNamePattern, searchFolder);
            FullFilePath.Set(context, filePaths);
        }

        public string FindFile(string fileNameDefaultPattern, string fileNamePattern, string searchFolder)
        {
            #region Parameter Validation

            if (String.IsNullOrWhiteSpace(fileNameDefaultPattern))
            {
                throw new ArgumentException("FileNameDefaultPattern must contain a search pattern");
            }

            // validate that there is a file pattern to work with
            if (String.IsNullOrWhiteSpace(fileNamePattern))
            {
                throw new ArgumentException("FileNamePattern must contain a search pattern");
            }

            // Validate that there is a search folder for the search
            if (String.IsNullOrWhiteSpace(searchFolder))
            {
                throw new ArgumentException("searchFolder");
            }

            #endregion

            var searchPattern = fileNamePattern;
            var fileList = Directory.EnumerateFiles(searchFolder, fileNamePattern).ToArray();

            if (fileList.Length != 1 )
            {
                fileList = Directory.EnumerateFiles(searchFolder, fileNameDefaultPattern).ToArray();
                searchPattern = fileNameDefaultPattern;
            }

            if (fileList.Length == 1)
            {
                return fileList[0];
            }

            var exMessage = fileList.Length > 1 ?
                string.Format("Search pattern '{0}' retrieved more than one file at: {1}", searchPattern, searchFolder) :
                string.Format("Search pattern '{0}' did not find any files at: {1}", searchPattern, searchFolder);

            throw new ArgumentException(exMessage);
        }
    }
}