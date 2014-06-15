!!!! XERO Automation !!!!!

Tools used : VS 20012, Selenium Wedriver, FireFox
Solution: This Solution consists of 2 Projects 1.XERO APP Framework and 2. XERO Automation Tests

PRE-Requisites Before Running Autoamtions:
------------------------------------------
1. Login to Xero.com and Make sure only Organization(Blah Blah) is created. Delete all the other Organizations
2. Open Organization Blah Blah and Navigate to Accounts/Sales/Repeating Invoices Tab. Make Sure Below list is as it is. 
Allu - ApproveD
Brahmi - Saved as draft
Jack - Saved as draft
bompr - Approved & Sent
Jumbo - Approved
 
Debugging Options: 
------------------
After every testcase run can check the TestResults folder for a Video Recording during testing and Log File
Video Recording will be in TestResults\TestRun\IN\<XYZ>\<XYZ>\ScreeningRecording.wmv
Log File will be in TestResults\TestRun\Out\LogFile_<Date>.txt