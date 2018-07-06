# EasyDatabaseCompare
A simple database 2 time snapshot comparison tool

Easy to find target database data changing!

Very convenient when analyzing third-party software database changes!

I found that there are many database comparer software, but most are very cumbersome, not suitable for analyzing third-party databases, and supported database also too less. <span style="background-color:#000">And most are not free.<span/>

So, I made this.

-------------------
!! Database schema should not changed

Accept:

|Database Type|Version|Accept State|
|-|-|-|
|MicroSoft Sql Server|All|OK|
|MicroSoft Access|All|OK|
|MySQL|All|OK|
|SQLite|After SQLite3|OK|
|Firebird|Unkonw|OK|
|InterBase|Unkonw|Maybe? <br /> Try Firebird|

How to use
-----------------------
![](https://github.com/Flithor/EasyDatabaseCompare/blob/NewFramework/img.png)

Special Note
------------------------
This project is my practice & learn various design patterns project

If possible, please try to use design pattern to extend it

If not, it does not matter

Used: [Rx](https://github.com/dotnet/reactive), [Ninject](https://github.com/ninject/Ninject)
