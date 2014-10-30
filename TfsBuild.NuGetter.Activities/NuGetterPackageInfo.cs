namespace TfsBuild.NuGetter.Activities
{
    public class NuGetterPackageInfo
    {
        public string Name { get; set; }
        public string AdditionalOptions { get; set; }
        public string BasePath { get; set; }
        public string NuSpecFilePath { get; set; }
        public string PowerShellScriptPath { get; set; }
        public string Version { get; set; }
        public string OutputDirectory { get; set; }
        public bool SwitchInvokePush { get; set; }
        public string PushDestination { get; set; }
        public bool SwitchInvokePowerShell { get; set; }  
    }
}