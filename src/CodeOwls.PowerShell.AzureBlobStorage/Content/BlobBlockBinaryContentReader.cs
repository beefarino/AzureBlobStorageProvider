using System.Collections;
using System.IO;
using System.Management.Automation.Provider;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    internal class BlobBlockBinaryContentReader : IContentReader
    {
        private BinaryReader _reader;

        public BlobBlockBinaryContentReader(IListBlobItem item)
        {
            var blob = (CloudBlockBlob) item;
            
            _reader = new BinaryReader(blob.OpenRead());
        }

        public void Dispose()
        {
            // note: this method never appears to be called by PowerShell
            Close();
        }

        public IList Read(long readCount)
        {
            byte[] buffer = null;

            if (0 == readCount)
            {
                buffer = _reader.ReadBytes( (int) _reader.BaseStream.Length );
            }
            else
            {
                buffer = _reader.ReadBytes((int) readCount);
            }

            var data = new ArrayList(buffer);
            return data;
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _reader.BaseStream.Seek(offset, origin);
        }

        public void Close()
        {
            if (null == _reader)
            {
                return;
            }

            try
            {
                _reader.Close();
            }
            catch
            {
            }

            try
            {
                _reader.Dispose();
            }
            catch
            {
            }
            finally
            {
                _reader = null;
            }
        }
    }
}