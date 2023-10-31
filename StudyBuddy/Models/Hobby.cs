namespace StudyBuddy.Models;

public class Hobby
{
    public Hobby(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}
