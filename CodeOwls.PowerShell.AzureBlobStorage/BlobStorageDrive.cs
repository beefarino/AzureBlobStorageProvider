using System.Management.Automation;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobStorageDrive : Drive
    {
        public BlobStorageDrive(PSDriveInfo driveInfo)
            : base(
                new PSDriveInfo(driveInfo.Name, driveInfo.Provider, "[" + driveInfo.Root + "]", driveInfo.Description,
                    driveInfo.Credential))
        {
            // example: DefaultEndpointsProtocol=[http|https];AccountName=myAccountName;AccountKey=myAccountKey

            Account = CloudStorageAccount.Parse(driveInfo.Root);
            Client = Account.CreateCloudBlobClient();
        }

        public CloudStorageAccount Account { get; }
        public CloudBlobClient Client { get; }

        public static string ExtractDriveIdentifier(string path, out string adjustedPath)
        {
            adjustedPath = path;
            var re = new Regex(@"\[([^]]+)\]");
            var match = re.Match(path);
            if (!match.Success)
            {
                return null;
            }

            adjustedPath = re.Replace(path, string.Empty);

            return match.Groups[1].Value;
        }
    }
}