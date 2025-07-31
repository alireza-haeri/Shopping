using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Shopping.WebFramework.Extensions;

namespace Shopping.WebFramework.Swagger;

public class ApiVersionDocumentProcessor:IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var version = context.Document.Info.Version;
        
        var pathsToRemove = context.Document.Paths
            .Where(pathItem => !RegExHelpers.MatchesApiVersion(version, pathItem.Key))
            .Select(c => c.Key)
            .ToList();

        foreach (var pathToRemove in pathsToRemove)
        {
            if (pathToRemove != null) context.Document.Paths.Remove(pathToRemove);
        }
    }
}