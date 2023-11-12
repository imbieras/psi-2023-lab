using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyBuddy.Models;

public class UserAuth
{

    public UserAuth(){}

    [Key, ForeignKey("User")]
    public Guid UserId { get; set; }

    public string PasswordHash { get; set; }
}
