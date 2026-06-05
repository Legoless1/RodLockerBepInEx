# Rod Locker (BepInEx Edition)

BepInEx mod for DREDGE.

Adds a Rod Locker button to Blackstone Isle. The locker contains one of each platform-colored Custom Rod variant, including the mobile iOS and Android rods. Each rod is free, uses the vanilla Custom Rod stats, and can only be taken once per save.

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
