# CleverSchedule

<!--Comment Codes-->

DESCRIPTION: A list of coded comments to use to label area's in code or snippets that I will implement later.

- [ ] Code: 140800
Category: Identity User
Description: Snippet for Identity user select list
Purpose: Add to Admin view to select what type of account to create (Admin/Contractor)
Snippet:

<div class="form-group">
    <label asp-for="Input.Role"></label>
    <select asp-for="Input.Role" class="form-control" aps-items="@Modul.Roles"></select>
</div>


-[ ] Code: 1408001 <br/>
Category: Identity User
Descrption: Role Seeding
Purpose: Update Seeded data to prevent Identity role 'bug'
Snippet:

- [ ] Code: 140802
Category: Identity User
Description: Role assignment during registration
Purpose: Encapsulate user Role. To find better way to limit client ability to assign role.
Snippet: 

- [ ] Code: 140803
Category: User Story
Description: Check appointment availability
Purpose: Create appointment algorythm
Snippet:

Code: 140804 (COMPLETED)
Category: Hard Coded Information
Description: Contractor start location
Purpose: 
    1. Create functionality to create seperate Client - Contractor relationship.
    2. Set start location to Contractor address where ContractorId == Clients.ContractorId
Snippet: 

var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
var client = _context.Clients.Where(c => c.IdentityUserId == userId)
    .Include(c => c.Address)
    .Include(c => c.Contractor)
    .Include(co => c.Contractor.Address)
    .SingleOrDefault();

Address startAddress = client.Contractor.Address;


