namespace SkepsTicket.Model
{
    public class EmpresasConfig
    {
        public List<Empresa> Empresas { get; set; }
    }

    public class Empresa
    {
        public string Email { get; set; }
        public string Categoria { get; set; }
        public string Contrato { get; set; }
        public string Key { get; set; }
        public string Bot { get; set; }
        public string OwnerId { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerBusinessName { get; set; }
        public string CategoriaEmailAtivo { get; set; }
        public string OwnerTeam { get; set; }
        public string OriginEmailAccount { get; set; }
        public string EmailNaoResponda { get; set; }
        public List<string> AtendenteEmails { get; set; }
    }
}
