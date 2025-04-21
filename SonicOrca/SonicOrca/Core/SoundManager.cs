// Decompiled with JetBrains decompiler
// Type: SonicOrca.Core.SoundManager
// Assembly: SonicOrca, Version=2.0.1012.10518, Culture=neutral, PublicKeyToken=null
// MVID: 2E579C53-B7D9-4C24-9AF5-48E9526A12E7
// Assembly location: C:\Games\S2HD_2.0.1012-rc2\SonicOrca.dll

using SonicOrca.Audio;
using SonicOrca.Geometry;
using SonicOrca.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonicOrca.Core
{

    public class SoundManager
    {
      private readonly int NumJingleTypes = EnumHelpers.GetEnumCount(typeof (JingleType));
      private readonly SampleInfo[] _jingleSampleInfo = new SampleInfo[EnumHelpers.GetEnumCount(typeof (JingleType))];
      private readonly SampleInstance[] _jingleInstance = new SampleInstance[EnumHelpers.GetEnumCount(typeof (JingleType))];
      private readonly List<JingleType> _jingleOrder = new List<JingleType>();
      private readonly List<LevelSound> _sounds = new List<LevelSound>();
      private readonly List<SampleInstance> _simpleInstances = new List<SampleInstance>();
      private readonly SonicOrcaGameContext _gameContext;
      private readonly Level _level;
      private readonly ResourceTree _resourceTree;
      private SampleInfo _music;
      private SampleInfo _queuedMusic;
      private SampleInstance _musicInstance;
      private JingleType _pausedJingle;
      private SoundManager.MusicFadeState _musicFadeState;
      private double _musicFadeVolume;
      private double _musicFadeAmount;

      public SampleInstance MusicInstance => this._musicInstance;

      public SampleInstance CurrentInstance
      {
        get
        {
          return ((IEnumerable<SampleInstance>) this._jingleInstance).FirstOrDefault<SampleInstance>((Func<SampleInstance, bool>) (x => x != null && x.Playing)) ?? this._musicInstance;
        }
      }

      public SoundManager(Level level)
      {
        this._gameContext = level.GameContext;
        this._level = level;
        this._resourceTree = this._gameContext.ResourceTree;
      }

      public void Update()
      {
        this.UpdateMusicFading();
        this.CheckFinishedJingles();
        this.UpdateSounds();
        this._simpleInstances.RemoveAll((Predicate<SampleInstance>) (x => !x.Playing));
      }

      public void PlayMusic(string fullKeyPath)
      {
        if (string.IsNullOrEmpty(fullKeyPath))
          return;
        this.PlayMusic(this._resourceTree.GetLoadedResource<SampleInfo>(fullKeyPath));
      }

      public void PlayMusic(SampleInfo si)
      {
        this.SelectMusic(si);
        this.PlayMusic();
      }

      public void CrossFadeMusic(string fullKeyPath)
      {
        this.CrossFadeMusic(this._resourceTree.GetLoadedResource<SampleInfo>(fullKeyPath));
      }

      public void CrossFadeMusic(SampleInfo si)
      {
        if (this.CurrentInstance != null)
        {
          this._queuedMusic = si;
          this.FadeOutMusic(120);
        }
        else
        {
          this.SelectMusic(si);
          this.PlayMusic();
        }
      }

      private void SelectMusic(SampleInfo si)
      {
        if (this._music == si)
          return;
        if (this._musicInstance != null)
        {
          this._musicInstance.Stop();
          this._musicInstance.Dispose();
        }
        this._music = si;
        this._musicInstance = new SampleInstance(this._gameContext, si);
      }

      public void UpdateMusicFading()
      {
        SampleInstance currentInstance = this.CurrentInstance;
        if (currentInstance == null)
          return;
        switch (this._musicFadeState)
        {
          case SoundManager.MusicFadeState.FadingOut:
            this._musicFadeVolume -= this._musicFadeAmount;
            currentInstance.Volume = Math.Min(currentInstance.Volume, this._musicFadeVolume);
            if (currentInstance.Volume > 0.0)
              break;
            currentInstance.Volume = 0.0;
            currentInstance.Stop();
            this._musicFadeState = SoundManager.MusicFadeState.None;
            if (this._queuedMusic == null)
              break;
            this.SelectMusic(this._queuedMusic);
            this._queuedMusic = (SampleInfo) null;
            this.PlayMusic();
            break;
          case SoundManager.MusicFadeState.FadingIn:
            currentInstance.Volume += this._musicFadeAmount;
            if (currentInstance.Volume < 1.0)
              break;
            currentInstance.Volume = 1.0;
            this._musicFadeState = SoundManager.MusicFadeState.None;
            break;
        }
      }

      public void PlayMusic(bool continueFadeOut = false)
      {
        if (this._musicInstance == null)
          return;
        this.StopAllJingles();
        this._musicInstance.Volume = 1.0;
        if (continueFadeOut && this._musicFadeState == SoundManager.MusicFadeState.FadingOut)
          this._musicInstance.Volume = this._musicFadeVolume;
        else
          this._musicFadeState = SoundManager.MusicFadeState.None;
        this._musicInstance.Classification = SampleInstanceClassification.Music;
        this._musicInstance.SeekToStart();
        this._musicInstance.Play();
      }

      public void PauseAll()
      {
        this._pausedJingle = this._jingleOrder.LastOrDefault<JingleType>();
        for (int index = 0; index < this.NumJingleTypes; ++index)
        {
          SampleInstance sampleInstance = this._jingleInstance[index];
          if (sampleInstance != null && sampleInstance.Playing)
            sampleInstance.Stop();
        }
        if (this._musicInstance != null)
          this._musicInstance.Stop();
        foreach (LevelSound sound in this._sounds)
          sound.Pause();
      }

      public void ResumeAll()
      {
        if (this._pausedJingle != JingleType.None && this._jingleInstance[(int) this._pausedJingle] != null)
          this._jingleInstance[(int) this._pausedJingle].Play();
        else if (this._musicInstance != null)
          this._musicInstance.Play();
        this._pausedJingle = JingleType.None;
        foreach (LevelSound sound in this._sounds)
          sound.Resume();
      }

      public void FadeInMusic(int fadeDuration = 60)
      {
        if (this._musicInstance == null)
          return;
        this._musicInstance.Volume = 0.0;
        this._musicInstance.Play();
        this._musicFadeState = SoundManager.MusicFadeState.FadingIn;
        this._musicFadeAmount = 1.0 / (double) fadeDuration;
      }

      public void FadeOutMusic(int fadeDuration)
      {
        this._musicFadeState = SoundManager.MusicFadeState.FadingOut;
        this._musicFadeVolume = this.CurrentInstance.Volume;
        this._musicFadeAmount = 1.0 / (double) fadeDuration;
      }

      public void MuteMusic()
      {
        if (this._musicInstance == null)
          return;
        this._musicInstance.Volume = 0.0;
      }

      public void StopMusic()
      {
        if (this._musicInstance == null)
          return;
        this._musicInstance.Stop();
      }

      public void CheckFinishedJingles()
      {
        for (int index = 0; index < this.NumJingleTypes; ++index)
        {
          if (this._jingleInstance[index] != null && !this._jingleInstance[index].Playing)
            this.StopJingle((JingleType) index);
        }
      }

      public void SetJingle(JingleType jingleType, string resourceKey)
      {
        Resource resource = this._gameContext.ResourceTree[resourceKey].Resource;
        SampleInfo sampleInfo = resource.Identifier == ResourceTypeIdentifier.SampleInfo ? (SampleInfo) resource.LoadedResource : new SampleInfo((Sample) resource.LoadedResource);
        this._jingleSampleInfo[(int) jingleType] = sampleInfo;
      }

      public void SetJingle(JingleType jingleType, SampleInfo si)
      {
        this._jingleSampleInfo[(int) jingleType] = si;
      }

      public void PlayJingleOnce(Sample sample)
      {
        SampleInstance sampleInstance = new SampleInstance(this._gameContext, sample);
        sampleInstance.Classification = SampleInstanceClassification.Music;
        sampleInstance.Play();
        this._simpleInstances.Add(sampleInstance);
      }

      public void PlayJingle(JingleType jingleType)
      {
        if (this._jingleSampleInfo[(int) jingleType] == null)
          return;
        switch (jingleType)
        {
          case JingleType.Life:
            this.MuteMusic();
            break;
          case JingleType.Invincibility:
            this.StopMusic();
            break;
          case JingleType.SpeedShoes:
            this.StopMusic();
            break;
          case JingleType.Super:
            this.StopMusic();
            break;
          case JingleType.Drowning:
            this.StopMusic();
            break;
          default:
            return;
        }
        for (int index = 0; index < this.NumJingleTypes; ++index)
        {
          if (this._jingleInstance[index] != null)
            this._jingleInstance[index].Volume = 0.0;
        }
        if (this._jingleInstance[(int) jingleType] == null)
          this._jingleInstance[(int) jingleType] = new SampleInstance(this._gameContext, this._jingleSampleInfo[(int) jingleType]);
        else
          this._jingleOrder.Remove(jingleType);
        this._jingleOrder.Add(jingleType);
        this._jingleInstance[(int) jingleType].Classification = SampleInstanceClassification.Music;
        this._jingleInstance[(int) jingleType].Volume = 1.0;
        this._jingleInstance[(int) jingleType].SeekToStart();
        this._jingleInstance[(int) jingleType].Play();
      }

      public void StopJingle(JingleType jingleType)
      {
        if (this._jingleInstance[(int) jingleType] == null)
          return;
        this._jingleInstance[(int) jingleType].Stop();
        this._jingleInstance[(int) jingleType].Dispose();
        this._jingleInstance[(int) jingleType] = (SampleInstance) null;
        this._jingleOrder.Remove(jingleType);
        if (this._jingleOrder.Count == 0)
        {
          switch (jingleType)
          {
            case JingleType.Life:
              if (this._musicFadeState == SoundManager.MusicFadeState.FadingOut)
                break;
              this.FadeInMusic();
              break;
            case JingleType.Drowning:
              break;
            default:
              this.PlayMusic(true);
              break;
          }
        }
        else
          this._jingleInstance[(int) this._jingleOrder.Last<JingleType>()].Volume = 1.0;
      }

      public void StopAllJingles()
      {
        for (int index = 0; index < this.NumJingleTypes; ++index)
        {
          if (this._jingleInstance[index] != null && this._jingleInstance[index].Playing)
            this.StopJingle((JingleType) index);
        }
      }

      public void StopAll()
      {
        this.StopMusic();
        this.StopAllJingles();
        foreach (LevelSound sound in this._sounds)
          sound.Dispose();
        this._sounds.Clear();
        foreach (SampleInstance simpleInstance in this._simpleInstances)
          simpleInstance.Dispose();
      }

      public void PlaySound(IActiveObject activeObject, string resourceKey)
      {
        this.PlaySound(activeObject.Position, resourceKey);
      }

      public void PlaySound(Vector2i position, string resourceKey)
      {
        Sample loadedResource;
        if (!this._resourceTree.TryGetLoadedResource<Sample>(resourceKey, out loadedResource))
          return;
        this.PlaySound(position, loadedResource);
      }

      public void PlaySound(Vector2i position, Sample sample)
      {
        LevelSound levelSound = new LevelSound(this._level, sample, position);
        levelSound.Play();
        this.AddLevelSound(levelSound);
      }

      public void AddLevelSound(LevelSound levelSound) => this._sounds.Add(levelSound);

      public void PlaySound(string resourceKey)
      {
        Sample loadedResource;
        if (!this._resourceTree.TryGetLoadedResource<Sample>(resourceKey, out loadedResource))
          return;
        this.PlaySound(loadedResource);
      }

      public void PlaySound(Sample sample)
      {
        SampleInstance sampleInstance = new SampleInstance(this._gameContext, sample);
        sampleInstance.Play();
        this._simpleInstances.Add(sampleInstance);
      }

      public void PlaySoundVisibleOnly(string resourceKey, Vector2i position)
      {
        if (!this._level.Camera.Bounds.Contains((Vector2) position))
          return;
        this.PlaySound(resourceKey);
      }

      private void UpdateSounds()
      {
        foreach (LevelSound sound in this._sounds)
          sound.Update();
        this._sounds.RemoveAll((Predicate<LevelSound>) (s => s.Finished));
      }

      private enum MusicFadeState
      {
        None,
        FadingOut,
        FadingIn,
      }
    }
}
