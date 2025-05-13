namespace DevRoutine.Api.Entities;

public sealed class Routine
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RoutineType Type { get; set; }
    public Frequency Frequency { get; set; }
    public Target Target { get; set; }
    public RoutineStatus Status { get; set; }
    public bool IsArchived { get; set; }
    public DateOnly? EndDate { get; set; }
    public Milestone? Milestone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastCompletedAt { get; set; }
    
}

public enum RoutineType
{
    None = 0,
    Binary = 1,
    Measurable = 2
}

public enum RoutineStatus
{
    None = 0,
    Ongoing = 1,
    Completed = 2
}

public sealed class Frequency
{
    public FrequencyType Type { get; set; }
    public int TimesPerPeriod { get; set; }
}

public enum FrequencyType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3
}

public sealed class Target
{
    public int Value { get; set; }
    public string Unit { get; set; }
}

public sealed class Milestone
{
    public int Target { get; set; }
    public int Current { get; set; }
}
