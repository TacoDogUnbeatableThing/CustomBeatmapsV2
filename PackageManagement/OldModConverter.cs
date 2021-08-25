using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;

namespace CustomBeatmaps.Packages
{
	public class OldModConverter
	{

		private readonly string _packagesFolder;

		private readonly string _oldUserBeatmapPathName;

		private readonly string _alreadyConvertedInfoPath;

		private static readonly string DumbAudioFilenamePrefix = "USER_BEATMAPS/";

		private string OldUserBeatmapPath => Application.streamingAssetsPath + "/" + _oldUserBeatmapPathName;

		public bool DetectOldBeatmaps()
		{
			if (Directory.Exists(OldUserBeatmapPath))
			{
				AlreadyConvertedBeatmapsInfo alreadyConvertedBeatmapsInfo =
					AlreadyConvertedBeatmapsInfo.Load(_alreadyConvertedInfoPath);
				foreach (string oldBeatmapPath in Directory.EnumerateFiles(OldUserBeatmapPath))
				{
					string text = oldBeatmapPath.Replace('\\', '/');
					if ((text.EndsWith(".osu") || text.EndsWith(".beatmap")) &&
					    !alreadyConvertedBeatmapsInfo.IsBeatmapAlreadyConverted(text))
					{
						return true;
					}
				}
			}

			return false;
		}

		public OldModConverter(string packagesFolder, string oldUserBeatmapPathName, string alreadyConvertedFilename)
		{
			_packagesFolder = packagesFolder;
			_oldUserBeatmapPathName = oldUserBeatmapPathName;
			_alreadyConvertedInfoPath = OldUserBeatmapPath + "/" + alreadyConvertedFilename;
		}

		public void ConvertFiles(bool moveDontCopy, Action<string> onLog)
		{
			void Log(string message)
			{
				Debug.Log(message);
				onLog?.Invoke(message);
			}

			void MoveFile(string from, string to)
			{
				if (moveDontCopy)
				{
					File.Move(from, to);
					Log("Moved: " + from + " -> " + to);
				}
				else
				{
					File.Copy(from, to, true);
					Log("Copied: " + from + " -> " + to);
				}
			}

			try
			{
				if (Directory.Exists(OldUserBeatmapPath))
				{
					AlreadyConvertedBeatmapsInfo alreadyConvertedBeatmapsInfo =
						AlreadyConvertedBeatmapsInfo.Load(_alreadyConvertedInfoPath);
					Dictionary<string, List<string>> songToBeatmaps = new Dictionary<string, List<string>>();
					foreach (string fpath in Directory.EnumerateFiles(OldUserBeatmapPath))
					{
						if ((fpath.EndsWith(".osu") || fpath.EndsWith(".beatmap")) &&
						    !alreadyConvertedBeatmapsInfo.IsBeatmapAlreadyConverted(fpath))
						{
							string beatmapText = File.ReadAllText(fpath);
							string text = PackageGrabberUtils.GetBeatmapProp(beatmapText, "AudioFilename", fpath);
							if (text.StartsWith(DumbAudioFilenamePrefix))
							{
								text = text.Substring(DumbAudioFilenamePrefix.Length);
							}

							text = Path.GetDirectoryName(fpath) + "/" + text;
							if (!songToBeatmaps.ContainsKey(text))
							{
								songToBeatmaps[text] = new List<string>();
							}

							songToBeatmaps[text].Add(fpath);
						}
					}

					foreach (KeyValuePair<string, List<string>> pair in songToBeatmaps)
					{
						string songPath = pair.Key;
						string songName = Path.GetFileName(songPath);
						string songNameWithoutExtension = Path.GetFileNameWithoutExtension(songPath);
						string newPackageName = "LOCAL_" + songNameWithoutExtension + "_CONVERTED";
						string newPackageFullPath = _packagesFolder + "/" + newPackageName;
						if (!Directory.Exists(newPackageFullPath))
						{
							Directory.CreateDirectory(newPackageFullPath);
							Log("New directory: " + newPackageFullPath);
						}

						MoveFile(songPath, newPackageFullPath + "/" + songName);
						string artist = "(undefined)";
						DateTime dateTime = default;
						Dictionary<string, string> difficulties = new Dictionary<string, string>();
						foreach (string beatmapPath in pair.Value)
						{
							string beatmapFileName = Path.GetFileName(beatmapPath);
							string text = File.ReadAllText(beatmapPath);
							text = text.Replace(DumbAudioFilenamePrefix, "");
							artist = PackageGrabberUtils.GetBeatmapProp(text, "Artist", beatmapPath);
							string creator = PackageGrabberUtils.GetBeatmapProp(text, "Creator", beatmapPath);
							string difficulty = PackageGrabberUtils.GetBeatmapProp(text, "Version", beatmapPath);
							difficulties[difficulty] = creator;
							DateTime creationTime = File.GetCreationTime(beatmapPath);
							if (dateTime == default || creationTime > dateTime)
							{
								dateTime = creationTime;
							}

							string newBeatmapFullPath = newPackageFullPath + "/" + beatmapFileName;
							MoveFile(beatmapPath, newBeatmapFullPath);
							File.WriteAllText(newBeatmapFullPath, text);
							Log("Updated AudioFilename for " + newBeatmapFullPath);
							alreadyConvertedBeatmapsInfo.RegisterBeatmapConversion(beatmapPath, newBeatmapFullPath);
						}

						CustomPackageInfo data = new CustomPackageInfo(songNameWithoutExtension,
							$"{dateTime.Month}/{dateTime.Day}/{dateTime.Year}", artist, difficulties,
							new UniqueId(newPackageName));
						CustomPackageInfo.Save(data, newPackageFullPath + "/info.json");
					}

					AlreadyConvertedBeatmapsInfo.Save(alreadyConvertedBeatmapsInfo, _alreadyConvertedInfoPath);
					if (moveDontCopy)
					{
						string[] files = Directory.GetFiles(OldUserBeatmapPath);
						if (files.Length == 0 || (files.Length == 1 &&
						                          files[0].Replace('\\', '/') ==
						                          _alreadyConvertedInfoPath.Replace('\\', '/')))
						{
							Directory.Delete(OldUserBeatmapPath, true);
							Log("Deleted old " + DumbAudioFilenamePrefix + " directory as it was empty.");
						}
					}

					Log("\n");
					Log("CONVERSIONS FINISHED!");
				}
				else
				{
					Log("No " + _oldUserBeatmapPathName + " folder detected, nothing converted.");
				}
			}
			catch (Exception e)
			{
				Log($"ERROR: {e}");
				throw;
			}
		}
		private class AlreadyConvertedBeatmapsInfo
		{
			public Dictionary<string, string> AlreadyConverted = new Dictionary<string, string>();

