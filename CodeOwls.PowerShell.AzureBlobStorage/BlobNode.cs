using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Provider;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Paths;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public abstract class BlobNode : PathNodeBase, IGetItemContent
    {
        private readonly bool _isDirectory;
        private readonly string _name;
        protected readonly IListBlobItem BlobItem;

        protected BlobNode(IListBlobItem blobItem)
        {
            BlobItem = blobItem;
            _isDirectory = false;
            if (BlobItem is CloudBlobDirectory)
            {
                _name = ((CloudBlobDirectory) BlobItem).Prefix.TrimEnd('/', '\\');
                _isDirectory = true;
            }
            else if (BlobItem is CloudBlob)
            {
                _name = ((CloudBlob) blobItem).Name;
            }

            var re = new Regex(@".+\/");
            _name = re.Replace(_name, string.Empty);
        }

        public override string Name
        {
            get { return _name; }
        }

        public virtual IContentReader GetContentReader(IProviderContext providerContext)
        {
            throw new NotSupportedException();
        }

        public object GetContentReaderDynamicParameters(IProviderContext providerContext)
        {
            return new ContentReaderDynamicParameters();
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            var directory = BlobItem as CloudBlobDirectory;
            if (null == directory)
            {
                return null;
            }

            var blobs = directory.ListBlobs();
            var pathNodes = new List<IPathNode>();

            var dirPathNodes = blobs.OfType<CloudBlobDirectory>().ToList();
            var blockPathNodes = blobs.ToList();
            blockPathNodes.RemoveAll(m => dirPathNodes.Contains(m));

            pathNodes.AddRange(dirPathNodes.ConvertAll(a => new BlobDirectoryNode(a)));
            pathNodes.AddRange(blockPathNodes.ConvertAll(a => new BlobBlockNode(a)));

            return pathNodes;
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue(BlobItem, Name, _isDirectory);
        }
    }
}