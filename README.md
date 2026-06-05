# Rod Locker BepInEx Edition

Adds a Rod Locker option at Blackstone Isle in DREDGE.

The locker contains one of each Custom Rod variant, free to take once per save.

## Install

Install BepInEx 5 for DREDGE, then place `RodLockerBepInEx.dll` in:

```text
DREDGE/BepInEx/plugins/
```

## Build

Set `DredgeDir` if your DREDGE install is not in the default Steam location.

```powershell
dotnet build -c Release
```

Example:

```powershell
dotnet build -c Release -p:DredgeDir="D:\SteamLibrary\steamapps\common\DREDGE"
```
