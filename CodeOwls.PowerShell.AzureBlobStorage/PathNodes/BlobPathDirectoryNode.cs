using System.Management.Automation;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobPathDirectoryNode : BlobPathNode
    {
        public BlobPathDirectoryNode(IListBlobItem blobItem) : base(blobItem)
        {
        }
    }
}