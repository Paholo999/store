

use bdstore;

drop table Products;
drop table Sales;


delete from Products where id = 1;

alter table Products
add Stock int not null;


create table Products(
	id int not null identity(1,1) primary key,
	name varchar(500) not null,
	unitprice float not null,
	description varchar(500) not null,
	stock int not null
)

create table Sales(
	id int not null identity(1,1) primary key,
	quantity int not null,
	vat float not null,
	totalvat float not null,
	totalsale float not null
)

