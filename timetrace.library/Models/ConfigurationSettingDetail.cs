using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace timetrace.library.Models;

[Table(nameof(ConfigurationSettingDetail))]
public class ConfigurationSettingDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConfigurationSettingDetailId
    {
        get; set;
    }

    public string? ConfigurationSettingValue
    {
        get; set;
    }

    public string? ConfigurationSettingKey
    {
        get; set;
    }

    [ForeignKey(nameof(ConfigurationSettingId))]
    public ConfigurationSetting ConfigurationSetting
    {
        get; set;
    }

    public int ConfigurationSettingId
    {
        get; set;
    }

    [Timestamp]
    public DateTime DateTimeStamp
    {
        get; set;
    }

}
