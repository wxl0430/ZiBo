using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;

namespace CRSim
{
    public class UpdateChecker
    {
        private const string ApiUrl = "https://crsim.com.cn/api/version";
        private readonly HttpClient _httpClient = new HttpClient();
        private ProgressWindow _progressWindow;
        private CancellationTokenSource _cancellationTokenSource;

        public async Task CheckForUpdates()
        {
            try
            {
                await ExecuteOnUIThread(async () =>
                {
                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    var latestVersionInfo = await GetLatestVersionInfo();

                    if (latestVersionInfo != null && latestVersionInfo.ver != currentVersion)
                    {
                        var result = ShowQuestion($"有新版本可用，当前版本：{currentVersion}\n最新版本: {latestVersionInfo.ver}\n\n更新日志:\n{latestVersionInfo.log}\n\n是否更新?（点击更新后请等待，程序将自动重启，时间与下载速度有关）");
                        
                        if (result)
                        {
                            _cancellationTokenSource = new CancellationTokenSource();
                            ShowProgressWindow("准备下载更新...");
                            
                            try
                            {
                                await DownloadAndInitiateUpdate(latestVersionInfo, _cancellationTokenSource.Token);
                            }
                            catch (OperationCanceledException)
                            {
                                ShowMessage("更新已取消");
                            }
                            finally
                            {
                                CloseProgressWindow();
                                _cancellationTokenSource = null;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CloseProgressWindow();
                ShowError($"更新检查失败：{ex.Message}");
            }
        }

        private async Task<VersionInfo> GetLatestVersionInfo()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<VersionInfo>(json);
                }
            }
            catch (Exception ex)
            {
                ShowError($"调用API失败： {ex.Message}");
            }
            return null;
        }

        private async Task DownloadAndInitiateUpdate(VersionInfo versionInfo, CancellationToken cancellationToken)
        {
            try
            {
                var tempFilePath = await DownloadFileWithProgress(versionInfo.download_file, cancellationToken);

                var isValid = await Task.Run(() => ValidateFile(tempFilePath, versionInfo.file_md5, versionInfo.file_sha256));
                
                if (!isValid)
                {
                    var forceUpdate = ShowQuestion("md5和sha256验证失败，可能遭到网络劫持，是否强制更新？");
                    if (!forceUpdate)
                    {
                        File.Delete(tempFilePath);
                        return;
                    }
                }

                var programDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var appExePath = Process.GetCurrentProcess().MainModule.FileName;
                var appName = Path.GetFileName(appExePath);
                string batchScript = $@"
@echo off
taskkill /F /IM ""{appName}"" >nul 2>&1
timeout /t 2 >nul
cd /d ""{programDirectory}""
powershell -Command ""Expand-Archive -Path '{tempFilePath}' -DestinationPath '{programDirectory}' -Force""
del /f /q ""{tempFilePath}""
start """" ""{appExePath}""
exit /b 0
";

                string batchFilePath = Path.Combine(Path.GetTempPath(), "CRSimUpdate.bat");
                File.WriteAllText(batchFilePath, batchScript);

                try
                {
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c \"{batchFilePath}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process.Start(processInfo);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    ShowError($"启动更新脚本进程失败: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"更新失败: {ex.Message}");
                throw;
            }
        }

        private async Task<string> DownloadFileWithProgress(string downloadUrl, CancellationToken cancellationToken)
        {
            var tempFile = Path.GetTempFileName() + ".zip";
            
            try
            {
                using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                var contentLength = response.Content.Headers.ContentLength;
                UpdateProgress("开始下载更新文件...", 0);

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = new FileStream(tempFile, FileMode.Create);

                var buffer = new byte[8192];
                var totalBytesRead = 0L;
                var lastUpdateBytes = 0L;
                var updateIntervalBytes = contentLength.HasValue ? Math.Max(1024 * 1024, contentLength.Value / 100) : 1024 * 1024;
                int bytesRead; 
                
                UpdateProgress($"准备下载: {GetSizeString(contentLength ?? 0)}", 0);

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    totalBytesRead += bytesRead;

                    if (totalBytesRead - lastUpdateBytes >= updateIntervalBytes || totalBytesRead == contentLength)
                    {
                        if (contentLength.HasValue)
                        {
                            var progressPercentage = (int)((totalBytesRead * 100) / contentLength.Value);
                            UpdateProgress($"下载中: {progressPercentage}%", progressPercentage);
                        }
                        else
                        {
                            UpdateProgress($"下载中: {GetSizeString(totalBytesRead)}", 0);
                        }

                        lastUpdateBytes = totalBytesRead;
                    }
                }

                UpdateProgress("下载完成，准备更新...", 100);
                return tempFile;
            }
            catch (OperationCanceledException)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                
                ShowError($"下载文件失败: {ex.Message}");
                throw;
            }
        }

        private string GetSizeString(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024):F1} MB";
        }

        private bool ValidateFile(string filePath, string expectedMd5, string expectedSha256)
        {
            try
            {
                using var md5 = MD5.Create();
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                
                var actualMd5 = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                stream.Position = 0;
                var actualSha256 = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                
                return actualMd5 == expectedMd5 && actualSha256 == expectedSha256;
            }
            catch
            {
                return false;
            }
        }

        private void ShowProgressWindow(string message)
        {
            ExecuteOnUIThread(() =>
            {
                _progressWindow = new ProgressWindow { Owner = Application.Current.MainWindow };
                _progressWindow.UpdateStatus(message);
                _progressWindow.Show();
            });
        }

        private void UpdateProgress(string message, int progress)
        {
            ExecuteOnUIThread(() =>
            {
                if (_progressWindow != null && _progressWindow.IsVisible)
                {
                    _progressWindow.UpdateStatus(message);
                    _progressWindow.UpdateProgress(progress);
                }
            });
        }

        private void CloseProgressWindow()
        {
            ExecuteOnUIThread(() =>
            {
                _progressWindow?.Close();
                _progressWindow = null;
            });
        }

        private Task ExecuteOnUIThread(Action action)
        {
            if (Application.Current == null)
            {
                action();
                return Task.CompletedTask;
            }
            
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return Application.Current.Dispatcher.InvokeAsync(action).Task;
            }
        }

        private void ShowMessage(string message)
        {
            ExecuteOnUIThread(() =>
            {
                MessageBox.Show(message, "CRSim", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        private void ShowError(string message)
        {
            ExecuteOnUIThread(() =>
            {
                MessageBox.Show(message, "CRSim - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private bool ShowQuestion(string message)
        {
            var result = false;
            
            ExecuteOnUIThread(() =>
            {
                result = MessageBox.Show(message, "CRSim", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            }).Wait();
            
            return result;
        }
    }

    public class ProgressWindow : Window
    {
        private readonly TextBlock _statusText;
        private readonly ProgressBar _progressBar;

        public ProgressWindow()
        {
            Title = "更新进度";
            Width = 400;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            
            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            
            _statusText = new TextBlock
            {
                Text = "准备中...",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 10, 0, 20)
            };
            
            _progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Height = 20
            };
            
            stackPanel.Children.Add(_statusText);
            stackPanel.Children.Add(_progressBar);
            
            Content = stackPanel;
        }
        
        public void UpdateStatus(string message)
        {
            _statusText.Text = message;
        }
        
        public void UpdateProgress(int value)
        {
            _progressBar.Value = value;
        }
    }

    public class VersionInfo
    {
        public string status_code { get; set; }
        public string message { get; set; }
        public string ver { get; set; }
        public string download_file { get; set; }
        public string file_md5 { get; set; }
        public string file_sha256 { get; set; }
        public string log { get; set; }
    }
}