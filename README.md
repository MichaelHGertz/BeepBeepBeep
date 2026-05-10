# Beep Beep Beep

An interval timer app for Android and Windows built with .NET MAUI. Plays three audible beeps when each interval ends.

## Features

- Configurable primary and secondary interval durations
- Configurable rep count (up to 20)
- Pause and resume mid-session
- Settings persist between sessions
- Works on Android and Windows

## Dev Environment

- Windows 11
- .NET 9 SDK with MAUI workload (`dotnet workload install maui`)
- Android platform-tools (adb) installed at `C:\platform-tools\`
- GitHub remote: https://github.com/MichaelHGertz/BeepBeepBeep

## Building & Running

### Windows (dev/test)

```powershell
dotnet run -f net9.0-windows10.0.19041.0
```

### Android — Pixel (sideload)

Build targeting ARM64 with assemblies embedded (required for manual install):

```powershell
dotnet build BeepBeepBeep.csproj -f net9.0-android -r android-arm64 -p:EmbedAssembliesIntoApk=true
```

Install to connected device (replaces existing install):

```powershell
C:\platform-tools\adb.exe install -r "bin\Debug\net9.0-android\android-arm64\com.companyname.beepbeepbeep-Signed.apk"
```

> **Note:** The device must have USB debugging enabled and the computer must be trusted.
> Run `C:\platform-tools\adb.exe devices` first to confirm the device is visible.

### Android — Emulator

Start the emulator from Android Studio first, then:

```powershell
dotnet build BeepBeepBeep.csproj -f net9.0-android -t:Run
```

## Deploying Updates

```powershell
git add .
git commit -m "your message"
git push
```

## Usage

1. Tap **⚙ SETTINGS** to configure your intervals and rep count
2. Tap **START** to begin
3. Three beeps sound at the end of each primary and secondary interval
4. Use **PAUSE** / **RESUME** to hold mid-session, **STOP** to cancel
