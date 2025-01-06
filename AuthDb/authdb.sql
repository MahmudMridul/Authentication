use Auth

select *
from Roles;

select *
from Users;

select *
from RefreshTokens;

--update Users
--set RefreshTokenExpiryTime = '2025-01-01'
--where UserName = 'UserFive@5';

--delete from Users;

select u.UserName, u.Email, rt.Token, rt.Expires
from Users u inner join RefreshTokens rt on u.Id = rt.UserId;