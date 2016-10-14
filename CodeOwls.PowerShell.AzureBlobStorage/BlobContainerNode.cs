using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobContainerNode : PathNodeBase
    {
        private readonly CloudBlobContainer _container;

        public BlobContainerNode(CloudBlobContainer container)
        {
            _container = container;
        }

        public override string Name => _container.Name;

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            var blobs = _container.ListBlobs();
            var pathNodes = new List<IPathNode>();

            var dirPathNodes = blobs.OfType<CloudBlobDirectory>().ToList().ConvertAll(a => new BlobDirectoryNode(a));
            var blockPathNodes = blobs.OfType<CloudBlockBlob>().ToList().ConvertAll(a => new BlobBlockNode(a));

            pathNodes.AddRange(dirPathNodes);
            pathNodes.AddRange(blockPathNodes);

            return pathNodes;
        }

        public override IPathValue GetNodeValue()
        {
            return new ContainerPathValue(_container, Name);
        }
    }
}