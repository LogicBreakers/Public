using API;

namespace MasterHunter
{
    // Class necessary to be recognized by the client
    public class Entry
    {
        public static void Init()
        {
            // Runs & Adds the HunterBot class to the main handler 
            Handler.GameObject.AddComponent<HunterBot>();
        }
    }
}
