using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timetrace.library.Models;

[Table(nameof(Image))]
public class Image
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImageId
    {
        get; set;
    }

    [Required]
    public string Name
    {
        get; set;
    }

    [Required]
    public string ImagePath
    {
        get; set;
    }

    [Timestamp]
    public DateTime DateTimeStamp
    {
        get; set;
    }

    [ForeignKey(nameof(ProcessDetailId))]
    public virtual ProcessDetail ProcessDetail
    {
        get; set;
    }

    public int ProcessDetailId
    {
        get; set;
    }

}
