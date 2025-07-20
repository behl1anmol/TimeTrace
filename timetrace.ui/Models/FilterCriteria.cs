namespace timetrace.ui.Models;

public record FilterCriteria
(
    DateTime? FromDate,
    DateTime? ToDate,
    DateTime? SelectedDate,
    IList<string> Statuses // e.g., Active, Minimized, Closed
);
