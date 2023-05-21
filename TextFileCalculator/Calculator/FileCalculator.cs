namespace TextFileCalculatorTest.Calculator;

internal class FileCalculator : IFileCalculator
{
    private readonly string _outputFolder;
    private readonly int _numAttempts = 10;

    public FileCalculator(string outpuFolder)
    {
        _outputFolder = outpuFolder;
    }

    public bool Calculate(string fileName)
    {
        //если предполагается, что файл может быть очень большим,
        //то надо сделать через какой-нибудь BufferStream
        for (int i = 0; i < _numAttempts; i++)
        {
            try
            {
                string text = File.ReadAllText(fileName);

                int numLetters = text.Where(char.IsLetter).Count();

                File.WriteAllText(Path.Combine(_outputFolder, Path.GetFileName(fileName)), numLetters.ToString());

                return true;
            }
            catch (Exception)
            {
                Thread.Sleep(100);
            }
        }

        return false;
    }
}
