using System.Diagnostics;

namespace WeatherApp.Services.Common;

// Ход работы: что программа делает сейчас, со временем от старта.
// Пишется отдельно от отчёта, чтобы в стандартном выводе оставались только данные.
internal sealed class ProgressLog(TextWriter writer)
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly Lock _lock = new Lock();

    // Этап верхнего уровня.
    public void Step(string message)
    {
        Write(message);
    }

    // Подробность внутри этапа.
    public void Detail(string message)
    {
        Write($"  {message}");
    }

    // Сообщения приходят из параллельных запросов, поэтому запись под блокировкой.
    private void Write(string message)
    {
        TimeSpan elapsed = _stopwatch.Elapsed;

        lock (_lock)
        {
            writer.WriteLine($"[{(int)elapsed.TotalMinutes:00}:{elapsed.Seconds:00}] {message}");
        }
    }
}
