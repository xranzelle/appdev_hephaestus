using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly MembershipDate { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
