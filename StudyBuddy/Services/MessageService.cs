namespace StudyBuddy.Services;

public class MessageService
{
    private readonly Dictionary<string, List<string>> messages = new Dictionary<string, List<string>>();

    public void AddMessage(string groupName, string message)
    {
        if (!messages.ContainsKey(groupName))
        {
            messages[groupName] = new List<string>();
        }

        messages[groupName].Add(message);
    }

    public List<string> GetMessages(string groupName)
    {
        if (messages.ContainsKey(groupName))
        {
            return messages[groupName];
        }

        return new List<string>();
    }
}
