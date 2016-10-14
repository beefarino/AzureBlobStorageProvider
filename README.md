# AzureBlobStorageProvider
A PowerShell Provider for Azure Blob Storage

# Usage Information

At present you'll need to load the DLLs for this provider manually into your PowerShell session.  A container module
will be added to the project shortly.

## Azure Storage Connection Strings

To mount an Azure Blob Storage account as a drive, you'll need a connection string for the account.  This connection string
needs to be passed as the -Root parameter for the new-psdrive cmdlet.

The format of these connection strings is documented [here](https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/).

## Provider Hierarchy

```
Drive Root (represents an Azure Storage Account)
|───BlobContainer
│   │
│   └───BlobDirectory
│       │   BlobBlock
│       │   BlobBlock
│       │   ...
```

## Currently Supported Operations

* get-item
* get-childitem
* get-content

### Get-Content Dynamic Parameters

The get-content cmdlet supports the following custom parameters:
* -AsText: when specified, the content is returned as text rather than bytes
* -Raw: when specified, text is returned as a single string; implies -AsText when specified
* -Delimiter <string>: The end-of-line delimiter to use; implies -AsText when specified; defaults to Environment.Newline

#### Examples

```PowerShell
# fetch the content in mydatafile as individual bytes
get-content ./mydatafile

# fetch the content in mydatafile as text
get-content ./mydatafile -asText

# fetch the content in mydatafile as a single string
get-content ./mydatafile -Raw

# fetch the content in mydatafile as a semicolon delimited string
get-content ./mydatafile -delimiter ";"
```

# Quick Start

Below is a session transcript that demonstrates the provider's basic usage.

```PowerShell
PS C:\> ls *.dll | import-module; #import the necessary provider DLLs
PS C:\> new-psdrive -psp blob -name blob -root 'DefaultEndpointsProtocol=https;AccountName=YourAccountName;AccountKey=YourAccessKey'
PS C:\> cd blob:
PS blob:\> dir

SSItemMode    : d  <
PSPath        : ...
PSParentPath  : ...
PSChildName   : dataout
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

SSItemMode    : d  <
PSPath        : ...
PSParentPath  : ...
PSChildName   : hadorp
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

PS blob:\> cd dataout
PS blob:\dataout> dir

SSItemMode    : d  <
PSPath        : ...
PSParentPath  : ...
PSChildName   : documentwordcounts.txt
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

SSItemMode    : d  <
PSPath        : ...
PSParentPath  : ...
PSChildName   : totalwordcounts.txt
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

SSItemMode                   :    <
PSPath                       : ...
PSParentPath                 : ...
PSChildName                  : documentwordcounts.txt
PSDrive                      : blob
PSProvider                   : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer                : False
StreamWriteSizeInBytes       : 4194304
...

SSItemMode                   :    <
PSPath                       : ...
PSParentPath                 : ...
PSChildName                  : totalwordcounts.txt
PSDrive                      : blob
PSProvider                   : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer                : False
StreamWriteSizeInBytes       : 4194304
ServiceClient                : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
StreamMinimumReadSizeInBytes : 4194304
...

PS blob:\dataout> cd totalwordcounts.txt
PS blob:\dataout\totalwordcounts.txt> dir

SSItemMode    : d  <
PSPath        : ...
PSParentPath  : ...
PSChildName   : .hive-staging_hive_2016-09-12_03-11-56_447_8455690809408588477-1
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

SSItemMode                   :    <
PSPath                       : ...
PSParentPath                 : ...
PSChildName                  : .hive-staging_hive_2016-09-12_03-11-56_447_8455690809408588477-1
PSDrive                      : blob
PSProvider                   : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer                : False
StreamWriteSizeInBytes       : 4194304
...

SSItemMode    :    <
PSPath        : ...
PSParentPath  : ...
PSChildName   : .hive-staging_hive_2016-09-12_03-11-56_447_8455690809408588477-1
PSDrive       : blob
PSProvider    : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer : True
ServiceClient : Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient
...

SSItemMode                   :    <
PSPath                       : ...
PSParentPath                 : ...
PSChildName                  : 000000_0
PSDrive                      : blob
PSProvider                   : CodeOwls.PowerShell.AzureBlobStorage\Blob
PSIsContainer                : False
StreamWriteSizeInBytes       : 4194304
...

PS blob:\dataout\totalwordcounts.txt> $data = get-content ./000000_0 -astext
PS blob:\dataout\totalwordcounts.txt> $data
a,42424,1781
a-star,1,1
a-z,2,2
aa,14,5
aad,3,2
aaron,9,9
abap,168,9
...
```
