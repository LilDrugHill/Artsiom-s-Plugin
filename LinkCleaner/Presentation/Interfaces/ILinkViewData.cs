namespace LinkCleaner.Presentation.Interfaces
{
    public interface ILinkViewData
    {
        bool IsMonitoring { get; set; }

        string Name { get; set; }

        Guid Guid { get; init; }
    }
}
