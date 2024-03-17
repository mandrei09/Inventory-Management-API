namespace Optima.Fais.Dto
{
    public class OrderDelete
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string UserId { get; set; }
        public OrderDelete(int id, string reason, string userId)
        {
            Id = id;
            Reason = reason;
            UserId = userId;
        }
    }
}
