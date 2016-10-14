using System;
using System.Collections;
using System.IO;
using System.Management.Automation.Provider;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CodeOwls.PowerShell.AzureBlobStorage
{
    internal class BlobBlockTextContentReader : IContentReader
    {
        private readonly ContentReaderDynamicParameters _contentReaderDynamicParameters;
        private StreamReader _reader;
        private CloudBlockBlob _blob;

        public BlobBlockTextContentReader(IListBlobItem item,
            ContentReaderDynamicParameters contentReaderDynamicParameters)
        {
            _contentReaderDynamicParameters = contentReaderDynamicParameters;

            _blob = (CloudBlockBlob) item;
            _reader = new StreamReader(_blob.OpenRead());
        }

        public void Dispose()
        {
            // note: this method never appears to be called by PowerShell
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

        public IList Read(long readCount)
        {
            if (null == _reader || _reader.EndOfStream)
            {
                return null;
            }

            var data = new ArrayList();

            if (_contentReaderDynamicParameters.Raw)
            {
                data.Add(_reader.ReadToEnd());
            }
            else if (0 == readCount)
            {
                data.AddRange(_reader.ReadToEnd()
                    .Split(new[] {_contentReaderDynamicParameters.Delimiter}, StringSplitOptions.None));
            }
            else
            {
                while (0 < readCount-- && ! _reader.EndOfStream)
                {                    
                    data.Add(_reader.ReadLine());
                }
            }

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