			public bool IsBeatmapAlreadyConverted(string oldBeatmapfullPath)
			{
				oldBeatmapfullPath = oldBeatmapfullPath.Replace('\\', '/');
				if (!File.Exists(oldBeatmapfullPath))
				{
					return true;
				}

				if (AlreadyConverted.ContainsKey(oldBeatmapfullPath))
				{
					Debug.Log($"Does exist? {AlreadyConverted[oldBeatmapfullPath]}");
					return File.Exists(AlreadyConverted[oldBeatmapfullPath]);
				}

				Debug.Log("OOF");
				return false;
			}

			public void RegisterBeatmapConversion(string oldBeatmapPath, string newBeatmapPath)
			{
				AlreadyConverted[oldBeatmapPath] = newBeatmapPath;
			}

			private void PerformPurge()
			{
				AlreadyConverted = AlreadyConverted
					.Where(pair => File.Exists(pair.Key) && File.Exists(pair.Value))
					.ToDictionary(pair => pair.Key, pair => pair.Value);
			}

			public static AlreadyConvertedBeatmapsInfo Load(string fpath)
			{
				if (!File.Exists(fpath))
				{
					return new AlreadyConvertedBeatmapsInfo();
				}

				try
				{
					AlreadyConvertedBeatmapsInfo alreadyConvertedBeatmapsInfo =
						JsonParser.FromJson<AlreadyConvertedBeatmapsInfo>(File.ReadAllText(fpath));
					alreadyConvertedBeatmapsInfo.PerformPurge();
					return alreadyConvertedBeatmapsInfo;
				}
				catch (Exception)
				{
					return new AlreadyConvertedBeatmapsInfo();
				}
			}

			public static void Save(AlreadyConvertedBeatmapsInfo beatmapsInfo, string fpath)
			{
				File.WriteAllText(fpath, JsonWriter.ToJson(beatmapsInfo));
			}
		}
	}
}
