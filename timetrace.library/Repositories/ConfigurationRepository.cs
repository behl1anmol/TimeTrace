using timetrace.library.Context;
using timetrace.library.Models;

namespace timetrace.library.Repositories;
public class ConfigurationRepository(DatabaseContext dbContext) : RepositoryBase(dbContext), IConfigurationRepository
{
    #region Insert
    public ConfigurationSetting AddConfigurationSetting(ConfigurationSetting configurationSetting)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingIndex.Equals(configurationSetting.ConfigurationSettingIndex));

        if (res != null)
        {
            return res;
        }

        var entity = base.Add(configurationSetting);
        AddDefaultConfigurationSettingDetail(entity.ConfigurationSettingId);
        return entity;
    }
    /// <summary>
    /// Adds a default configuration setting detail for the specified index. The default configuration setting detail will have the Key as "Default" and Value as "null"
    /// </summary>
    /// <param name="configurationSettingId"></param>
    private void AddDefaultConfigurationSettingDetail(int configurationSettingId)
    {
        base.Add(new ConfigurationSettingDetail
        {
            ConfigurationSettingId = configurationSettingId,
            ConfigurationSettingKey = "Default",
            ConfigurationSettingValue = null
        });
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
        AddDefaultConfigurationSettingDetail(entity.ConfigurationSettingId);
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
    #endregion

    #region Delete
    public bool DeleteConfigurationSettingDetail(string index, string key)
    {
        var res = base.Find<ConfigurationSettingDetail>(csd => csd.ConfigurationSetting.ConfigurationSettingIndex == index && csd.ConfigurationSettingKey == key);
        if (res == null)
        {
            throw new Exception("Configuration Setting Detail does not exist.");
        }
        base.Delete(res);
        return true;
    }
    public void DeleteConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail)
    {
        base.Delete(configurationSettingDetail);
    }
    public bool DeleteConfigurationSettingIndex(string index)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingIndex == index);
        if (res == null)
        {
            throw new Exception("Configuration Setting does not exist.");
        }
        base.Delete(res);
        return true;
    }
    public void DeleteConfigurationSetting(ConfigurationSetting configurationSetting)
    {
        base.Delete(configurationSetting);
    }
    #endregion

    #region Fetch
    public List<string> FetchAllConfigurationSettingIndexes() => base.FetchAll<ConfigurationSetting>().Select(cs => cs.ConfigurationSettingIndex).ToList();
    public Dictionary<string, string?> FetchConfigurationSettingKeyValueByIndex(string index)
    {
        var entities = base.FindAll<ConfigurationSettingDetail>(csd => csd.ConfigurationSetting.ConfigurationSettingIndex == index)
                                                 .ToDictionary(csd => csd.ConfigurationSettingKey, csd => csd.ConfigurationSettingValue);
        return entities;
    }
    public string? FetchConfigurationSettingValueByKeyIndex(string index, string key)
    {
        var entity = base.Find<ConfigurationSettingDetail>(csd => csd.ConfigurationSetting.ConfigurationSettingIndex == index && csd.ConfigurationSettingKey == key);
        return entity?.ConfigurationSettingValue;


    }
    #endregion

    #region Update
    public ConfigurationSetting UpdateConfigurationSetting(ConfigurationSetting configurationSetting)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingId == configurationSetting.ConfigurationSettingId);
        if (res == null)
        {
            throw new Exception("Configuration Setting does not exist.");
        }
        var entity = base.Update(configurationSetting);
        return entity;

    }
    public ConfigurationSetting UpdateConfigurationSettingIndex(string index, string newIndex)
    {
        var res = base.Find<ConfigurationSetting>(cs => cs.ConfigurationSettingIndex == index);
        if (res == null)
        {
            throw new Exception("Configuration Setting does not exist.");
        }
        res.ConfigurationSettingIndex = newIndex;
        var entity = base.Update(res);
        return entity;
    }
    public ConfigurationSettingDetail UpdateConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail)
    {
        var res = base.Find<ConfigurationSettingDetail>(csd => csd.ConfigurationSettingDetailId == configurationSettingDetail.ConfigurationSettingDetailId);
        if (res == null)
        {
            throw new Exception("Configuration Setting Detail does not exist.");
        }
        var entity = base.Update(configurationSettingDetail);
        return entity;
    }
    public ConfigurationSettingDetail UpdateConfigurationSettingDetailKey(string index, string newKey)
    {
        var res = base.Find<ConfigurationSettingDetail>(csd => csd.ConfigurationSetting.ConfigurationSettingIndex == index);
        if (res == null)
        {
            throw new Exception("Configuration Setting Detail does not exist.");
        }
        res.ConfigurationSettingKey = newKey;
        var entity = base.Update(res);
        return entity;

    }
    public ConfigurationSettingDetail UpdateConfigurationSettingDetailKeyValue(string index, string key, string newValue)
    {
        var res = base.Find<ConfigurationSettingDetail>(csd => csd.ConfigurationSetting.ConfigurationSettingIndex == index && csd.ConfigurationSettingKey == key);
        if (res == null)
        {
            throw new Exception("Configuration Setting Detail does not exist.");
        }
        res.ConfigurationSettingValue = newValue;
        var entity = base.Update(res);
        return entity;
    }
    #endregion
}
