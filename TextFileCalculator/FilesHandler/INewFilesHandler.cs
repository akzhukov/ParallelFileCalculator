namespace TextFileCalculator.FilesHandler;

internal interface INewFilesHandler
{
    IEnumerable<string> GetNewFiles();
}
