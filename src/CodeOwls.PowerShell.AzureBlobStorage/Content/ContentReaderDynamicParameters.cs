using System;
using System.Management.Automation;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class ContentReaderDynamicParameters
    {
        [Parameter(Mandatory = false, HelpMessage = "When specified, download the contents as text rather than bytes")]
        [Alias("Text")]
        public SwitchParameter AsText { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "When specified, text is returned as a single string; implies -AsText when specified.")]
        public SwitchParameter Raw { get; set; }

        [Parameter(Mandatory = false,
            HelpMessage =
                "The end-of-line delimiter to use; implies -AsText when specified; defaults to Environment.Newline"
            )]
        public string Delimiter { get; set; }


    }
}