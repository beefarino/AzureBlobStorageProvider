using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobDirectoryNode : BlobNode
    {
        public BlobDirectoryNode(IListBlobItem blobItem) : base(blobItem)
        {
        }
    }
}