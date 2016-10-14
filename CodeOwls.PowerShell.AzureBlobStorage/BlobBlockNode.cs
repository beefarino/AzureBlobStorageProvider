using System;
using System.Management.Automation;
using System.Management.Automation.Provider;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobBlockNode : BlobNode
    {
        private static string CachedItemMode;

        public BlobBlockNode(IListBlobItem blobItem) : base(blobItem)
        {
        }

        public override IContentReader GetContentReader(IProviderContext providerContext)
        {
            var p = providerContext.DynamicParameters as ContentReaderDynamicParameters;
            if (null == p)
            {
                p = new ContentReaderDynamicParameters();
            }

            if (p.AsText || p.Raw || null != p.Delimiter)
            {
                p.AsText = new SwitchParameter(true);
                p.Delimiter = p.Delimiter ?? Environment.NewLine;

                return new BlobBlockTextContentReader(BlobItem, p);
            }

            return new BlobBlockBinaryContentReader(BlobItem);
        }
    }
}