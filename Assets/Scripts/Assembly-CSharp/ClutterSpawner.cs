using GameCore;
using System.Collections.Generic;
using UnityEngine;

public class ClutterSpawner : MonoBehaviour
{
    [SerializeField]
    private List<ClutterStruct> clutters = new List<ClutterStruct>();

    private static bool noHolidays;

    public static bool IsHolidayActive(Holidays holiday)
    {
        System.DateTime now = System.DateTime.UtcNow;
        if (noHolidays && holiday == Holidays.NoHoliday)
            return true;

        return holiday switch
        {
            Holidays.Always => true,
            Holidays.AprilFools => (now.Day == 1 && now.Month == 4) && !noHolidays,
            Holidays.FirstHalfOfApril => (now.Day <= 15 && now.Month == 4) && !noHolidays,
            Holidays.October => (now.Month == 10) && !noHolidays,
            Holidays.December => (now.Month == 12) && !noHolidays,
            Holidays.OctoberOrDecember => (now.Month == 10 || now.Month == 12) && !noHolidays,
            Holidays.Christmas => (now.Day >= 24 && now.Month == 12) && !noHolidays,
            Holidays.Halloween => ((now.Day >= 28 && now.Month == 10) || (now.Day <= 3 && now.Month == 11)) && !noHolidays,
            Holidays.NoHoliday => true,
            _ => false
        };
    }

    private void Awake()
    {
        MapGeneration.SeedSynchronizer.OnMapGenerated += GenerateClutter;
    }

    private void OnDestroy()
    {
        MapGeneration.SeedSynchronizer.OnMapGenerated -= GenerateClutter;
    }

    private void Start()
    {
        noHolidays = ConfigFile.ServerConfig.GetBool("no_holidays", false);
    }

    public void GenerateClutter()
    {

        for (int num = clutters.Count - 1; num >= 0; num--)
        {
            ClutterStruct clutterData = clutters[num];

            /*
            GameCore.Console.AddDebugLog("MGCLTR",
                $"Checking spawn conditions for clutter struct \"{clutterData.descriptor}\" on object \"{gameObject.name}\"",
                MessageImportance.LeastImportant, true);
            */

            if (clutterData.clutterComponent == null || clutterData.clutterComponent.spawned)
                continue;

            bool spawn = true;

            if (clutterData.chanceToSpawn <= 0f)
            {
                spawn = false;
            }
            else if ((float)UnityEngine.Random.Range(1, 101) > clutterData.chanceToSpawn)
            {
                spawn = false;
            }
            else if (!clutterData.invertTimespan)
            {
                bool anyActive = false;
                foreach (Holidays h in clutterData.validTimespan)
                {
                    if (IsHolidayActive(h))
                        anyActive = true;
                }
                spawn = anyActive;
            }
            else
            {
                foreach (Holidays h in clutterData.validTimespan)
                {
                    if (IsHolidayActive(h))
                        spawn = false;
                }
            }

            if (spawn)
            {
                clutterData.clutterComponent.SpawnClutter();
            }
            else
            {
                clutterData.clutterComponent.gameObject.SetActive(false);
                UnityEngine.Object.Destroy(clutterData.clutterComponent.holderObject);
            }
        }
    }
}