BaseURL=localhost:5000/api/TodoApi

Get All Todo’s
url=/
method=get

Get Specific Todo
url=/{id}
method=get

Get Incoming ToDo (for today/next day/current week)
url=/incoming/{type}
method=get
type = enum today, tommorow, thisweek

Create Todo
url=/
method=post

Update Todo
url=/{id}
method=put

Set Todo percent complete
url=/progress/{id}/{progress}
method=put
progress=int 0-100

Delete Todo
url=/{id}
method=delete

Mark Todo as Done
url=/marktodo/{id}
method=put
