using System.Management.Automation;
using System.Management.Automation.Provider;
using CodeOwls.PowerShell.Paths.Processors;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    [CmdletProvider("Blob", ProviderCapabilities.Filter | ProviderCapabilities.ShouldProcess)]
    public class BlobStorageProvider : Provider.Provider
    {

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive is BlobStorageDrive) return drive;

            return new BlobStorageDrive( drive );
        }

        protected override IPathResolver PathResolver => new BlobStoragePathResolver();
    }
}