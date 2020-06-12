# CleverSchedule

<!--Comment Codes-->

DESCRIPTION: A list of coded comments to use to label area's in code or snippets that I will implement later.

- [ ] Code: 140800<br/>
Category: Identity User<br/>
Description: Snippet for Identity user select list<br/>
Purpose: Add to Admin view to select what type of account to create (Admin/Contractor)<br/>
Snippet:<br/>

```
<div class="form-group">
    <label asp-for="Input.Role"></label><br/>
    <select asp-for="Input.Role" class="form-control" aps-items="@Modul.Roles"></select>
</div>
```


- [ ] Code: 140801 <br/>
    Category: Identity User<br/>
    Descrption: Role Seeding<br/>
    Purpose: Update Seeded data to prevent Identity role 'bug'<br/>
    Snippet:<br/>

- [ ] Code: 140802<br/>
Category: Identity User<br/>
Description: Role assignment during registration<br/>
Purpose: Encapsulate user Role. To find better way to limit client ability to assign role.<br/>
Snippet: 

- [ ] Code: 140803
Category: User Story<br/>
Description: Check appointment availability<br/>
Purpose: Create appointment algorythm<br/>
Snippet:<br/>

- [x] Code: 140804 <br/>
Category: Hard Coded Information<br/>
Description: Contractor start location<br/>
Purpose: <br/>
    1. Create functionality to create seperate Client - Contractor relationship.<br/>
    2. Set start location to Contractor address where ContractorId == Clients.ContractorId<br/>
Snippet: <br/>
```
var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);<br/>
var client = _context.Clients.Where(c => c.IdentityUserId == userId)<br/>
    .Include(c => c.Address)<br/>
    .Include(c => c.Contractor)<br/>
    .Include(co => c.Contractor.Address)<br/>
    .SingleOrDefault();<br/>

Address startAddress = client.Contractor.Address;<br/>
```

