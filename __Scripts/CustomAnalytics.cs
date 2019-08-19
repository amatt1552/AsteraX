using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

static public class CustomAnalytics
{
    
    static public void SendAchievementUnlocked(AchievementSettings ach)
    {
        AnalyticsEvent.AchievementUnlocked(ach.title, new Dictionary<string, object>
        {
            {"time", DateTime.Now}
        });
    }


    static public void SendLevelStart(int level)
    {
        AnalyticsEvent.LevelStart(level, new Dictionary<string, object>
        {
            {"time", DateTime.Now}
        });
    }


    static public void SendGameOver()
    {
		AnalyticsEvent.GameOver(null, new Dictionary<string, object>
		{
			{ "time", DateTime.Now },
			{ "score", Achievements.SCORE },
			{ "level", AsteraX.GetLevel() },
            { "gotHighScore", Achievements.GOT_HIGH_SCORE }
        });
    }


	static public void SendFinalShipPartChoice()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("time", DateTime.Now);

		/* This is designed to accommodate additional eShipPartTypes.
        foreach (ShipParts.eShipPartType type in (ShipParts.eShipPartType[])
                 Enum.GetValues( typeof(ShipParts.eShipPartType) ) )
        {
            if (dict.Count == 10) {
                // This is necessary because AnalyticsEvent.Custom has a hard
                //  limit on its dict size of 10, according to:
                //  https://docs.unity3d.com/Manual/UnityAnalyticsCustomEventsSDK.html
                break;
            }
            num = ShipCustomizationPanel.GetSelectedPart(type);
            
        }
		*/
		dict.Add("Turret", Achievements.GetProgress().currentTurret);
		dict.Add("Body", Achievements.GetProgress().currentBody);
		AnalyticsEvent.Custom("ShipPartChoice", dict);
    }
	
}
