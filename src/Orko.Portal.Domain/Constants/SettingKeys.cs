namespace Orko.Portal.Domain.Constants;

public static class SettingKeys
{
    // SMTP
    public const string SmtpHost = "Smtp:Host";
    public const string SmtpPort = "Smtp:Port";
    public const string SmtpUsername = "Smtp:Username";
    public const string SmtpPassword = "Smtp:Password";
    public const string SmtpFromAddress = "Smtp:FromAddress";
    public const string SmtpFromName = "Smtp:FromName";
    public const string SmtpEnableSsl = "Smtp:EnableSsl";

    // Work Order Email Notification
    public const string WorkOrderEmailEnabled = "WorkOrderEmail:Enabled";
    public const string WorkOrderEmailRecipients = "WorkOrderEmail:Recipients";
    public const string WorkOrderEmailSubject = "WorkOrderEmail:Subject";
    public const string WorkOrderEmailTemplate = "WorkOrderEmail:Template";

    // Reminder
    public const string ReminderEnabled = "Reminder:Enabled";
    public const string ReminderCronExpression = "Reminder:CronExpression";
    public const string ReminderRecipients = "Reminder:Recipients";
    public const string ReminderDraftThresholdHours = "Reminder:DraftThresholdHours";
    public const string ReminderSubject = "Reminder:Subject";
    public const string ReminderTemplate = "Reminder:Template";

    // Categories
    public const string CategorySmtp = "Smtp";
    public const string CategoryWorkOrderEmail = "WorkOrderEmail";
    public const string CategoryReminder = "Reminder";
}
