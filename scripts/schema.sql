create schema dbo;

create table dbo.todo (
    id int not null,
    title varchar (200) null,
    description varchar (2000) null,
    priority int null,
    due date null,
    primary key  (id)
);



