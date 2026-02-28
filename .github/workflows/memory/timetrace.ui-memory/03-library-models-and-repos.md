# timetrace.library — Models & Repository Interfaces

This file documents the backend library's data models and repository interfaces that the UI must stay compatible with.

---

## Entity Models (EF Core, SQLite)

### Process
```csharp
[Table(nameof(Process))]
public class Process
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessId { get; set; }

    [Required]
    public string Name { get; set; }

    [Timestamp]
    public DateTime DateTimeStamp { get; set; }

    public virtual ICollection<ProcessDetail> ProcessDetails { get; set; }
}
```

### ProcessDetail
```csharp
[Table(nameof(ProcessDetail))]
public class ProcessDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessDetailId { get; set; }

    [Required]
    public string? Description { get; set; }

    [Timestamp]
    public DateTime DateTimeStamp { get; set; }

    public int ProcessId { get; set; }

    [ForeignKey(nameof(ProcessId))]
    public virtual Process Process { get; set; }

    public virtual ICollection<Image> Images { get; set; }
}
```

### Image
```csharp
[Table(nameof(Image))]
public class Image
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImageId { get; set; }

    [Required]
    public string Name { get; set; }

    public string? ImagePath { get; set; }  // null = deleted or save error

    [Timestamp]
    public DateTime DateTimeStamp { get; set; }

    [ForeignKey(nameof(ProcessDetailId))]
    public virtual ProcessDetail ProcessDetail { get; set; }

    public int ProcessDetailId { get; set; }

    [Required]
    public Guid ImageGuid { get; set; }
}
```

### ConfigurationSetting
```csharp
[Table(nameof(ConfigurationSetting))]
public class ConfigurationSetting
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConfigurationSettingId { get; set; }

    [Required]
    public string ConfigurationSettingIndex { get; set; }

    [Timestamp]
    public DateTime DateTimeStamp { get; set; }

    public virtual ICollection<ConfigurationSettingDetail> ConfigurationSettingDetails { get; set; }
}
```

### ConfigurationSettingDetail
```csharp
[Table(nameof(ConfigurationSettingDetail))]
public class ConfigurationSettingDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ConfigurationSettingDetailId { get; set; }

    public string? ConfigurationSettingValue { get; set; }

    public string ConfigurationSettingKey { get; set; }

    [ForeignKey(nameof(ConfigurationSettingId))]
    public virtual ConfigurationSetting ConfigurationSetting { get; set; }

    public int ConfigurationSettingId { get; set; }

    [Timestamp]
    public DateTime DateTimeStamp { get; set; }
}
```

---

## Repository Interfaces

### IRepositoryBase (generic CRUD)
- `Add<TE>`, `AddAll<TE>`, `Update<TE>`, `UpdateAll<TE>`
- `Delete<TE>`, `DeleteAll<TE>` (multiple overloads)
- `Exists<TE>`, `Find<TE>`, `FindAll<TE>` (with pagination)
- `FetchAll<TE>`

### IProcessRepository (extends IRepositoryBase)

**Process CRUD:**
- `AddProcess(Process)`, `AddProcesses(IEnumerable<Process>)`
- `DeleteProcess(Process)`, `DeleteProcess(int id)`
- `DeleteProcesses(IEnumerable<Process>)`, `DeleteAllProcesses()`
- `GetProcess(int id)`, `GetProcessByName(string name)`
- `GetProcesses(int page, int pageSize)`
- `UpdateProcess(Process)`, `UpdateProcesses(IEnumerable<Process>)`

**ProcessDetail CRUD:**
- `AddProcessDetail(ProcessDetail)`, `AddProcessDetails(IEnumerable<ProcessDetail>)`
- `DeleteProcessDetail(ProcessDetail)`, `DeleteProcessDetails(IEnumerable<ProcessDetail>)`
- `DeleteProcessDetailsByProcessId(int processId)`
- `GetProcessDetail(int id)`
- `GetProcessDetails(int processId, int page, int pageSize)`
- `GetProcessDetailsByDateRange(DateTime start, DateTime end, int page, int pageSize)`
- `UpdateProcessDetail(ProcessDetail)`, `UpdateProcessDetails(IEnumerable<ProcessDetail>)`

