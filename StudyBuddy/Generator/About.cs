using System.Runtime.CompilerServices;

namespace StudyBuddy.Generator;

public static class About
{
    public static string NameGenerator()
    {
        Random r = new Random();
        string[] words = {"Liam", "Tom", "Noah", "James", "William", "Lucas",
        "Olivia", "Emma", "Charlotte", "Sophia", "Mia", "Evelyn"};
        return words[r.Next(0, words.Length)];
    }
    public static string SurnameGenerator()
    {
        Random r = new Random();
        string[] words = {"Smith", "Johnson", "williams", "Brown", "Jones",
        "Miller", "Lopez", "Martinez", "Rodriguez", "Gonzales"};
        return words[r.Next(0, words.Length)];
    }

    public static int AgeGenerator()
    {
        Random rnd = new Random();
        return rnd.Next(18, 99);
      //DateTime datetime = DateTime.Now;
      //return DateOnly.FromDateTime(DateTime.Now);
    }

    public static string SubjectGenerator()
    {
        Random r = new Random();
        string[] words = {"Natural Sciences", "Health Sciences", "Engineering & Technology",
        "Social Sciences", "Humanities", "Business & Management", "Arts & Design",
        "Mathematical Sciences", "Bio Sciences", "Law & Legal Studies", "Agriculture & Forestry"};
        return words[r.Next(0, words.Length)];
    }
  
    
}
