# Beep Beep Beep

An interval timer app for Android and Windows built with .NET MAUI. Plays three audible beeps when each interval ends.

## Features

- Configurable primary and secondary interval durations
- Configurable rep count (up to 20)
- Pause and resume mid-session
- Settings persist between sessions
- Works on Android and Windows

## Building

### Requirements

- .NET 9 SDK
- MAUI workload: `dotnet workload install maui`
- Android workload for Android builds

### Windows

```powershell
dotnet run -f net9.0-windows10.0.19041.0
```

### Android (device)

```powershell
dotnet build BeepBeepBeep.csproj -f net9.0-android -r android-arm64 -p:EmbedAssembliesIntoApk=true
adb install -r "bin\Debug\net9.0-android\android-arm64\com.companyname.beepbeepbeep-Signed.apk"
```

## Usage

1. Tap **⚙ SETTINGS** to configure your intervals and rep count
2. Tap **START** to begin
3. Three beeps sound at the end of each primary and secondary interval
4. Use **PAUSE** / **RESUME** to hold mid-session, **STOP** to cancel
