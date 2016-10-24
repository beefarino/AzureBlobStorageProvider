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

        public BlobBlockTextContentReader(IListBlobItem item,
            ContentReaderDynamicParameters contentReaderDynamicParameters)
        {
            _contentReaderDynamicParameters = contentReaderDynamicParameters;

            var blob = (CloudBlockBlob) item;
            _reader = new StreamReader(blob.OpenRead());
        }

        public void Dispose()
        {
            // note: this method never appears to be called by PowerShell
            Close();
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
                    string value = ReadDelimitedLine();
                    data.Add(value);
                }
            }

            return data;
        }

        private string ReadDelimitedLine()
        {
            
            if (_contentReaderDynamicParameters.Delimiter == Environment.NewLine)
            {
                return _reader.ReadLine();
            }

            StringBuilder builder = new StringBuilder();
            var delimiter = _contentReaderDynamicParameters.Delimiter;
            while (!builder.ToString().EndsWith(delimiter))
            {
                int read = _reader.Read();
                if (-1 == read)
                {
                    break;
                }
                var readChar = (char) read;
                builder.Append(readChar);
            }

            var value = builder.ToString();
            if (value.EndsWith(delimiter))
            {
                value = value.Substring(0, value.Length - delimiter.Length);
            }
            return value;
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