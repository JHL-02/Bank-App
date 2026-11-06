namespace Bank_App.Users
{
    internal class PersonalDetails
    {
        private string _email = "";
        private string _phone = "";
        private string _name = "";

        public string Email => string.IsNullOrWhiteSpace(_email) ? "N/A" : _email;
        public string Phone => string.IsNullOrWhiteSpace(_phone) ? "N/A" : _phone;
        public string Name => string.IsNullOrWhiteSpace(_name) ? "N/A" : _name;

        public void SetEmail(string email) => _email = email;
        public void SetPhone(string phone) => _phone = phone;
        public void SetName(string name) => _name = name;
    }
}
