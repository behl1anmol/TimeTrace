using timetrace.library.Models;

namespace timetrace.library.Repositories;

/// <summary>
/// This interface is used to define the methods that are required to interact with the 
/// ConfigurationSetting and ConfigurationSettingDetail tables in the database.
/// </summary>
public interface IConfigurationRepository : IRepositoryBase
{
    /// <summary>
    /// Adds a new configuration setting to the database. If the configuration setting is already present this functions returns the existing configuration setting.
    /// </summary>
    /// <param name="configurationSetting">The configuration setting to add.</param>
    /// <returns>The added configuration setting.</returns>
    ConfigurationSetting AddConfigurationSetting(ConfigurationSetting configurationSetting);

    /// <summary>
    /// Adds a new configuration setting to the database. If the configuration setting is already present this functions returns the existing configuration setting.
    /// </summary>
    /// <param name="index">The configuration setting index to add.</param>
    /// <returns>The added configuration setting.</returns>
    ConfigurationSetting AddConfigurationSetting(string index);

    /// <summary>
    /// Fetches all the configuration setting indexes from the database.
    /// </summary>
    /// <returns>A list of configuration setting indexes.</returns>
    List<ConfigurationSetting> FetchAllConfigurationSettingIndexes();

    /// <summary>
    /// Deletes a configuration setting index from the database.
    /// </summary>
    /// <param name="index">The index of the configuration setting to delete.</param>
    /// <returns>True if the configuration setting index was deleted successfully, otherwise false.</returns>
    bool DeleteConfigurationSettingIndex(string index);

    /// <summary>
    /// Updates a configuration setting in the database.
    /// </summary>
    /// <param name="configurationSetting">The configuration setting to update.</param>
    /// <returns>The updated configuration setting.</returns>
    ConfigurationSetting UpdateConfigurationSetting(ConfigurationSetting configurationSetting);

    /// <summary>
    /// Adds a default configuration setting detail for the specified index. The default configuration setting detail will have the Key as "Default" and Value as "null"
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <returns>The added default configuration setting detail.</returns>
    ConfigurationSettingDetail AddDefaultConfigurationSettingDetail(string index);

    /// <summary>
    /// Adds a configuration setting detail with the specified key and value for the specified index. If the configuration setting with the index is not present, it will be added.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <param name="key">The key of the configuration setting detail.</param>
    /// <param name="value">The value of the configuration setting detail.</param>
    /// <returns>The added configuration setting detail.</returns>
    ConfigurationSettingDetail AddConfigurationSettingDetail(string index, string key, string value);

    /// <summary>
    /// Adds a configuration setting detail to the database.
    /// </summary>
    /// <param name="configurationSettingDetail">The configuration setting detail to add.</param>
    /// <returns>The added configuration setting detail.</returns>
    ConfigurationSettingDetail AddConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail);

    /// <summary>
    /// Fetches the configuration setting detail value by the specified key and index.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <param name="key">The key of the configuration setting detail.</param>
    /// <returns>The configuration setting detail with the specified key and index.</returns>
    ConfigurationSettingDetail FetchConfigurationSettingValueByKeyIndex(string index, string key);

    /// <summary>
    /// Fetches the configuration setting detail key-value pair by the specified index.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <returns>A key-value pair for the specified index.</returns>
    KeyValuePair<string?,string?> FetchConfigurationSettingKeyValueByIndex(string index);

    /// <summary>
    /// Updates a configuration setting detail in the database.
    /// </summary>
    /// <param name="configurationSettingDetail">The configuration setting detail to update.</param>
    /// <returns>The updated configuration setting detail.</returns>
    ConfigurationSettingDetail UpdateConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail);

    /// <summary>
    /// Updates the value of a configuration setting detail with the specified key and index.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <param name="key">The key of the configuration setting detail.</param>
    /// <param name="newValue">The new value for the configuration setting detail.</param>
    /// <returns>The updated configuration setting detail.</returns>
    ConfigurationSettingDetail UpdateConfigurationSettingDetailKeyValue(string index, string key, string newValue);

    /// <summary>
    /// Updates the key of a configuration setting detail with the specified index.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <param name="newKey">The new key for the configuration setting detail.</param>
    /// <returns>The updated configuration setting detail.</returns>
    ConfigurationSettingDetail UpdateConfigurationSettingDetailKey(string index, string newKey);

    /// <summary>
    /// Deletes a configuration setting detail with the specified key and index from the database.
    /// </summary>
    /// <param name="index">The index of the configuration setting.</param>
    /// <param name="key">The key of the configuration setting detail.</param>
    /// <returns>True if the configuration setting detail was deleted successfully, otherwise false.</returns>
    bool DeleteConfigurationSettingDetail(string index, string key);

    /// <summary>
    /// Deletes a configuration setting detail from the database.
    /// </summary>
    /// <param name="configurationSettingDetail">The configuration setting detail to delete.</param>
    /// <returns>True if the configuration setting detail was deleted successfully, otherwise false.</returns>
    bool DeleteConfigurationSettingDetail(ConfigurationSettingDetail configurationSettingDetail);
}
