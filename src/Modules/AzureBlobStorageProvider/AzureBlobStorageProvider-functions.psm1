<#
  Copyright (c) 2016 Code Owls LLC, All Rights Reserved.
#>

function copy-azureBlobItem {
  param(
      [parameter(position=0, mandatory=$true, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true)]
      [alias("path", "Azure BlobFilePath")]
      [string]
      # the path to the item; this must point to an Azure Blob provider path
      $pspath,

      [parameter(position=1, mandatory=$true, ValueFromPipelineByPropertyName=$true)]
      [alias("filepath")]
      [string]
      # the path to the local file; this must point to a Filesystem provider path
      $localFilePath,

      [parameter( mandatory=$false)]
      [switch]
      # overwrites the local file if it already exists
      $force
  )

  process {
    $d = '';
    [system.management.automation.providerinfo] $providerInfo = $null;
    [system.management.automation.psdriveinfo] $driveInfo = $null;


    $pathInfo = $ExecutionContext.SessionState.Path;
    $isAbsFilePath = $pathInfo.IsPSAbsolute($localFilePath, [ref]$d);
    if( -not $isAbsFilePath ) {
      $d = $pathInfo.CurrentFileSystemLocation.Drive.Name;
      $localFilePath = $pathInfo.GetUnresolvedProviderPathFromPSPath(
          "${d}:" + $localFilePath
      )
    }

    $pathInfo.GetUnresolvedProviderPathFromPSPath(
      $pspath,
      [ref]$providerInfo,
      [ref]$driveInfo
    ) | out-null;

    if( $providerInfo.Name -notmatch 'blob' ) {
      write-error -message "the -pspath parameter must point to a Azure Blob provider location" -targetObject $pspath
      return;
    }

    $pathInfo.GetUnresolvedProviderPathFromPSPath(
      $localFilePath,
      [ref]$providerInfo,
      [ref]$driveInfo
    ) | out-null;

    if( $providerInfo.Name -notmatch 'filesystem' ) {
      write-error -message "the -localFilePath parameter must point to a file system location" -targetObject $localFilePath
      return;
    }

    $bytes = get-content -literalpath $pspath;
    if( (Test-Path $localFilePath) -and (-not $force) ) {
        write-error -message "local file $localFilePath exists, and -force was not specified" -targetObject $localFilePath
        return;
    }

    [system.io.file]::writeAllBytes( $localFilePath, $bytes );

    get-item $localFilePath;
  }

<#
.SYNOPSIS
Copies one or more files from the mounted Azure Blob account to the local
file system.

.DESCRIPTION
Copies one or more files from the mounted Azure Blob account to the local
file system.  The PSPath parameter specifies the path to the Azure Blob
item, and the LocalFilePath parameter identifies where to save the file
locally.

.INPUTS
The Azure Blob item to copy, either as a provider object with a PSPath property,
or a String containing the PSPath to the Azure Blob object.

.OUTPUTS
The local file system object copied from Azure Blob.

.EXAMPLE
C:\PS> copy-AzureBlobItem dp:/transcripts/audit.txt -localFilePath ./audit.txt

This example copies the audit.txt file from the transcripts hive on Azure Blob
to a file named audit.txt in the current file system provider location.

.EXAMPLE
DP:\transcripts> dir | copy-AzureBlobItem -localFilePath {$_.name} -force

This example copies all items from the current location in the Azure Blob
provider to the local file system.  The force parameter is specified, so the
command will overwrite any existing files.

.LINK
about_AzureBlobProvider

.LINK
about_AzureBlobProvider_Version
#>
}
