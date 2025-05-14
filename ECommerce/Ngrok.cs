using System.Diagnostics;

namespace ECommerce
{
    public class Ngrok : IDisposable
    {
        private Process _ngrokProcess;

        public Ngrok()
        {
            _ngrokProcess = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Ngrok\ngrok.exe",
                    Arguments = $"http --url=rational-deep-dinosaur.ngrok-free.app https://localhost:7182",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                Console.WriteLine("Starting ngrok process...");
                _ngrokProcess.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start ngrok process: {ex.Message}");
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_ngrokProcess != null && !_ngrokProcess.HasExited)
            {
                _ngrokProcess.Kill();
                _ngrokProcess.Dispose();
                Console.WriteLine("Ngrok process terminated.");
            }
        }
    }
}
