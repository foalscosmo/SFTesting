using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
	[System.Serializable]
	public enum SoundTypesEnum
	{
		MainMusicAudioClip,
		GainZombie,
		DestroyCar,
		DestroyRandomObjects,
		MenuClickSound,
		CarBump,
		AttackVoiceLine,
		GethitVoiceLine,
		WinVoiceLine,
		LoseVoiceLine,
		GameStartVoiceLine,
		KillEnemyVoiceLine,
		LureZombieVoiceLine,
		SnekDeath,
		SnekKill,
		SnekBoost,
		SnekCoinGain,
		Destructible,
		Crown
	}

	[System.Serializable]
	public class Sounds
	{
		public SoundTypesEnum SoundTypes;
		public AudioClip[] soundClip;
		
		[Range(0,100)]
		public int voiceLineWeight = 10;
	}
	
	public class SoundManager : MonoBehaviour
	{
		public static SoundManager instance;

		public static bool soundOn = true, musicOn = false;

		public int soundPoolLength = 20;

		public Sounds[] sounds;

		public AudioMixer musicMixer;
		
		private Dictionary<SoundTypesEnum, AudioClip[]> _soundsDictionary = new Dictionary<SoundTypesEnum, AudioClip[]>();
		private Dictionary<SoundTypesEnum, int> _voiceLineWeights = new Dictionary<SoundTypesEnum, int>();
		
		private HashSet<AudioSource> _freeAudioSources = new HashSet<AudioSource>();
		private HashSet<AudioSource> _inUseAudioSources = new HashSet<AudioSource>();

		private AudioSource _musicSource, _gemGainloopSource;

		private bool _soundsAreOn = true, _musicIsOn = true;

		private Coroutine _currentlyActiveVoiceLineCoroutine;
		private AudioSource _currentlyActiveVoiceLineAudioSource;
		private SoundTypesEnum _currentlyActiveVoiceLineType;


		private void Awake()
		{
			instance = this;
			Initialize();
		}

		public void Initialize()
		{;
			for (var i = 0; i < sounds.Length; i++)
			{
				if (!_soundsDictionary.ContainsKey(sounds[i].SoundTypes))
				{
					_soundsDictionary.Add(sounds[i].SoundTypes, sounds[i].soundClip);
				}
				if (!_voiceLineWeights.ContainsKey(sounds[i].SoundTypes))
				{
					_voiceLineWeights.Add(sounds[i].SoundTypes, sounds[i].voiceLineWeight);
				}
			}

			for (var i = 0; i < soundPoolLength; i++)
			{
				_freeAudioSources.Add(CreateAudioSource(!_soundsAreOn, false, "SoundSource"));
			}

			_musicSource = CreateAudioSource(!_musicIsOn, true, "MusicSource");
			//_gemGainloopSource = CreateAudioSource(!_musicIsOn, true, "GemSoundLoop");
			_musicSource.volume = 0.22f;
			_musicSource.priority = 40;
			//_gemGainloopSource.volume = 0.5f;
			PlayMusic(SoundTypesEnum.MainMusicAudioClip);
		}

		private AudioSource CreateAudioSource(bool mute, bool loop, string newName)
		{
			var source = new GameObject(newName).AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.transform.SetParent(this.transform);
			source.loop = loop;
			source.mute = mute;
			return source;
		}

		public void SetMusicSpeed(bool makeFaster, int timeElapsed = 0)
		{
			if (makeFaster)
			{
				var playbackSpeedModifier = (float) timeElapsed / 9000f + 1.1f;
				_musicSource.pitch = playbackSpeedModifier;
				musicMixer.SetFloat("pitchBend", 1f / playbackSpeedModifier);
			}
			else
			{
				_musicSource.pitch = 1f;
				musicMixer.SetFloat("pitchBend", 1f);
			}
		}

		public void PlayMenuSound()
		{
			PlaySound(SoundTypesEnum.MenuClickSound);
		}

		/*public void PlayBuyGemSound()
		{
			PlaySound(MSFSoundTypeEnum.BuyAudioClip);
		}*/

		/// <summary>
		/// Play voice line audio clip.
		/// </summary>
		/// <param name="soundTypesToPlay">Sound type to play.</param>
		/// <param name="random">If random is true, it sound may not play all the time.</param>
		/// <param name="randomNumber">Higher the randomNumber, higher the chance of playing the voice.</param>
		public void PlayVoiceLine(SoundTypesEnum soundTypesToPlay, bool random = true, int randomNumber = 50)
		{
			var shouldPlay = !random;
			
			if (!shouldPlay)
			{
				shouldPlay = (Random.Range(0, 100) < randomNumber);
			}

			if (shouldPlay && _currentlyActiveVoiceLineAudioSource != null)
			{
				shouldPlay = _voiceLineWeights[_currentlyActiveVoiceLineType] < _voiceLineWeights[soundTypesToPlay];
				
				if (shouldPlay)
				{
					if (_currentlyActiveVoiceLineCoroutine != null)
						StopCoroutine(_currentlyActiveVoiceLineCoroutine);
					
					_currentlyActiveVoiceLineAudioSource.Pause();
					_currentlyActiveVoiceLineAudioSource.time = 0f;
					_inUseAudioSources.Remove(_currentlyActiveVoiceLineAudioSource);
					_freeAudioSources.Add(_currentlyActiveVoiceLineAudioSource);
					
					_currentlyActiveVoiceLineAudioSource = null;
					_currentlyActiveVoiceLineCoroutine = null;
					_currentlyActiveVoiceLineType = SoundTypesEnum.CarBump;
				}
			}

			if (shouldPlay)
			{
				PlaySound(soundTypesToPlay, true);
			}
		}

		public void PlaySound(SoundTypesEnum soundTypesToPlay, bool isVoiceLine = false)
		{
			if (soundTypesToPlay != SoundTypesEnum.SnekBoost && soundTypesToPlay != SoundTypesEnum.SnekDeath &&
			    soundTypesToPlay != SoundTypesEnum.SnekKill && soundTypesToPlay != SoundTypesEnum.SnekCoinGain && 
			    soundTypesToPlay != SoundTypesEnum.Destructible && soundTypesToPlay != SoundTypesEnum.Crown)
				return;
			if (!soundOn) return;
			if(_freeAudioSources.Count == 0) return;
			
			var source = _freeAudioSources.First();

			if (soundTypesToPlay == SoundTypesEnum.GainZombie)
			{
				source.volume = 0.2f;
			}
			else
			{
				source.volume = 0.45f;
			}
			
			_freeAudioSources.Remove(source);
			source.clip = _soundsDictionary[soundTypesToPlay][Random.Range(0,_soundsDictionary[soundTypesToPlay].Length)];
			_inUseAudioSources.Add(source);
			source.Play();

			if (isVoiceLine)
			{
				_currentlyActiveVoiceLineCoroutine 		= StartCoroutine(OnSoundFinish(source.clip.length, source));
				_currentlyActiveVoiceLineType 			= soundTypesToPlay;
				_currentlyActiveVoiceLineAudioSource 	= source;
			}
			else
			{
				StartCoroutine(OnSoundFinish(source.clip.length, source));
			}
		}

		public void PlayMusic(SoundTypesEnum soundTypesToPlay)
		{
			if (!musicOn)
			{
				_musicSource.Pause();
			}
			else if (!_musicSource.isPlaying || (_musicSource.clip == null && musicOn))
			{
				_musicSource.clip = _soundsDictionary[soundTypesToPlay][Random.Range(0,_soundsDictionary[soundTypesToPlay].Length)];
				_musicSource.Play();
			}
		}

		/*public void PlayStopGemSound(bool start)
		{
			if (!soundOn) return;
			if (start)
			{
				PlaySound(MSFSoundTypeEnum.SmallGemGainSoundStart);
			
				MSFGameManager.instance.StartWaiter(() =>
				{
					_gemGainloopSource.clip = _soundsDictionary[MSFSoundTypeEnum.SmallGemGainSound];
					_gemGainloopSource.Play();
				}, 0.816f);
			}
			else
			{
				_gemGainloopSource.Stop();
			}
		}*/

		private IEnumerator OnSoundFinish(float time, AudioSource source)
		{
			yield return new WaitForSeconds(time);
			_inUseAudioSources.Remove(source);
			_freeAudioSources.Add(source);

			if (_currentlyActiveVoiceLineAudioSource == source && _currentlyActiveVoiceLineAudioSource != null)
			{
				_currentlyActiveVoiceLineType = SoundTypesEnum.CarBump;
				_currentlyActiveVoiceLineAudioSource = null;
			}
		}

		public void SetSoundsOnOff(bool setOn)
		{
			soundOn = musicOn = setOn;
			_musicSource.mute = !setOn;
		}
	}
}
