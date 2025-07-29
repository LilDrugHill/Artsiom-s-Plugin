namespace LinkCleaner.Presentation.Interfaces
{
    public interface IDocumentViewData
    {

        Guid Guid { get; init; }

        bool Status { get; set; }

        List<ILinkViewData>? LinksInDocument { get ; init; }

        string Name { get; set; }
        
    }
}
