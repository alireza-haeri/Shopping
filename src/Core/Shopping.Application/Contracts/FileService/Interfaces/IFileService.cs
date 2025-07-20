using Shopping.Application.Contracts.FileService.Models;

namespace Shopping.Application.Contracts.FileService.Interfaces;

public interface IFileService
{
    Task<List<SaveFileResultModel>> SaveFilesAsync(List<SaveFileModel> files,CancellationToken cancellationToken);
    Task<List<GetFileModel>> GetFilesByNameAsync(List<string> fileNames, CancellationToken cancellationToken);
    Task RemoveFileAsync(string[] fileName, CancellationToken cancellationToken);
        
}