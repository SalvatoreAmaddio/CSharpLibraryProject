# MyApplicationDemo

Simple Demo Application that manage Employee's Payslips. Payslips can be viewed in a ReportViewer control, printed as PDF, sent by email to employees.
Also, data can be extracted and saved into Excel files.

Payslip's calculation was simplified for demonstration purposes. 

## Description
This is demo demonstrates the use of the [DesktopBusinessAppSharpBuilder Framework](https://github.com/SalvatoreAmaddio/DesktopBusinessAppSharpBuilder).

The SQLite database has a table named User which is empty.

If the user table is empty, the System assumes that the software is running for the first time on the local machine.
A dialog will prompt the user to register themselves. Here, the user can provide a username and password.

Upon registration, the user will be asked to login with their new credentials.

Delete the user from the database to repeat the process.