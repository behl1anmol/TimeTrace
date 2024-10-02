using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timetrace.library.Models;

[Table(nameof(ProcessDetail))]
public class ProcessDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessDetailId
    {
        get; set;
    }

    public string? Description
    {
        get; set;
    }

    [Timestamp]
    public DateTime DateTimeStamp
    {
        get; set;
    }

    public int ProcessId
    {
        get; set;
    }

    [ForeignKey(nameof(ProcessId))]
    public Process Process
    {
        get; set;
    }

    public ICollection<Image> Images
    {
        get; set;
    }

}
