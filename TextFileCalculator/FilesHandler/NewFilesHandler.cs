namespace TextFileCalculator.FilesHandler;

internal class NewFilesHandler : INewFilesHandler
{
    private readonly string _inputFolder;
    private readonly HashSet<string> _oldFiles = new();

    public NewFilesHandler(string inputFolder)
    {
        _inputFolder = inputFolder;
    }

    public IEnumerable<string> GetNewFiles()
    {
        var files = Directory.GetFiles(_inputFolder);
        List<string> newFiles = new();

        foreach (var file in files)
        {
            if (!_oldFiles.Contains(file))
                newFiles.Add(file);
            _oldFiles.Add(file);
        }

        return newFiles;
    }
}
