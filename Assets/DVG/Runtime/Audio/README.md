# Audio System

## Install
Add Git package URL: https://github.com/ducvg/DVG.git?path=Assets/_DVG/Audio 

## Usage

Add AudioManager on a gameObject, this is a DontDestroyOnLoad Singleton wrap around audio controllers. Every controller implement IAudioController, add custom controller by create class implement this then assign on AudioManager inspector. 2 controllers prepared: SfxAudio and BackgroundAudio

To play a audio, call .Play on a controller and pass in an IAudioData. 2 AudioData prepared: SingleAudioData and RandomAudioData. Create custom by implement this interface then use normally.

Get a controller:
```csharp
AudioManager.Get<SfxAudio>.Play(shootSfx);
AudioManager.Get<BackgroundAudio>.Play(backgroundAudio);
```

Create a AudioData
```csharp
[System.Serializable]
public sealed class CustomData : IAudioData
{...}
```

Use an AudioData
```csharp
[SerializeField] private RandomAudioData shootSfx //asignn clips on inspector

AudioManager.Get<SfxAudio>.Play(shootSfx);
```