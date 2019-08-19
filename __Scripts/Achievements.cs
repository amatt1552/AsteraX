using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
	public string highScoreTitle;
	public string highScoreDescription;
	public AchievementSettings[] settings;
	private Progress _progress;
	private Queue<AchievementSettings> _settingsQueue;
	public static int ASTEROIDS_HIT;
	public static int BULLET_WRAP_COUNT;
	public static int BULLETS_FIRED;
	public static int SCORE;
	public static bool GOT_HIGH_SCORE;
	private bool _newHighScore;
	
	public float displayTime;

	private static Achievements _S;

	private void Awake()
	{
		//stores the high score and all that.
		_progress = new Progress();
		if (_S == null)
			_S = this;
		_settingsQueue = new Queue<AchievementSettings>();
		SCORE = 0;
		SaveGameManager.Load();
	}
	private void Update()
	{
		//queue for achievements.
		if (_settingsQueue.Count > 0)
		{
			if (UIScript.AchievementDone())
			{
				AchievementSettings setting = _settingsQueue.Peek();
				UIScript.DisplayAchievementForTime(displayTime, setting.title, setting.description.Replace("#", "" + setting.count));
				_settingsQueue.Dequeue();
			}
		}
	}

	//decided I'd try to only run this on an event(aka when the value changed) so after I add the value
	//I just call it on the script I added it from. plan on making methods later to make it simpler.
	/// <summary>
	/// make sure to call this after changing variables
	/// </summary>
	public static void AchievementCheck()
	{
		if (SaveGameManager.CheckHighScore(SCORE) && !_S._newHighScore)
		{
			_S._newHighScore = true;
			AchievementSettings highScoreSettings = new AchievementSettings();
			highScoreSettings.title = _S.highScoreTitle;
			highScoreSettings.description = _S.highScoreDescription;
			highScoreSettings.count = SCORE;
			_S._settingsQueue.Enqueue(highScoreSettings);
			UIScript.SetGameOverText("HIGH SCORE!");
		}
		foreach (AchievementSettings setting in _S.settings)
		{
			
			switch ((int)setting.achievementType)
			{
				case 0://LuckyShot
					if (BULLET_WRAP_COUNT >= setting.count && !setting.complete)
					{
						setting.complete = true;
						_S._settingsQueue.Enqueue(setting);
						CustomAnalytics.SendAchievementUnlocked(setting);
					}
					break;
				case 1://AsteroidHitCount
					if (ASTEROIDS_HIT >= setting.count && !setting.complete)
					{
						setting.complete = true;
						_S._settingsQueue.Enqueue(setting);
						CustomAnalytics.SendAchievementUnlocked(setting);
					}
					break;
				case 2://BulletsFired
					if (BULLETS_FIRED >= setting.count && !setting.complete)
					{
						setting.complete = true;
						_S._settingsQueue.Enqueue(setting);
						CustomAnalytics.SendAchievementUnlocked(setting);
					}
					break;
				case 3://Points
					if (SCORE >= setting.count && !setting.complete)
					{
						setting.complete = true;
						_S._settingsQueue.Enqueue(setting);
						CustomAnalytics.SendAchievementUnlocked(setting);
					}
					break;
				case 4://LevelsComplete
					if (AsteraX.GetLevel() >= setting.count && !setting.complete)
					{
						setting.complete = true;
						_S._settingsQueue.Enqueue(setting);
						CustomAnalytics.SendAchievementUnlocked(setting);
					}
					break;
				default:
					break;

			}
			
			UIScript.UnlockToggleFromAchievement(setting);
		}
		SaveGameManager.Save();
	}
	void UpdateProgress()
	{
		if (SCORE > _progress.highScore)
		{
			_progress.highScore = SCORE;
			GOT_HIGH_SCORE = true;
		}
		_progress.asteroidsHit = ASTEROIDS_HIT;
		_progress.bulletsFired = BULLETS_FIRED;
		_progress.bulletWrapCount = BULLET_WRAP_COUNT;
		_progress.currentBody = UIScript.GetActiveBodyIndex();
		_progress.currentTurret = UIScript.GetActiveTurretIndex();
	}
	public static void LoadDataFromSaveFile(SaveFile saveFile)
	{
		_S.settings = saveFile.achievements;
		_S._progress = saveFile.progress;
		ASTEROIDS_HIT = _S._progress.asteroidsHit;
		BULLETS_FIRED = _S._progress.bulletsFired;
		BULLET_WRAP_COUNT = _S._progress.bulletWrapCount;
		UIScript.SetActiveBodyIndex(_S._progress.currentBody);
		UIScript.SetActiveTurretIndex(_S._progress.currentTurret);
		print(UIScript.GetActiveBodyIndex());
	}
	public static AchievementSettings[] GetAchievements()
	{
		return _S.settings;
	}
	public static void ClearAchievements()
	{
		_S._progress = new Progress();
		UIScript.SetActiveBodyIndex(_S._progress.currentBody);
		UIScript.SetActiveTurretIndex(_S._progress.currentTurret);
		ASTEROIDS_HIT = 0;
		BULLETS_FIRED = 0;
		BULLET_WRAP_COUNT = 0;
		foreach (AchievementSettings setting in _S.settings)
		{
			setting.complete = false;
		}
		
	}
	public static Progress GetProgress()
	{
		_S.UpdateProgress();
		return _S._progress;
	}
}

[System.Serializable]
public class AchievementSettings
{
	public enum AchievementType
	{
		LuckyShot,
		AsteroidHitCount,
		BulletsFired,
		Points,
		LevelsComplete
	}
	public enum PartAwarded
	{
		Turret,
		Body
	}

	
	public AchievementType achievementType;
	public string title;
	[Tooltip("add a # where you want to display a value")]
	public string description;
	public int count;
	public PartAwarded partAwarded;
	public int indexOfPart;
	[SerializeField] 
	private bool _complete;

	public bool complete
	{
		get { return _complete; }
		internal set { _complete = value; }
	}
	
}
