using System.Collections;
using System.IO;
using System.Management.Automation.Provider;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    internal class BlobBlockBinaryContentReader : IContentReader
    {
        private BinaryReader _reader;
        private MemoryStream _stream;

        public BlobBlockBinaryContentReader(IListBlobItem item)
        {
            _stream = new MemoryStream();

            var blob = (CloudBlockBlob) item;
            blob.DownloadToStream(_stream);
            _stream.Position = 0;

            _reader = new BinaryReader(_stream);
        }

        public void Dispose()
        {
            // note: this method never appears to be called by PowerShell
            try
            {
                _stream.Dispose();
                _reader.Dispose();
            }
            catch
            {
            }
            finally
            {
                _stream = null;
                _reader = null;
            }
        }

        public IList Read(long readCount)
        {
            if (null == _stream)
            {
                return null;
            }

            byte[] buffer = null;

            if (0 == readCount)
            {
                buffer = _stream.ToArray();
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
            try
            {
                _stream.Close();
                _reader.Close();
            }
            catch
            {
            }

            try
            {
                _stream.Dispose();
                _reader.Dispose();
            }
            catch
            {
            }
            finally
            {
                _stream = null;
                _reader = null;
            }
        }
    }
}