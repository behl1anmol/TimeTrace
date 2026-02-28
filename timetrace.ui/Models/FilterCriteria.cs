namespace timetrace.ui.Models;

/// <summary>
/// Immutable filter criteria for image date-range filtering.
/// NOTE: Backend IProcessRepository.GetProcessDetailsByDateRange(startDate, endDate, page, pageSize)
///       aligns to FromDate/ToDate.
/// </summary>
public record FilterCriteria(
    DateTime? FromDate,
    DateTime? ToDate
);
