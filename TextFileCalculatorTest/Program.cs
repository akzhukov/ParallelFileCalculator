using TextFileCalculatorTest.Manager;

if (args.Length != 2)
{
    Console.WriteLine("Incorrect console arguments, first argumet must be input folder, second is output folder!");
    return;
}

string inputFolder = args[0];
string outputFolder = args[1];

CancellationTokenSource cancellationTokenSource = new();
CancellationToken cancellationToken = cancellationTokenSource.Token;

CalculatorsManager manager = new(inputFolder, outputFolder);

try
{
    manager.Run(4, cancellationToken);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

ConsoleKeyInfo consoleKeyInfo;

while (true)
{
    consoleKeyInfo = Console.ReadKey();
    if (consoleKeyInfo.Modifiers == ConsoleModifiers.Control
        && consoleKeyInfo.Key == ConsoleKey.Z)
    {
        break;
    }
}

cancellationTokenSource.Cancel();

while (true)
{
    if (manager.IsAllCalculatorsFinished())
        Environment.Exit(0);
}
