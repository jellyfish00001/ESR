namespace ERS.DTO.BDTreelevel
{
    public class QueryBDTreelevelDto
    {
        public string levelname { get; set; } //簽核層級名稱(英文)
        public string leveltwname { get; set; } //簽核層級名稱(繁中)
        public string levelcnname { get; set; } //簽核層級名稱(簡中)
        public decimal levelnum { get; set; } //簽核層級編號

    }
}