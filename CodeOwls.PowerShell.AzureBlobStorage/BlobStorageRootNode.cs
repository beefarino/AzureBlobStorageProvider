using System;
using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    public class BlobStorageRootNode : PathNodeBase
    {
        private readonly CloudBlobClient _client;
        
        public BlobStorageRootNode(CloudBlobClient client)
        {
            _client = client;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            var containers = _client.ListContainers();

            var pathNodes = containers.ToList().ConvertAll(a => new BlobContainerNode(a));

            return pathNodes;
        }

        public override IPathValue GetNodeValue()
        {
            return new ContainerPathValue(_client, String.Empty );
        }

        public override string Name => String.Empty;
    }
}