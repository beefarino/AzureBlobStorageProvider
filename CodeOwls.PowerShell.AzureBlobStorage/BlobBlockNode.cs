using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobBlockNode : BlobNode
    {
        public BlobBlockNode(IListBlobItem blobItem) : base(blobItem)
        {
        }
    }
}