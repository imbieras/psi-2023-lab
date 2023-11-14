namespace StudyBuddy.Utilities;

public static class ConversationIdHelper
{
    public static Guid GetGroupId(Guid senderGuid, Guid receiverGuid)
    {
        // Sort user IDs to ensure consistent group names regardless of user roles
        var guids = senderGuid.CompareTo(receiverGuid) < 0
            ? (senderGuid, receiverGuid)
            : (receiverGuid, senderGuid);

        byte[] bytes1 = guids.Item1.ToByteArray();
        byte[] bytes2 = guids.Item2.ToByteArray();

        for (int i = 0; i < bytes1.Length; i++)
        {
            bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        return new Guid(bytes1);
    }
}
