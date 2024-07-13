
    public class CreateUserDto
    {
        //public Guid Id { get; set; }
        //public Guid RoleId { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty ;
        public bool IsApproved { get; set; } = true;
        public string Password { get; set; } = string.Empty;



    }
