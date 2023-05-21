using System.Collections.Concurrent;
using TextFileCalculator.FilesHandler;
using TextFileCalculatorTest.Calculator;

namespace TextFileCalculatorTest.Manager;

public class CalculatorsManager
{
    private readonly string _inputFolder;
    private readonly string _outputFolder;
    private readonly ConcurrentQueue<string> _filesQueue = new();
    private int _numfinishedCalculators = 0;
    private int _numCalculators = 0;
    private readonly object _locker = new();
    private bool _isNewFileHanderRun = false;

    public CalculatorsManager(string inputFolder, string outputFolder)
    {
        _inputFolder = inputFolder;
        _outputFolder = outputFolder;
    }

    public void Run(int numCalculators, CancellationToken cancellationToken)
    {
        CheckFolders();

        if (!_isNewFileHanderRun)
            RunNewFilesHandler(cancellationToken);

        for (int i = 0; i < numCalculators; i++)
        {
            RunCalculator(cancellationToken);
        }
        _numCalculators += numCalculators;
    }

    public bool IsAllCalculatorsFinished()
    {
        return _numfinishedCalculators == _numCalculators;
    }

    private void CheckFolders()
    {
        if (!Directory.Exists(_inputFolder))
        {
            throw new DirectoryNotFoundException($"Folder {_inputFolder} does not found");
        }

        if (!Directory.Exists(_outputFolder))
        {
            Directory.CreateDirectory(_outputFolder);
        }
    }

    private void RunCalculator(CancellationToken cancellationToken)
    {
        Task task = new(() =>
        {
            IFileCalculator calculator = new FileCalculator(_outputFolder);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    lock (_locker)
                    {
                        _numfinishedCalculators++;
                    }
                    return;
                }
                if (_filesQueue.TryDequeue(out var fileName))
                {
                    if (!calculator.Calculate(fileName))
                    {
                        _filesQueue.Enqueue(fileName);
                    }
                }
            }

        }, cancellationToken);

        task.Start();
    }

    private void RunNewFilesHandler(CancellationToken cancellationToken)
    {
        Task task = new(() =>
        {
            INewFilesHandler filesHandler = new NewFilesHandler(_inputFolder);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _isNewFileHanderRun = false;
                    return;
                }

                var files = filesHandler.GetNewFiles();

                foreach (var file in files)
                {
                    _filesQueue.Enqueue(file);
                }
            }

        }, cancellationToken);

        _isNewFileHanderRun = true;
        task.Start();
    }
}
