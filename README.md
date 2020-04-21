# CleverSchedule

<!--Comment Codes-->

DESCRIPTION: A list of coded comments to use to label area's in code or snippets that I will implement later.

>>>>>>>>>>>>>>>>>>

Code: (INCOMPLETE)
Category: Identity User
Description: Snippet for Identity user select list
Purpose: Add to Admin view to select what type of account to create (Admin/Contractor)
Snippet:

<div class="form-group">
    <label asp-for="Input.Role"></label>
    <select asp-for="Input.Role" class="form-control" aps-items="@Modul.Roles"></select>
</div>

================

Code: 1408001 (complete)
Category: Identity User
Descrption: Role Seeding
Purpose: Update Seeded data to prevent Identity role 'bug'
Snippet:

>>>>>>>>>>>>>>>>>>

Code: 140802
Category: Identity User
Description: Role assignment during registration
Purpose: Encapsulate user Role. SET: Client for production build.
Snippet: 

value="Client" hidden/>

================