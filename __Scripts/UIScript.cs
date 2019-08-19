using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
	private static UIScript _S;
	[Header("To use this script you need to set a GUI, GameOver, and LevelChange tag.")]
	[Tooltip("Where the player score, jumps, achievements, and pause is located.")]
	public Canvas playerCanvas;
	private Text _jumpsText;
	private Text _scoreText;
	private Toggle _pauseToggle;
	private Image _achievementImage;
	private Text _achievementTitleText;
	private Text _achievementDescriptionText;
	bool _achievementDone = true; //checks for when the achievement is done displaying so it can do the next one.

	[Tooltip("Canvas shown on game over")]
	public Canvas gameOverCanvas;
	private Text _finalScoreText;
	private Text _gameOverText;

	[Tooltip("Canvas shown on start")]
	public Canvas menuCanvas;

	[Tooltip("Canvas shown on level change")]
	public Canvas levelChangeCanvas;
	private Text _currentLevelText;
	private Text _levelSettingsText;

	[Tooltip("Canvas shown on Pause")]
	public Canvas pauseCanvas;
	private ToggleGroup _turretToggleGroup;
	private Toggle[] _turretToggleList;
	private ToggleGroup _bodyToggleGroup;
	private Toggle[] _bodyToggleList;

	private void Awake()
	{
		if (_S == null)
			_S = this;

		#region Menu

		if (menuCanvas == null)
		{
			menuCanvas = InitCanvas("MenuCanvas");
		}

		#endregion

		#region Player canvas

		if (playerCanvas == null)
		{
			playerCanvas = InitCanvas("PlayerCanvas");
		}
		if (playerCanvas != null)
		{
			_jumpsText = InitText("JumpsText", playerCanvas.transform);
			_scoreText = InitText("ScoreText", playerCanvas.transform);
			_pauseToggle = InitToggle("PauseToggle", playerCanvas.transform);
			_achievementImage = InitImage("AchievementImage", playerCanvas.transform);
			_achievementTitleText = InitText("AchievementTitleText", playerCanvas.transform);
			_achievementDescriptionText = InitText("AchievementDescriptionText", playerCanvas.transform);
		}
		#endregion

		#region Game Over

		if (gameOverCanvas == null)
		{
			gameOverCanvas = InitCanvas("GameOver");
		}
		if (gameOverCanvas != null)
		{
			_finalScoreText = InitText("FinalScoreText", gameOverCanvas.transform);
			_gameOverText = InitText("GameOverText", gameOverCanvas.transform);
		}
		#endregion

		#region Level Change

		if (levelChangeCanvas == null)
		{
			levelChangeCanvas = InitCanvas("LevelChange");
		}
		if (levelChangeCanvas != null)
		{
			_currentLevelText = InitText("CurrentLevelText", levelChangeCanvas.transform);
			_levelSettingsText = InitText("LevelSettingsText", levelChangeCanvas.transform);
		}
		#endregion

		#region Pause
		if (pauseCanvas == null)
		{
			pauseCanvas = InitCanvas("PauseCanvas");
		}
		if (pauseCanvas != null)
		{
			_turretToggleGroup = InitToggleGroup("TurretToggleGroup", pauseCanvas.transform);
			_bodyToggleGroup = InitToggleGroup("BodyToggleGroup", pauseCanvas.transform);
		}
		#endregion

		
		HideLevelChange();
		HideGameOver();
		HideAchievement();
		_turretToggleList = GetTurretToggles();
		_bodyToggleList = GetBodyToggles();
		ShowPause(false);
	}
	

	#region Initialization help

	/// <summary>
	/// Finds canvas with tag
	/// </summary>
	/// <param name="name"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	Canvas InitCanvas(string tag)
	{
		Canvas tempCanvas = null;

		if (gameObject.DoesTagExist(tag))
		{
			GameObject tempObject = GameObject.FindGameObjectWithTag(tag);
			if (tempObject.GetComponent<Canvas>())
			{
				tempCanvas = tempObject.GetComponent<Canvas>();
			}
			else
			{
				Debug.LogError("No canvas on " + tag + " GameObject Set. Make sure to add a canvas.");
			}
		}
		return tempCanvas;
	}

	/// <summary>
	/// Finds Text component on a child of parent object by the name given
	/// </summary>
	/// <param name="name"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	Text InitText(string name, Transform parent)
	{
		Text[] tempTextArray = parent.GetComponentsInChildren<Text>();
		foreach (Text child in tempTextArray)
		{
			if (child.name == name)
			{
				return child;
			}
		}

		Debug.LogError("Nothing named by " + name + " is a child of " + parent.name + ".");
		return null;
	}

	/// <summary>
	/// Finds Toggle component on a child of parent object by the name given
	/// </summary>
	/// <param name="name"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	Toggle InitToggle(string name, Transform parent)
	{
		Toggle[] tempTextArray = parent.GetComponentsInChildren<Toggle>();
		foreach (Toggle child in tempTextArray)
		{
			if (child.name == name)
			{
				return child;
			}
		}

		Debug.LogError("Nothing named by " + name + " is a child of " + parent.name + ".");
		return null;
	}

	/// <summary>
	/// Finds ToggleGroup component on a child of parent object by the name given
	/// </summary>
	/// <param name="name"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	ToggleGroup InitToggleGroup(string name, Transform parent)
	{
		ToggleGroup[] tempTextArray = parent.GetComponentsInChildren<ToggleGroup>();
		foreach (ToggleGroup child in tempTextArray)
		{
			if (child.name == name)
			{
				return child;
			}
		}

		Debug.LogError("Nothing named by " + name + " is a child of " + parent.name + ".");
		return null;
	}

	/// <summary>
	/// Finds Image component on a child of parent object by the name given
	/// </summary>
	/// <param name="name"></param>
	/// <param name="parent"></param>
	/// <returns></returns>
	Image InitImage(string name, Transform parent)
	{
		Image[] tempTextArray = parent.GetComponentsInChildren<Image>();
		foreach (Image child in tempTextArray)
		{
			if (child.name == name)
			{
				return child;
			}
		}

		Debug.LogError("Nothing named by " + name + " is a child of " + parent.name + ".");
		return null;
	}

	#endregion

	#region Player canvas methods
	//score stuff

	public static void UpdateScore(int value)
	{
		if (_S._scoreText != null)
		{
			
			_S._scoreText.text = value.ToString("N0");
			
		}
	}

	//jumps stuff

	public static void UpdateJumps(int value)
	{
		if (_S._jumpsText != null)
		{
			_S._jumpsText.text = value + " Jumps";
		}
	}

	//Pause stuff

	public static bool Paused()
	{
		return _S._pauseToggle.isOn;
	}

	//Achievement stuff

	public static void DisplayAchievementForTime(float time, string title, string description)
	{

		if (_S._achievementDone)
		{
			if (_S._achievementTitleText != null)
			{
				_S._achievementTitleText.text = title;
			}
			if (_S._achievementDescriptionText != null)
			{
				_S._achievementDescriptionText.text = description;
			}
			_S.ShowAchievement();
			_S._achievementDone = false;
			_S.StartCoroutine("WaitForTimeAchievement", time);
		}
	}
	IEnumerator WaitForTimeAchievement(float time)
	{
		yield return new WaitForSeconds(time);
		HideAchievement();
		_S._achievementDone = true;
	}

	public static bool AchievementDone()
	{
		return _S._achievementDone;
	}

	void HideAchievement()
	{
		if (_S._achievementImage != null)
		{
			_S._achievementImage.enabled = false;
		}
		if (_S._achievementTitleText != null)
		{
			_S._achievementTitleText.enabled = false;
		}
		if (_S._achievementDescriptionText != null)
		{
			_S._achievementDescriptionText.enabled = false;
		}
	}
	void ShowAchievement()
	{
		if (_S._achievementImage != null)
		{
			_S._achievementImage.enabled = true;
		}
		if (_S._achievementTitleText != null)
		{
			_S._achievementTitleText.enabled = true;
		}
		if (_S._achievementDescriptionText != null)
		{
			_S._achievementDescriptionText.enabled = true;
		}
	}
	#endregion

	#region Game Over canvas methods

	public void ShowGameOver()
	{
		if (gameOverCanvas != null)
		{
			gameOverCanvas.enabled = true;
		}
	}

	public void HideGameOver()
	{
		if (gameOverCanvas != null)
		{
			gameOverCanvas.enabled = false;
		}
	}

	public static void SetGameOver(bool gameOver)
	{
		if (_S.gameOverCanvas != null)
		{
			_S.gameOverCanvas.enabled = gameOver;
		}
	}
	public static void SetGameOverText(string text)
	{
		if (_S._gameOverText != null)
		{
			_S._gameOverText.text = text;
		}
	}
	public static void SetFinalScore(int score, int level)
	{
		if (_S._finalScoreText != null)
		{
			_S._finalScoreText.text = "Final Score: " + score + ", Final Level: " + level;
		}
	}
	#endregion

	#region Level change canvas methods

	public void ShowLevelChange()
	{
		if (levelChangeCanvas != null)
		{
			levelChangeCanvas.enabled = true;
		}
	}

	public void HideLevelChange()
	{
		if (levelChangeCanvas != null)
		{
			levelChangeCanvas.enabled = false;
		}
	}
	public static void SetLevelSettings(int asteroidParents, int asteroidChildren)
	{
		if (_S._levelSettingsText != null)
		{
			_S._levelSettingsText.text = asteroidParents + " parents, " + asteroidChildren + " children";
		}
	}
	public void DisplayLevelSettingsForTime(float time)
	{
		if (levelChangeCanvas != null && _currentLevelText != null)
		{
			levelChangeCanvas.enabled = true;
			_currentLevelText.text = "Level " + AsteraX.GetLevel();
			Time.timeScale = 0;
			StartCoroutine("HideLevelDisplay", time);
		}
	}
	IEnumerator HideLevelDisplay(float time)
	{
		yield return new WaitForSecondsRealtime(time);
		Time.timeScale = 1;
		if (levelChangeCanvas != null)
		{
			levelChangeCanvas.enabled = false;
		}
	}
	#endregion

	#region Menu canvas methods

	public void HideMenu()
	{
		if(menuCanvas != null)
			menuCanvas.enabled = false;
	}
	public void ShowMenu()
	{
		if (menuCanvas != null)
			menuCanvas.enabled = true;
	}
	public void DeleteSave()
	{
		SaveGameManager.DeleteSave();
	}
	#endregion

	#region Pause canvas methods

	public static void ShowPause(bool shown)
	{
		if (_S.pauseCanvas != null)
			_S.pauseCanvas.enabled = shown;
	}

	public static void UnlockToggleFromAchievement(AchievementSettings setting)
	{
		if (setting != null)
		{
			if (setting.complete)
			{
				switch ((int)setting.partAwarded)
				{
					case 0:
						if (setting.indexOfPart < _S._turretToggleList.Length && setting.indexOfPart > 0)
							_S._turretToggleList[setting.indexOfPart].interactable = true;
						else
							Debug.LogError("achievement out of range.");
						break;
					case 1:
						if (setting.indexOfPart < _S._bodyToggleList.Length && setting.indexOfPart > 0)
							_S._bodyToggleList[setting.indexOfPart].interactable = true;
						else
							Debug.LogError("achievement out of range.");
						break;

				}
			}
			else
			{
				switch ((int)setting.partAwarded)
				{
					case 0:
						if (setting.indexOfPart < _S._turretToggleList.Length && setting.indexOfPart > 0)
							_S._turretToggleList[setting.indexOfPart].interactable = false;
						else
							Debug.LogError("achievement out of range.");
						break;
					case 1:
						if (setting.indexOfPart < _S._bodyToggleList.Length && setting.indexOfPart > 0)
							_S._bodyToggleList[setting.indexOfPart].interactable = false;
						else
							Debug.LogError("achievement out of range.");
						break;

				}
			}
		}
	}

	//turrets

	public Toggle[] GetTurretToggles()
	{
		if (_turretToggleGroup != null)
		{
			return _turretToggleGroup.ActiveToggles(true);
		}
		return null;
	}
	public static int GetActiveTurretIndex()
	{
		
		return _S._turretToggleGroup.GetActiveToggleIndex();
	}
	public static void SetActiveTurretIndex(int index)
	{
		_S._turretToggleGroup.allowSwitchOff = true;
		if (_S._turretToggleList != null)
		{
			foreach (Toggle toggle in _S._turretToggleList)
			{
				if (toggle == _S._turretToggleList[index])
				{
					toggle.isOn = true;
				}
				else
				{
					toggle.isOn = false;
				}
			}
		}
		_S._turretToggleGroup.allowSwitchOff = false;
	}

	//bodies

	public Toggle[] GetBodyToggles()
	{
		if (_bodyToggleGroup != null)
		{
			return _bodyToggleGroup.ActiveToggles(true);
		}
		return null;
	}
	public static int GetActiveBodyIndex()
	{
		return _S._bodyToggleGroup.GetActiveToggleIndex();
	}
	public static void SetActiveBodyIndex(int index)
	{
		_S._bodyToggleGroup.allowSwitchOff = true;
		if (_S._bodyToggleList != null)
		{
			foreach (Toggle toggle in _S._bodyToggleList)
			{
				if (toggle == _S._bodyToggleList[index])
				{
					toggle.isOn = true;
				}
				else
				{
					toggle.isOn = false;
				}
			}
		}
		_S._bodyToggleGroup.allowSwitchOff = false;
	}
	

	
	#endregion
}
