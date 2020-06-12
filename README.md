# CleverSchedule

### Technologies: 

* DB - Code first:
  * SQL Server database
  * Entity Framework 

* Back-End: 
  * ASP.Net Core MVC
  * LINQ

* Front-End:
  * ASP.Net Core MVC
  * Bootstrap 4 

### To do: 
  #### Current: Scheduling algorithm 140803

- [ ] Code: 140800<br/>

    Category: Identity User<br/>
    Description: Snippet for Identity user select list<br/>
    Purpose: Add to Admin view to select what type of account to create (Admin/Contractor)<br/>
    Snippet:<br/>
    ```
    <div class="form-group">
        <label asp-for="Input.Role"></label>
        <select asp-for="Input.Role" class="form-control" aps-items="@Modul.Roles"></select>
    </div>
    ```

- [ ] Code: 140801 <br/>

    Category: Identity User<br/>
    Descrption: Role Seeding<br/>
    Purpose: Update Seeded data to prevent Identity role 'bug'<br/>

- [ ] Code: 140802<br/>

    Category: Identity User<br/>
    Description: Role assignment during registration<br/>
    Purpose: Encapsulate user Role. To find better way to limit client ability to assign role<br/>

- [ ] Code: 140803<br />

    Category: User Story<br/>
    Description: Check appointment availability<br/>
    Purpose: Create appointment algorythm<br/>

- [x] Code: 140804 <br/>

    Category: Hard Coded Information<br/>
    Description: Contractor start location<br/>
    Purpose: <br/>
        1. Create functionality to create seperate Client - Contractor relationship.<br/>
        2. Set start location to Contractor address where ContractorId == Clients.ContractorId<br/>
    Snippet: <br/>
    ```
    var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
    var client = _context.Clients.Where(c => c.IdentityUserId == userId)
        .Include(c => c.Address)
        .Include(c => c.Contractor
        .Include(co => c.Contractor.Address
        .SingleOrDefault();

    Address startAddress = client.Contractor.Address;
    ```

