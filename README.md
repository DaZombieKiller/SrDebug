# SrDebug
A debug menu for Slime Rancher

![Screenshot of SrDebug in action](http://i.imgur.com/1nALiAO.png)

# Usage
### Official release
1. Extract the zip to your _Slime Rancher_ game directory (usually **SteamApps/common/Slime Rancher/**)
2. Run `SrDebug-Patcher.exe`, or `mono SrDebug-Patcher.exe` for non-Windows
3. Wait for the patch tool to complete, then run _Slime Rancher_

### Custom built
1. Browse to **Source/Assembly-SrDebug/bin/\<Build Type\>/** where \<Build Type\> can be *Debug* or *Release*, depending on how you compiled.
2. Copy _Assembly-SrDebug.dll_ to the **SlimeRancher_Data/Managed/** folder in your _Slime Rancher_ game directory
3. Browse to **Source/SrDebug-Patcher/bin/\<Build Type\>/** where \<Build Type\> can be *Debug* or *Release*, depending on how you compiled.
4. Copy _Mono.Cecil.dll_ and _SrDebug-Patcher.exe_ to your _Slime Rancher_ game directory
5. Run `SrDebug-Patcher.exe`, or `mono SrDebug-Patcher.exe` for non-Windows
6. Wait for the patch tool to complete, then run _Slime Rancher_

# Building
1. Browse to your _Slime Rancher_ game directory (usually **SteamApps/common/Slime Rancher/**)
2. Enter the **SlimeRancher_Data/Managed/** folder
3. Copy _UnityEngine.dll_ and _Assembly-CSharp.dll_ into the _Managed_ folder in the _SrDebug_ repository root
4. Run `generate.bat` or `Protobuild --generate` for non-Windows
5. Build the resultant solution file
