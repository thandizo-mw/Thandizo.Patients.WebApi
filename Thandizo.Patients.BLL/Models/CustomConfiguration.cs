namespace Thandizo.Patients.BLL.Models
{
    public class CustomConfiguration
    {
        public string DailySymptomsQueueAddress { get; set; }
        public string PatientQueueAddress { get; set; }
        public string EmailQueueAddress { get; set; }
        public string SmsQueueAddress { get; set; }
        public string SourceEmailAddress { get; set; }
        public string RegistrationEmailSubject { get; set; }
        public string DailySymptomsEmailSubject { get; set; }
        public string EmailTemplate { get; set; }
        public string SmsTemplate { get; set; }
    }
}
