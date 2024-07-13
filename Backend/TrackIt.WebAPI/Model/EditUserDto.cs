    public class EditUserDto
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }

        public string? Name { get; set; } = string.Empty;
        public string? Role { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public bool? IsApproved { get; set; } = true;
        public string? Password { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = true;
        public string? Token { get; set; } = string.Empty; 
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; } = Guid.Empty;
        public Guid? UpdatedBy { get; set; } = Guid.Empty;


    }
