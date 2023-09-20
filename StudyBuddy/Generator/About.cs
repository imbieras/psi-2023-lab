using System.Runtime.CompilerServices;
using StudyBuddy.Extensions;
namespace StudyBuddy.Generator;

public static class About
{
    private static Random random = new Random();    
    public static string NameGenerator()
    {
        string[] words = {"Liam", "Tom", "Noah", "James", "William", "Lucas",
        "Olivia", "Emma", "Charlotte", "Sophia", "Mia", "Evelyn"};
        return words[random.Next(0, words.Length)];
    }
    public static string SurnameGenerator()
    {

        string[] words = {"Smith", "Johnson", "Williams", "Brown", "Jones",
        "Miller", "Lopez", "Martinez", "Rodriguez", "Gonzales"};

        int randomIndex = random.Next(0, words.Length);
        string randomSurname = words[randomIndex];
        string surnameWithPossessive = randomSurname.AddPossessiveSuffix();

        return surnameWithPossessive;
    }

    public static DateTime DateGenerator()
    {
        
        int year = random.Next(1950, DateTime.Now.Year + 1); // Random year between 1900 and current year
        int month = random.Next(1, 13); // Random month between 1 and 12
        int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
        DateTime randomDateTime = new DateTime(year, month, day);

        return randomDateTime;
    }

    public static string SubjectGenerator()
    {
        string[] words = {"Natural Sciences", "Health Sciences", "Engineering & Technology",
        "Social Sciences", "Humanities", "Business & Management", "Arts & Design",
        "Mathematical Sciences", "Bio Sciences", "Law & Legal Studies", "Agriculture & Forestry"};
        return words[random.Next(0, words.Length)];
    }
  
    
}
