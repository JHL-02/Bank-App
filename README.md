# Bank App
Our console-based bank application allows users to open multiple accounts, transfer funds and take out loans. 

## **Features**
**Hardcoded users:**  
User0, 1234, is admin  
User1, 1234  

## **Structure**
Our bank app consists of multiple folders to make navigation easier. 
### **Currency**
The currency folder consists of 2 files. 
- Currency.cs only contains enums for our currency types: SEK, USD, EUR. 
- ExchangeRates.cs contain more logic. It has hardcoded exchange rates that an admin can change every day. It also handles conversion from one currency to another. But also shows the current exchange rate, updated or not. 
### **Menu**
Menu contains three files and handles the building of the menu. 
- MenuResults.cs contains only enums. 
- Menu.cs handles the base of the menu. It has a movable “Quit” button, it has built in error handling if a user doesn’t type an integer. And it has a moveable structure to make the menu pretty. 
- MenuOptions.cs 
### **Services**
Services contains 2 files. 
- Bank.cs is where most of the user interaction happens. It pulls from Menu to write out a nice, adjustable menu letting users log into the bank. It also contains some hardcoded users, one who is an admin. It also allows admins to adjust the credit rate for users, in general or depending on credit rates. 
- UserInput.cs is a general helper file that quick methods with built in error handling. Methods for example getting a string or integer from users. 
### **Users**
Users has one folder (Accounts) and 4 files. 
- User.cs is a massive file storing username and passwords but also accounts, debt and credit score. This contains code that allow users to add their own account (bank account or savings account), to take out loans but also see info about the user but also about their individual bank accounts. Users can transfer money between their own accounts or to other users, with a wait time of 15 minutes. Users can also choose which currency their account displays. Users can also see their transaction history.
- Admin.cs inherits from User and contains a special admin menu. If a user is an admin they can choose to either log in as a user (with restricted access) or as an admin. Admins can add people into the system, give specific users money or adjust the credit rates.
- Login.cs allows users to log in with unique usernames that are connected to a specific password. Users are only allowed to try and login 3 times before they get locked out. 
- UserDatabase keeps track of all the users in the system. It also has the methods to add or remove users into the system. Although, only admin is allowed to access that.  
#### **Accounts**
Accounts has two files. 
- BankAccount handles the normal accounts. It has limits for a maximum of 10 bank accounts per user with each account only being allowed a 20 character name. The logic for transactions is somewhere else but this class executes it with error handling, for example if users try and transfer money into the same account that they are transferring out from. 
- SavingAccount inherits from BankAccount and specifically handles interest rates. It shows how high the interest rates are and the expected returns at the end of the year.
