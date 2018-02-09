using CrawlerDemo;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class Class1
    {

        [Fact]
        public void PhoneRegExpPhoneWithTraceValidate()
        {
            var _phoneToValidate = new StringBuilder();
            _phoneToValidate.Append("(11) 4009-1211");
            _phoneToValidate.Append("(54) 3333-1211");
            _phoneToValidate.Append("(11)93333-1211");
            var _phoneValues = ExtractData.ExtractValidPhones(_phoneToValidate.ToString());

            Assert.Equal(3, _phoneValues.Count);
        }

        [Fact]
        public void PhoneRegExpPhoneWithoutSpaceAndTrace()
        {
            var _phoneToValidate = new StringBuilder();
            _phoneToValidate.Append("(11) 40091211");
            _phoneToValidate.Append("(54)33331211");

            var _phoneValues = ExtractData.ExtractValidPhones(_phoneToValidate.ToString());

            Assert.Equal(2, _phoneValues.Count);
        }

        [Fact]
        public void PhoneRegExpWorldWidePhone()
        {
            var _phoneToValidate = new StringBuilder();
            _phoneToValidate.Append("+55 (22) 4111-1211");
            _phoneToValidate.Append("55 (48)3131-0800");

            var _phoneValues = ExtractData.ExtractValidPhones(_phoneToValidate.ToString());

            Assert.Equal(2, _phoneValues.Count);
        }

        [Fact]
        public void PhoneRegExpPhoneWithPoint()
        {
            var _phoneToValidate = new StringBuilder();
            _phoneToValidate.Append("(54) 4111.1211");
            _phoneToValidate.Append("11 31310800");

            var _phoneValues = ExtractData.ExtractValidPhones(_phoneToValidate.ToString());

            Assert.Equal(2, _phoneValues.Count);
        }


    }
}
