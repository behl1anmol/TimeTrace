using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timetrace.library.Models;

[Table(nameof(Process))]
public class Process
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessId
    {
        get; set;
    }

    [Required]
    public string Name
    {
        get; set;
    }

    [Timestamp]
    public DateTime DateTimeStamp
    {
        get; set;
    }

    public ICollection<ProcessDetail> ProcessDetails
    {
        get; set;
    }
}
