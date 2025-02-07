using System.Text.Json.Serialization;

namespace SkepsTicket.Model
{
    public class TicketModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("type")]
        public int? Type { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("urgency")]
        public string Urgency { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("baseStatus")]
        public string BaseStatus { get; set; }

        [JsonPropertyName("justification")]
        public string Justification { get; set; }

        [JsonPropertyName("origin")]
        public int? Origin { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("originEmailAccount")]
        public string OriginEmailAccount { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }

        [JsonPropertyName("ownerTeam")]
        public string OwnerTeam { get; set; }

        [JsonPropertyName("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonPropertyName("serviceFull")]
        public List<object> ServiceFull { get; set; }

        [JsonPropertyName("serviceFirstLevelId")]
        public string ServiceFirstLevelId { get; set; }

        [JsonPropertyName("serviceFirstLevel")]
        public string ServiceFirstLevel { get; set; }

        [JsonPropertyName("serviceSecondLevel")]
        public string ServiceSecondLevel { get; set; }

        [JsonPropertyName("serviceThirdLevel")]
        public string ServiceThirdLevel { get; set; }

        [JsonPropertyName("contactForm")]
        public string ContactForm { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("cc")]
        public string Cc { get; set; }

        [JsonPropertyName("resolvedIn")]
        public DateTime? ResolvedIn { get; set; }

        [JsonPropertyName("closedIn")]
        public DateTime? ClosedIn { get; set; }

        [JsonPropertyName("canceledIn")]
        public DateTime? CanceledIn { get; set; }

        [JsonPropertyName("actionCount")]
        public int ActionCount { get; set; }

        [JsonPropertyName("lifeTimeWorkingTime")]
        public string LifeTimeWorkingTime { get; set; }

        [JsonPropertyName("stoppedTime")]
        public string StoppedTime { get; set; }

        [JsonPropertyName("stoppedTimeWorkingTime")]
        public string StoppedTimeWorkingTime { get; set; }

        [JsonPropertyName("resolvedInFirstCall")]
        public bool ResolvedInFirstCall { get; set; }

        [JsonPropertyName("chatWidget")]
        public string ChatWidget { get; set; }

        [JsonPropertyName("chatGroup")]
        public string ChatGroup { get; set; }

        [JsonPropertyName("chatTalkTime")]
        public string ChatTalkTime { get; set; }

        [JsonPropertyName("chatWaitingTime")]
        public string ChatWaitingTime { get; set; }

        [JsonPropertyName("sequence")]
        public string Sequence { get; set; }

        [JsonPropertyName("slaAgreement")]
        public string SlaAgreement { get; set; }

        [JsonPropertyName("slaAgreementRule")]
        public string SlaAgreementRule { get; set; }

        [JsonPropertyName("slaSolutionTime")]
        public int SlaSolutionTime { get; set; }

        [JsonPropertyName("slaResponseTime")]
        public int SlaResponseTime { get; set; }

        [JsonPropertyName("slaSolutionChangedByUser")]
        public bool SlaSolutionChangedByUser { get; set; }

        [JsonPropertyName("slaSolutionChangedBy")]
        public CreatedBy SlaSolutionChangedBy { get; set; }

        [JsonPropertyName("slaSolutionDate")]
        public DateTime? SlaSolutionDate { get; set; }

        [JsonPropertyName("slaSolutionDateIsPaused")]
        public bool SlaSolutionDateIsPaused { get; set; }

        [JsonPropertyName("jiraIssueKey")]
        public string JiraIssueKey { get; set; }

        [JsonPropertyName("redmineIssueId")]
        public string RedmineIssueId { get; set; }

        [JsonPropertyName("movideskTicketNumber")]
        public string MovideskTicketNumber { get; set; }

        [JsonPropertyName("linkedToIntegratedTicketNumber")]
        public string LinkedToIntegratedTicketNumber { get; set; }

        [JsonPropertyName("reopenedIn")]
        public DateTime? ReopenedIn { get; set; }

        [JsonPropertyName("lastActionDate")]
        public DateTime? LastActionDate { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [JsonPropertyName("slaResponseDate")]
        public DateTime? SlaResponseDate { get; set; }

        [JsonPropertyName("slaRealResponseDate")]
        public DateTime? SlaRealResponseDate { get; set; }

        [JsonPropertyName("clients")]
        public List<Client> Clients { get; set; }

        [JsonPropertyName("actions")]
        public List<Action> Actions { get; set; }

        [JsonPropertyName("parentTickets")]
        public List<object> ParentTickets { get; set; }

        [JsonPropertyName("childrenTickets")]
        public List<object> ChildrenTickets { get; set; }

        [JsonPropertyName("ownerHistories")]
        public List<OwnerHistory> OwnerHistories { get; set; }

        [JsonPropertyName("statusHistories")]
        public List<StatusHistory> StatusHistories { get; set; }

        [JsonPropertyName("satisfactionSurveyResponses")]
        public List<object> SatisfactionSurveyResponses { get; set; }

        [JsonPropertyName("customFieldValues")]
        public List<object> CustomFieldValues { get; set; }

        [JsonPropertyName("assets")]
        public List<object> Assets { get; set; }

        [JsonPropertyName("webhookEvents")]
        public string WebhookEvents { get; set; }
    }

    public class Owner
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("personType")]
        public int PersonType { get; set; }

        [JsonPropertyName("profileType")]
        public int ProfileType { get; set; }

        [JsonPropertyName("businessName")]
        public string BusinessName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }
    }

    public class CreatedBy : Owner { }

    public class Client : Owner
    {
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("organization")]
        public string Organization { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("complement")]
        public string Complement { get; set; }

        [JsonPropertyName("cep")]
        public string Cep { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }
    }

    public class Action
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("origin")]
        public int Origin { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("htmlDescription")]
        public string HtmlDescription { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("justification")]
        public string Justification { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("timeAppointments")]
        public List<object> TimeAppointments { get; set; }

        [JsonPropertyName("attachments")]
        public List<Attachments> Attachments { get; set; }

        [JsonPropertyName("expenses")]
        public List<object> Expenses { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }

    public class Attachments
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
    }

    public class OwnerHistory
    {
        [JsonPropertyName("ownerTeam")]
        public string OwnerTeam { get; set; }

        [JsonPropertyName("owner")]
        public Owner Owner { get; set; }

        [JsonPropertyName("changedBy")]
        public CreatedBy ChangedBy { get; set; }

        [JsonPropertyName("changedDate")]
        public DateTime? ChangedDate { get; set; }

        [JsonPropertyName("permanencyTimeFullTime")]
        public double? PermanencyTimeFullTime { get; set; }

        [JsonPropertyName("permanencyTimeWorkingTime")]
        public double? PermanencyTimeWorkingTime { get; set; }
    }

    public class StatusHistory
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("justification")]
        public string Justification { get; set; }

        [JsonPropertyName("changedBy")]
        public CreatedBy ChangedBy { get; set; }

        [JsonPropertyName("changedDate")]
        public DateTime? ChangedDate { get; set; }

        [JsonPropertyName("permanencyTimeFullTime")]
        public double? PermanencyTimeFullTime { get; set; }

        [JsonPropertyName("permanencyTimeWorkingTime")]
        public double? PermanencyTimeWorkingTime { get; set; }
    }
}
