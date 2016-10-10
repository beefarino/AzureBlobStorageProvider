using System;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeOwls.PowerShell.Paths.Extensions;
using CodeOwls.PowerShell.Provider;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobStorageDrive : Drive
    {
        public CloudStorageAccount Account { get; }
        public CloudBlobClient Client { get; }

        public BlobStorageDrive(PSDriveInfo driveInfo ) : base( new PSDriveInfo(driveInfo.Name, driveInfo.Provider, "[" + driveInfo.Root + "]", driveInfo.Description, driveInfo.Credential ) )
        {
            // example: DefaultEndpointsProtocol=[http|https];AccountName=myAccountName;AccountKey=myAccountKey
            Account = CloudStorageAccount.Parse(driveInfo.Root);
            Client = Account.CreateCloudBlobClient();
        }

        static public string ExtractDriveIdentifier(string path, out string adjustedPath)
        {
            adjustedPath = path;
            var re = new Regex(@"\[([^]]+)\]");
            var match = re.Match(path);
            if (! match.Success)
            {
                return null;
            }

            adjustedPath = re.Replace(path, String.Empty);

            return match.Groups[1].Value;
        }
    }
}
