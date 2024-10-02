using timetrace.library.Context;
using timetrace.library.Models;

namespace timetrace.library.Repositories;
public class ConfigurationRepository(DatabaseContext dbContext) : RepositoryBase(dbContext), IConfigurationRepository
{
    public ConfigurationSetting AddConfigurationSetting(ConfigurationSetting configurationSetting)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingIndex.Equals(configurationSetting.ConfigurationSettingIndex));

        if (res != null)
        {
            return res;
        }
        var entity = base.Add(configurationSetting);
        return entity;
    }
    public ConfigurationSetting AddConfigurationSetting(string index)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingIndex == index);

        if (res != null)
        {
            return res;
        }
        var entity = base.Add(new ConfigurationSetting
        {
            ConfigurationSettingIndex = index
        });
        return entity;
    }
    public ConfigurationSettingDetail AddConfigurationSettingDetail(string index, string key, string value)
    {
        var res = this.AddConfigurationSetting(index);

        var entity = base.Add(new ConfigurationSettingDetail
        {
            ConfigurationSettingId = res.ConfigurationSettingId,
            ConfigurationSettingKey = key,
            ConfigurationSettingValue = value
        });
        return entity;
    }
    public ConfigurationSettingDetail AddConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail)
    {
        var res = base.Exists<ConfigurationSetting>(cs => cs.ConfigurationSettingId == configurationSettingDetail.ConfigurationSettingId);
        if (res != true)
        {
            throw new Exception("ConfigurationSettingId does not exist in the ConfigurationSetting table.");
        }
        var entity = base.Add(configurationSettingDetail);
        return entity;
    }
    public ConfigurationSettingDetail AddDefaultConfigurationSettingDetail(string index) => throw new NotImplementedException(); //shall i make it private?
    public bool DeleteConfigurationSettingDetail(string index, string key) => throw new NotImplementedException();
    public bool DeleteConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail) => throw new NotImplementedException();
    public bool DeleteConfigurationSettingIndex(string index) => throw new NotImplementedException();
    public List<ConfigurationSetting> FetchAllConfigurationSettingIndexes() => throw new NotImplementedException();
    public KeyValuePair<string?, string?> FetchConfigurationSettingKeyValueByIndex(string index) => throw new NotImplementedException();
    public ConfigurationSettingDetail FetchConfigurationSettingValueByKeyIndex(string index, string key) => throw new NotImplementedException();
    public ConfigurationSetting UpdateConfigurationSetting(ConfigurationSetting configurationSetting) => throw new NotImplementedException();
    public ConfigurationSettingDetail UpdateConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail) => throw new NotImplementedException();
    public ConfigurationSettingDetail UpdateConfigurationSettingDetailKey(string index, string newKey) => throw new NotImplementedException();
    public ConfigurationSettingDetail UpdateConfigurationSettingDetailKeyValue(string index, string key, string newValue) => throw new NotImplementedException();
}
