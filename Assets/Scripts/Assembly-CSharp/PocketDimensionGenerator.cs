public class PocketDimensionGenerator : global::UnityEngine.MonoBehaviour
{
    private static global::System.Random _random;

    public void GenerateMap(int seed)
    {
        global::UnityEngine.Random.InitState(seed);
        if (global::Mirror.NetworkServer.active)
        {
            _random = Misc.CreateRandom();
            GenerateRandom();
        }
    }

    public void GenerateRandom()
    {
        PocketDimensionTeleport[] array = PrepTeleports();
        for (int i = 0; i < global::GameCore.ConfigFile.ServerConfig.GetInt("pd_exit_count", 2); i++)
        {
            if (!ContainsKiller(array))
            {
                break;
            }
            int num = -1;
            while ((num < 0 || array[num].GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Exit) && ContainsKiller(array))
            {
                num = _random.Next(0, array.Length);
            }
            array[global::UnityEngine.Mathf.Clamp(num, 0, array.Length - 1)].SetType(PocketDimensionTeleport.PDTeleportType.Exit);
        }
    }

    private static PocketDimensionTeleport[] PrepTeleports()
    {
        PocketDimensionTeleport[] array = global::UnityEngine.Object.FindObjectsOfType<PocketDimensionTeleport>();
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetType(PocketDimensionTeleport.PDTeleportType.Killer);
        }
        return array;
    }

    private static bool ContainsKiller(PocketDimensionTeleport[] pdtps)
    {
        for (int i = 0; i < pdtps.Length; i++)
        {
            if (pdtps[i].GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Killer)
            {
                return true;
            }
        }
        return false;
    }
}
