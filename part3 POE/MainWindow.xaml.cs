using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyPOE
{
    public partial class MainWindow : Window
    {
        private ChatEngine _engine;
        private SoundPlayer? _player;
        private DatabaseManager _dbManager;
        private ObservableCollection<UserTask> _tasks = new ObservableCollection<UserTask>();
        private DispatcherTimer _reminderTimer;
        private HashSet<int> _remindedTaskIds = new HashSet<int>();

public MainWindow()
{
            InitializeComponent();

            _engine = new ChatEngine();
            _dbManager = new DatabaseManager();

            _engine.OnMemoryUpdated = OnMemoryUpdated;

            LoadAudio();

            LoadTasks();

            _reminderTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _reminderTimer.Tick += ReminderTimer_Tick;
            _reminderTimer.Start();
            CheckReminders();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            AddBotMessage(_engine.GetInitialMessage());
            txtInput.Focus();
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = _dbManager.GetTasks();
                _tasks.Clear();
                foreach (var t in tasks)
                {
                    _tasks.Add(t);
                }
                lstTasks.ItemsSource = _tasks;
            }
            catch { }
        }

        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            CheckReminders();
            LoadTasks();
        }

        private void CheckReminders()
        {
            try
            {
                var tasks = _dbManager.GetTasks();
                foreach (var t in tasks)
                {
                    if (t.ReminderDate.HasValue && !t.IsCompleted && t.ReminderDate.Value <= DateTime.Now && !_remindedTaskIds.Contains(t.Id))
                    {
                        AddBotMessage($"Reminder: {t.Title} - {t.Description} (Due {t.ReminderDate.Value:g})");
                        _remindedTaskIds.Add(t.Id);
                    }
                }

            }
            catch { }
        }

        private void LoadAudio()
        {
            try
            {
                string audioPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(audioPath))
                {
                    _player = new SoundPlayer(audioPath);
                    _player.Load();
                }
            }
            catch (Exception)
            {
            }
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                _player?.Play();
            }
            catch (Exception)
            {
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);
            txtInput.Clear();

            string response = _engine.ProcessInput(input);
            AddBotMessage(response);
            txtInput.Focus();
        }

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string topic)
            {
                string input = $"Tell me about {topic}";
                AddUserMessage(input);

                string response = _engine.ProcessInput(input);
                AddBotMessage(response);

                txtInput.Focus();
            }
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string action)
            {
                string input = "";
                switch (action)
                {
                    case "tasks": input = "Show my tasks"; break;
                    case "quiz": input = "Start quiz"; break;
                    case "log": input = "Show activity log"; break;
                }

                if (!string.IsNullOrEmpty(input))
                {
                    AddUserMessage(input);
                    string response = _engine.ProcessInput(input);
                    AddBotMessage(response);
                    txtInput.Focus();
                }
            }
        }

        private void BtnVoice_Click(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
        }

        private void BtnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
            {
                var tasks = _dbManager.GetTasks();
                var task = tasks.Find(t => t.Id == id);
                _dbManager.CompleteTask(id);
                _remindedTaskIds.Remove(id);
                LoadTasks();
                AddBotMessage(task != null ? $"Task '{task.Title}' marked completed." : "Task marked completed.");
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int id))
            {
                var tasks = _dbManager.GetTasks();
                var task = tasks.Find(t => t.Id == id);
                _dbManager.DeleteTask(id);
                _remindedTaskIds.Remove(id);
                LoadTasks();
                AddBotMessage(task != null ? $"Task '{task.Title}' deleted." : "Task deleted.");
            }
        }

        private void AddBotMessage(string message)
        {
            var bubble = CreateMessageBubble(message, isBot: true);
            chatPanel.Children.Add(bubble);

            bubble.Opacity = 0;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(350));
            fadeIn.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            bubble.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                chatScroll.ScrollToEnd();
            }));
        }

        private void AddUserMessage(string message)
        {
            var bubble = CreateMessageBubble(message, isBot: false);
            chatPanel.Children.Add(bubble);

            bubble.Opacity = 0;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            bubble.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                chatScroll.ScrollToEnd();
            }));
        }

        private Border CreateMessageBubble(string message, bool isBot)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(
                    isBot ? 4 : 16,
                    isBot ? 16 : 4,
                    16,
                    16
                ),
                Padding = new Thickness(16, 12, 16, 12),
                Margin = new Thickness(
                    isBot ? 0 : 80,
                    6,
                    isBot ? 80 : 0,
                    6
                ),
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                MaxWidth = 580,
                Background = new SolidColorBrush(isBot
                    ? (Color)ColorConverter.ConvertFromString("#F8F9FA")
                    : (Color)ColorConverter.ConvertFromString("#E4E9FF")),
                BorderBrush = new SolidColorBrush(isBot
                    ? (Color)ColorConverter.ConvertFromString("#E9ECEF")
                    : (Color)ColorConverter.ConvertFromString("#D4DDFE")),
                BorderThickness = new Thickness(1)
            };

            var stack = new StackPanel();

            var labelStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };
            if (isBot)
            {
                labelStack.Children.Add(new Ellipse { Width = 16, Height = 16, Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0033E2")), Margin = new Thickness(0, 0, 8, 0) });
            }
            else
            {
                labelStack.Children.Add(new Ellipse { Width = 16, Height = 16, Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), Margin = new Thickness(0, 0, 8, 0) });
            }

            var label = new TextBlock
            {
                Text = isBot ? "CyberBot" : "You",
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                FontFamily = new FontFamily("Segoe UI"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                VerticalAlignment = VerticalAlignment.Center
            };
            labelStack.Children.Add(label);

            var text = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 13.5,
                LineHeight = 22,
                FontFamily = new FontFamily("Segoe UI"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#374151"))
            };
            var timestamp = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 10,
                FontFamily = new FontFamily("Segoe UI"),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF")),
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                Margin = new Thickness(0, 8, 0, 0)
            };
            stack.Children.Add(labelStack);
            stack.Children.Add(text);
            stack.Children.Add(timestamp);
            border.Child = stack;
            return border;
        }

        private void OnMemoryUpdated(string key, string value)
        {
            Dispatcher.Invoke(() =>
            {
                switch (key.ToLower())
                {
                    case "name":
                        txtMemoryName.Text = value;
                        txtMemoryName.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#86E24B"));
                        break;
                    case "age":
                        txtMemoryAge.Text = value;
                        txtMemoryAge.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#86E24B"));
                        break;
                    case "interest":
                        txtMemoryInterest.Text = value;
                        txtMemoryInterest.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#00A2FF"));
                        break;
                }
            });
        }
    }
}