**Combined:**
- `AddProcessWithDetails(Process, IEnumerable<ProcessDetail>)`
- `GetImagesForProcess(int processId, int page, int pageSize)`

### IImageRepository (extends IRepositoryBase)
- `GetImages(Expression<Func<Image, bool>> predicate, int page, int pageSize)`
- `GetImages(ProcessDetail processDetail, int page, int pageSize)`
- `GetImages(int processDetailId, int page, int pageSize)`
- `GetImages(IEnumerable<int> processDetailIds, int page, int pageSize)`
- `AddImage(Image)`, `AddImages(IEnumerable<Image>)`
- `DeleteImage(Image)`, `DeleteImages(...)`, `DeleteAllImages()`
- `UpdateAllImagePath(string oldPath, string newPath)`

### IConfigurationRepository (extends IRepositoryBase)
- `AddConfigurationSetting(ConfigurationSetting)`
- `FetchAllConfigurationSettingIndexes()`
- `DeleteConfigurationSettingIndex(string)`, `DeleteConfigurationSetting(ConfigurationSetting)`
- `UpdateConfigurationSetting(ConfigurationSetting)`, `UpdateConfigurationSettingIndex(int, string)`
- `AddConfigurationSettingDetail(ConfigurationSettingDetail)`
- `FetchConfigurationSettingValueByKeyIndex(string key, string index)`
- `FetchConfigurationSettingKeyValueByIndex(string index)`
- `UpdateConfigurationSettingDetail(ConfigurationSettingDetail)`
- `UpdateConfigurationSettingDetailKeyValue(int id, string key, string value)`
- `UpdateConfigurationSettingDetailKey(int id, string key)`
- `DeleteConfigurationSettingDetail(ConfigurationSettingDetail)`

---

## Database Configuration
- **Provider**: SQLite via EF Core
- **Connection**: `Data Source={LocalAppData}/timetrace.db;Password=Password12!`
- **Extension**: `ServiceCollectionExtensions.AddDatabaseContextFactory()` registers `DatabaseContext`

## Config Constants
```csharp
public static class ConfigSettingConstants
{
    public const string FilePathConfigSettingIndex = "Paths";
    public const string ImagePathConfigSettingKey = "ImagePath";
}
```

---

## UI ↔ Library Model Mapping Guide

| UI Model Field | Library Entity | Library Field | Notes |
|---------------|----------------|---------------|-------|
| `ApplicationModel.Id` | `Process` | `ProcessId` | int PK |
| `ApplicationModel.ProcessName` | `Process` | `Name` | string |
| `ApplicationModel.CaptureCount` | computed | Count of `Image` via `ProcessDetail` | Join Process → ProcessDetails → Images |
| `ApplicationModel.LastCaptured` | computed | Max `Image.DateTimeStamp` | Latest image timestamp |
| `ApplicationModel.Status` | N/A | Not in library | UI-only concept (Active/Minimized/etc.) |
| `ApplicationModel.ProcessId` | N/A | OS PID | Not stored in library |
| `ApplicationModel.IconPath` | N/A | Not in library | UI-only (emoji or icon) |
| `CapturedImageModel.Id` | `Image` | `ImageId` | int PK |
| `CapturedImageModel.ApplicationId` | `ProcessDetail` | `ProcessId` | FK via ProcessDetail |
| `CapturedImageModel.ScreenshotName` | `Image` | `Name` | string |
| `CapturedImageModel.FilePath` | `Image` | `ImagePath` | nullable string |
| `CapturedImageModel.Timestamp` | `Image` | `DateTimeStamp` | DateTime |
| `CapturedImageModel.ImageGuid` | `Image` | `ImageGuid` | Guid (to add to UI model) |
| Settings fields | `ConfigurationSettingDetail` | `Key` / `Value` | Key-value pairs grouped by `ConfigurationSetting.Index` |
