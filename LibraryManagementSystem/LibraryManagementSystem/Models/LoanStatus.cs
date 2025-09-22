using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class LoanStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
