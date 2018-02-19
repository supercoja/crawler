namespace CrawlerDemo
{
    public class ResultScrap
    {
        public string PhoneNumber { get; private set; }
        public string Image { get; private set; }
        public string Email { get; private set; }

        public void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public void SetImage(string image)
        {
            Image = image;
        }

        public void SetEmail(string email)
        {
            Email = email;
        }

        public bool IsValidResult()
        {
            var isValid = true;
            if (PhoneNumber == null)
                isValid = false;
            if (Image == null)
                isValid = false;
            if (Email == null)
                isValid = false;

            return isValid;
        }
    }
}
