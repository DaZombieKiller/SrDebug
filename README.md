# SrDebug
A debug menu for Slime Rancher

![Screenshot of SrDebug in action](http://i.imgur.com/1nALiAO.png)

# Usage
### Official release
1. Extract the zip to your _Slime Rancher_ game directory
  * usually **SteamApps/common/Slime Rancher/** on Windows
  * usually **Application Support/Steam/steamapps/common/Slime Rancher/Resources** on macOS
2. Run `SrDebug-Patcher.exe`, or `mono SrDebug-Patcher.exe` for non-Windows
3. Wait for the patch tool to complete, then run _Slime Rancher_

### Custom built
1. Create a folder named **SrDebug-Content** in your _Slime Rancher_ game folder
2. Browse to **Source/Assembly-SrDebug/bin/.../\<Build Type\>/** where \<Build Type\> can be *Debug* or *Release*, depending on how you compiled
3. Copy _Assembly-SrDebug.dll_ to the **SrDebug-Content** folder in your _Slime Rancher_ game directory
4. Browse to **Source/SrDebug-Patcher/bin/.../\<Build Type\>/** where \<Build Type\> can be *Debug* or *Release*, depending on how you compiled.
5. Copy _Mono.Cecil.dll_ to the **SrDebug-Content** directory
6. Copy _SrDebug-Patcher.exe_ and _SrDebug-Patcher.exe.config_ to your _Slime Rancher_ game directory
6. Run `SrDebug-Patcher.exe`, or `mono SrDebug-Patcher.exe` for non-Windows
7. Wait for the patch tool to complete, then run _Slime Rancher_

# Building
1. Browse to your _Slime Rancher_ game directory
  * usually **SteamApps/common/Slime Rancher/** on Windows
  * usually **Application Support/Steam/steamapps/common/Slime Rancher/Resources** on macOS
2. Locate the game's _Managed_ folder (SlimeRancher_Data/Managed on Windows & Linux, Content/Resources/Data/Managed on Mac)
3. Copy _UnityEngine.dll_ and _Assembly-CSharp.dll_ into the _Managed_ folder in the _SrDebug_ repository root
4. Run `generate.bat` or `Protobuild --generate` for non-Windows
5. Build the resultant solution file
