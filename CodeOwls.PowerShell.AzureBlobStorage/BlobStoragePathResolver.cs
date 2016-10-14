using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobStoragePathResolver : PathResolverBase
    {
        private CloudBlobClient _client;

        protected override IPathNode Root
        {
            get { return new BlobStorageRootNode(_client); }
        }

        public override IEnumerable<IPathNode> ResolvePath(IProviderContext providerContext, string path)
        {
            string adjustedPath;
            var id = BlobStorageDrive.ExtractDriveIdentifier(path, out adjustedPath);
            var drive = providerContext.Drive as BlobStorageDrive;

            if (null == drive)
            {
                var drives = providerContext.SessionState.Drive.GetAll().OfType<BlobStorageDrive>();

                if (null != id)
                {
                    var rootId = "[" + id.Replace("\\", "/") + "]";
                    drive = drives.First(m => m.Root == rootId);
                }

                if (null == drive)
                {
                    drive = drives.First();
                }
            }

            _client = drive.Client;

            return base.ResolvePath(providerContext, adjustedPath);
        }
    }
}