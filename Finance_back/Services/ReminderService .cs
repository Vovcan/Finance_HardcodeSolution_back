
namespace Finance_back.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly ILogger<ReminderService> _logger;
        private readonly MongoDBService _reminderRepository;

        public ReminderService(ILogger<ReminderService> logger, MongoDBService reminderRepository)
        {
            _logger = logger;
            _reminderRepository = reminderRepository;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Проверяем и отправляем напоминания
                await CheckAndSendReminders();

                // Ждем некоторое время перед следующей проверкой
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private async Task CheckAndSendReminders()
        {
            // Получаем напоминания с датой, совпадающей с текущей датой и временем
            var reminders = await _reminderRepository.GetRemindersDueNow();

            foreach (var reminder in reminders)
            {
                // Отправляем напоминание (замените эту часть кода на вашу логику отправки)
                // SendReminder(reminder);

                _logger.LogInformation($"Reminder sent for {reminder.Title}");
            }
        }
    }
}
