# ReadGameAssembly.ps1 — read ground-truth bytes/constants from the ORIGINAL game binary.
#
# Purpose: ISIL dumps (CODES/ClientCode/CPP2IL_ISIL_Client/IsilDump/*.txt) reference
# constants by absolute virtual address, e.g. `movss xmm1, dword ptr [181EA337Ch]`.
# This script converts that VA to a file offset inside GameAssembly.dll (via PE section
# headers) and prints the raw bytes interpreted as float/double/int32/int64.
# NO GUESSING NEEDED — read the real value from the real build.
#
# Usage examples:
#   powershell -File Tools\ReadGameAssembly.ps1 -Address 0x181EA337C
#   powershell -File Tools\ReadGameAssembly.ps1 -Address 0x1821875BC -Count 16
#   powershell -File Tools\ReadGameAssembly.ps1 -Address 0x00387A90 -IsFileOffset   # method body bytes
#
# -Address accepts:
#   * absolute VA from ISIL   (0x18xxxxxxxx — image base 0x180000000 is subtracted automatically)
#   * RVA                     (values < 0x180000000 are treated as RVA)
#   * raw file offset         (with -IsFileOffset; e.g. "File Offset" from *_metadata.txt)

param(
    [Parameter(Mandatory = $true)] [string]$Address,
    [int]$Count = 8,
    [switch]$IsFileOffset,
    [string]$DllPath = "D:\AssetToBaseStart12.0.2\Client\GameAssembly.dll"
)

$addr = [Convert]::ToUInt64($Address, 16)
if (-not (Test-Path $DllPath)) { throw "GameAssembly.dll not found: $DllPath" }

$fs = [System.IO.File]::OpenRead($DllPath)
$br = New-Object System.IO.BinaryReader($fs)
try {
    if ($IsFileOffset) {
        $fileOff = [int64]$addr
        Write-Output ("File offset 0x{0:X}" -f $fileOff)
    }
    else {
        # VA -> RVA (cpp2il assumes image base 0x180000000)
        $imageBase = [uint64]0x180000000
        $rva = if ($addr -ge $imageBase) { $addr - $imageBase } else { $addr }

        # Walk PE section headers to map RVA -> file offset
        $fs.Seek(0x3C, 0) | Out-Null
        $peOff = $br.ReadInt32()
        $fs.Seek($peOff + 6, 0) | Out-Null
        $numSec = $br.ReadUInt16()
        $fs.Seek($peOff + 20, 0) | Out-Null
        $optSize = $br.ReadUInt16()
        $secStart = $peOff + 24 + $optSize

        $fileOff = -1
        for ($i = 0; $i -lt $numSec; $i++) {
            $fs.Seek($secStart + $i * 40, 0) | Out-Null
            $name  = [System.Text.Encoding]::ASCII.GetString($br.ReadBytes(8)).TrimEnd([char]0)
            $vsize = $br.ReadUInt32(); $vaddr = $br.ReadUInt32()
            $rsize = $br.ReadUInt32(); $raw   = $br.ReadUInt32()
            if ($rva -ge $vaddr -and $rva -lt ($vaddr + $vsize)) {
                $fileOff = $raw + ($rva - $vaddr)
                Write-Output ("VA 0x{0:X} -> RVA 0x{1:X} -> section '{2}' -> file offset 0x{3:X}" -f $addr, $rva, $name, $fileOff)
                break
            }
        }
        if ($fileOff -lt 0) { throw ("RVA 0x{0:X} not found in any PE section" -f $rva) }
    }

    $fs.Seek($fileOff, 0) | Out-Null
    $bytes = $br.ReadBytes([Math]::Max($Count, 8))

    Write-Output ("bytes  : " + (($bytes | ForEach-Object { $_.ToString('X2') }) -join ' '))
    Write-Output ("float  : " + [BitConverter]::ToSingle($bytes, 0).ToString([System.Globalization.CultureInfo]::InvariantCulture))
    Write-Output ("double : " + [BitConverter]::ToDouble($bytes, 0).ToString([System.Globalization.CultureInfo]::InvariantCulture))
    Write-Output ("int32  : " + [BitConverter]::ToInt32($bytes, 0))
    Write-Output ("int64  : " + [BitConverter]::ToInt64($bytes, 0))
}
finally {
    $br.Close(); $fs.Close()
}
