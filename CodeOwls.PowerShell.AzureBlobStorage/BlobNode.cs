using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public abstract class BlobNode : PathNodeBase
    {
        private readonly IListBlobItem _blobItem;
        private readonly string _name;
        private readonly bool _isDirectory;

        protected BlobNode( IListBlobItem blobItem )
        {
            _blobItem = blobItem;
            _isDirectory = false;
            if (_blobItem is CloudBlobDirectory)
            {
                _name = ((CloudBlobDirectory) _blobItem).Prefix.TrimEnd('/','\\');
                _isDirectory = true;
            }
            else if (_blobItem is CloudBlob)
            {
                _name = ((CloudBlob)blobItem).Name;
            }
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            CloudBlobDirectory directory = _blobItem as CloudBlobDirectory;
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
            return new PathValue( _blobItem, Name, _isDirectory );
        }

        public override string Name { get { return _name; } }
    }
}