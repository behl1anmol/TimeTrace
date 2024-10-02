using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timetrace.library.Models;

[Table(nameof(ConfigurationSetting))]
public class ConfigurationSetting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConfigurationSettingId
    {
        get; set;
    }

    [Required]
    public string ConfigurationSettingIndex
    {
        get; set;
    }

    [Timestamp]
    public DateTime DateTimeStamp
    {
        get; set;
    }

    public ICollection<ConfigurationSettingDetail> ConfigurationSettingDetails
    {
        get; set;
    }
}
