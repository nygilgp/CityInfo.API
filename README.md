# CityInfo.API
.NET core project


## Response Status
Level 200 Success
- 200 OK
- 201 Created
- 204 No content, used after delete action

Leve 400 Client error
- 400 Bad request
- 401 Unauthorized
- 403 Forbidden, user is authorized but, don't have access to view the information
- 404 Not found
- 409 Conflict

Level 500 Sever Error
- 500 Internal server error, client can't do anything about it
