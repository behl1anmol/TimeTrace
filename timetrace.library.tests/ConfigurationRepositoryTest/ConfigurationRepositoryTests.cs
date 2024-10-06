using Microsoft.EntityFrameworkCore;
using timetrace.library.Context;
using timetrace.library.Models;
using timetrace.library.Repositories;
using timetrace.library.tests.TestHelpers;

namespace timetrace.library.tests.ConfigurationRepositoryTest;

[ConfigurationRepositoryCategory]
[TestFixture]
public class ConfigurationRepositoryTests
{
    private DatabaseContext _dbContext;
    private ConfigurationRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite("DataSource=:memory:")
            .EnableSensitiveDataLogging()
            .Options;

        _dbContext = new DatabaseContext(options);
        _dbContext.Database.OpenConnection();
        _dbContext.Database.EnsureCreated();
        _repository = new ConfigurationRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.CloseConnection();
        _dbContext.Dispose();
    }

    [Test, CustomizedAutoData]
    public void AddConfigurationSetting_ShouldAddNewSetting(ConfigurationSetting configurationSetting)
    {
        // Act
        var result = _repository.AddConfigurationSetting(configurationSetting);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(configurationSetting.ConfigurationSettingIndex, Is.EqualTo(result.ConfigurationSettingIndex));
        Assert.That(1, Is.EqualTo(_dbContext.ConfigurationSettings.Count()));
    }

    [Test, CustomizedAutoData]
    public void AddConfigurationSetting_ShouldReturnExistingSetting(ConfigurationSetting existingSetting)
    {
        // Arrange
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.AddConfigurationSetting(existingSetting);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(existingSetting.ConfigurationSettingIndex, Is.EqualTo(result.ConfigurationSettingIndex));
        Assert.That(1, Is.EqualTo(_dbContext.ConfigurationSettings.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteConfigurationSettingIndex_ShouldDeleteSetting(string index, ConfigurationSetting existingSetting)
    {
        // Arrange
        existingSetting.ConfigurationSettingIndex = index;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.DeleteConfigurationSettingIndex(index);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.ConfigurationSettings.Count()));
    }

    [Test, CustomizedAutoData]
    public void FetchAllConfigurationSettingIndexes_ShouldReturnIndexes(List<ConfigurationSetting> settings)
    {
        // Arrange
        _dbContext.ConfigurationSettings.AddRange(settings);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.FetchAllConfigurationSettingIndexes();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(settings.Count, Is.EqualTo(result.Count));
    }
    [Test, CustomizedAutoData]
    public void AddConfigurationSettingDetail_ShouldAddNewDetail(string index, string key, string value)
    {
        // Act
        var result = _repository.AddConfigurationSettingDetail(index, key, value);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(key, Is.EqualTo(result.ConfigurationSettingKey));
        Assert.That(value, Is.EqualTo(result.ConfigurationSettingValue));
        Assert.That(2, Is.EqualTo(_dbContext.ConfigurationSettingDetails.Count()));
        Assert.That(result, Is.EqualTo(
            _dbContext.ConfigurationSettingDetails
                .Where(csd=>csd.ConfigurationSettingKey.Equals(key)).FirstOrDefault()));
        Assert.That(_dbContext.ConfigurationSettings.ToList().Exists(cs=>cs.ConfigurationSettingIndex.Equals(index)), Is.True);
        Assert.That(_dbContext.ConfigurationSettingDetails.ToList()
            .Exists(csd=>csd.ConfigurationSettingKey.Equals("Default") 
                         && csd.ConfigurationSettingValue == null), Is.True);
    }

    [Test, CustomizedAutoData]
    public void AddConfigurationSettingDetail_ShouldThrowExceptionIfSettingIdNotExist(ConfigurationSettingDetail detail)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.AddConfigurationSettingDetail(detail));
        Assert.That(ex.Message, Is.EqualTo("ConfigurationSettingId does not exist in the ConfigurationSetting table."));
    }

    [Test, CustomizedAutoData]
    public void DeleteConfigurationSettingDetail_ShouldDeleteDetail(string index, string key, ConfigurationSetting existingSetting, ConfigurationSettingDetail existingDetail)
    {
        // Arrange
        existingSetting.ConfigurationSettingIndex = index;
        existingDetail.ConfigurationSettingKey = key;
        existingDetail.ConfigurationSettingId = existingSetting.ConfigurationSettingId;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.ConfigurationSettingDetails.Add(existingDetail);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.DeleteConfigurationSettingDetail(index, key);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(0, Is.EqualTo(_dbContext.ConfigurationSettingDetails.Count()));
    }

    [Test, CustomizedAutoData]
    public void DeleteConfigurationSettingDetail_ShouldThrowExceptionIfDetailNotExist(string index, string key)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.DeleteConfigurationSettingDetail(index, key));
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting Detail does not exist."));
    }

    [Test, CustomizedAutoData]
    public void FetchConfigurationSettingKeyValueByIndex_ShouldReturnKeyValuePairs(string index, List<ConfigurationSettingDetail> details)
    {
        // Arrange
        var res = _repository.AddConfigurationSetting(index);
        foreach (var detail in details)
        {
            detail.ConfigurationSettingId = res.ConfigurationSettingId;
            _dbContext.ConfigurationSettingDetails.Add(detail);
        }
        _dbContext.SaveChanges();

        // Act
        var result = _repository.FetchConfigurationSettingKeyValueByIndex(index);

        // Assert
        Assert.That(result, Is.Not.Null);
        //+1 because of the default detail
        Assert.That((details.Count+1), Is.EqualTo(result.Count));
    }

    [Test, CustomizedAutoData]
    public void FetchConfigurationSettingValueByKeyIndex_ShouldReturnValue(string index, string key, string value)
    {
        // Arrange
        _repository.AddConfigurationSettingDetail(index, key, value);


        // Act
        var result = _repository.FetchConfigurationSettingValueByKeyIndex(index, key);

        // Assert
        Assert.That(result, Is.EqualTo(value));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSetting_ShouldUpdateSetting(ConfigurationSetting existingSetting, string newIndex)
    {
        // Arrange
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.SaveChanges();
        existingSetting.ConfigurationSettingIndex = newIndex;

        // Act
        var result = _repository.UpdateConfigurationSetting(existingSetting);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(newIndex, Is.EqualTo(result.ConfigurationSettingIndex));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingIndex_ShouldUpdateIndex(string oldIndex, string newIndex, ConfigurationSetting existingSetting)
    {
        // Arrange
        existingSetting.ConfigurationSettingIndex = oldIndex;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.UpdateConfigurationSettingIndex(oldIndex, newIndex);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(newIndex, Is.EqualTo(result.ConfigurationSettingIndex));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingDetail_ShouldUpdateDetail(ConfigurationSetting existingSetting, ConfigurationSettingDetail existingDetail, string newValue)
    {
        // Arrange
        existingDetail.ConfigurationSettingId = existingSetting.ConfigurationSettingId;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.ConfigurationSettingDetails.Add(existingDetail);
        _dbContext.SaveChanges();
        existingDetail.ConfigurationSettingValue = newValue;

        // Act
        var result = _repository.UpdateConfigurationSettingDetail(existingDetail);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(newValue, Is.EqualTo(result.ConfigurationSettingValue));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingDetailKey_ShouldUpdateKey(string index, string newKey, ConfigurationSetting existingSetting, ConfigurationSettingDetail existingDetail)
    {
        // Arrange
        existingSetting.ConfigurationSettingIndex = index;
        existingDetail.ConfigurationSettingId = existingSetting.ConfigurationSettingId;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.ConfigurationSettingDetails.Add(existingDetail);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.UpdateConfigurationSettingDetailKey(index, newKey);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(newKey, Is.EqualTo(result.ConfigurationSettingKey));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingDetailKeyValue_ShouldUpdateKeyValue(string index, string key, string newValue, ConfigurationSetting existingSetting, ConfigurationSettingDetail existingDetail)
    {
        // Arrange
        existingSetting.ConfigurationSettingIndex = index;
        existingDetail.ConfigurationSettingKey = key;
        existingDetail.ConfigurationSettingId = existingSetting.ConfigurationSettingId;
        _dbContext.ConfigurationSettings.Add(existingSetting);
        _dbContext.ConfigurationSettingDetails.Add(existingDetail);
        _dbContext.SaveChanges();

        // Act
        var result = _repository.UpdateConfigurationSettingDetailKeyValue(index, key, newValue);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(newValue, Is.EqualTo(result.ConfigurationSettingValue));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingDetailKeyValue_ShouldThrowExceptionIfDetailNotExist(string index, string key, string newValue)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.UpdateConfigurationSettingDetailKeyValue(index, key, newValue));
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting Detail does not exist."));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingDetailKey_ShouldThrowExceptionIfDetailNotExist(string index, string newKey)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.UpdateConfigurationSettingDetailKey(index, newKey));
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting Detail does not exist."));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSettingIndex_ShouldThrowExceptionIfSettingNotExist(string oldIndex, string newIndex)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.UpdateConfigurationSettingIndex(oldIndex, newIndex));
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting does not exist."));
    }

    [Test, CustomizedAutoData]
    public void UpdateConfigurationSetting_ShouldThrowExceptionIfSettingNotExist(ConfigurationSetting setting)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.UpdateConfigurationSetting(setting));
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting does not exist."));
    }

    [Test, CustomizedAutoData]
    public void DeleteConfigurationSettingIndex_ShouldReturnFalseIfSettingNotExist(string index)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.DeleteConfigurationSettingIndex(index));
        // Assert
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting does not exist."));
    }

    [Test, CustomizedAutoData]
    public void DeleteConfigurationSettingDetail_ShouldReturnFalseIfSettingNotExist(string index, string key)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.DeleteConfigurationSettingDetail(index, key));
        // Assert
        Assert.That(ex.Message, Is.EqualTo("Configuration Setting Detail does not exist."));
    }
    [Test, CustomizedAutoData]
    public void FetchConfigurationSettingKeyValueByIndex_ShouldReturnEmptyListIfSettingNotExist(string index)
    {
        // Act
        var result = _repository.FetchConfigurationSettingKeyValueByIndex(index);
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(0, Is.EqualTo(result.Count));
    }
    [Test, CustomizedAutoData]
    public void FetchConfigurationSettingValueByKeyIndex_ShouldReturnNullIfSettingNotExist(string index, string key)
    {
        // Act
        var result = _repository.FetchConfigurationSettingValueByKeyIndex(index, key);
        // Assert
        Assert.That(result, Is.Null);
    }
    [Test, CustomizedAutoData]
    public void FetchAllConfigurationSettingIndexes_ShouldReturnEmptyListIfSettingNotExist()
    {
        // Act
        var result = _repository.FetchAllConfigurationSettingIndexes();
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(0, Is.EqualTo(result.Count));
    }
    [Test, CustomizedAutoData]
    public void AddConfigurationSettingDetail_ShouldThrowExceptionIfConfigurationSettingIdSettingNotExist(ConfigurationSettingDetail existingSetting)
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _repository.AddConfigurationSettingDetail(existingSetting));
        Assert.That(ex.Message, Is.EqualTo("ConfigurationSettingId does not exist in the ConfigurationSetting table."));
    }

}