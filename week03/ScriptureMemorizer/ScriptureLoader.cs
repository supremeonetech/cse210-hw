using System.Collections.Generic;

public static class ScriptureLoader
{
    public static List<Scripture> LoadScriptures()
    {
        return new List<Scripture>
        {
            new Scripture(new Reference("John", 3, 16, 16), "For God so loved the world that he gave his one and only Son"),
            new Scripture(new Reference("Proverbs", 3, 5, 5), "Trust in the Lord with all your heart and lean not on your own understanding"),
            new Scripture(new Reference("Proverbs", 3, 5, 6), "In all your ways submit to him and he will make your paths straight")
        };
    }
}
