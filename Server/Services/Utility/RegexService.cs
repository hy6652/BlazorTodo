using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BlazorTodo.Server.Services.Utility
{
    public class RegexService
    {
        public RegexService()
        {

        }

        public List<string> CheckText(string text)
        {
            List<string> emailList = CheckEmail(text);
            List<string> phoneNumberList = CheckPhoneNumber(text);
            List<string> registrationNumberLIst = CheckRegistrationNumber(text);
            List<string> cardNumberList = CheckCardNumber(text);

            return emailList.Concat(phoneNumberList).Concat(registrationNumberLIst).Concat(cardNumberList).ToList();
        }

        public List<string> CheckEmail(string text)
        {
            List<string> regexStrings = new List<string>();
            string pattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";

            foreach (Match match in Regex.Matches(text, pattern))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }

        public List<string> CheckPhoneNumber(string text)
        {
            List<string> regexStrings = new List<string>();
            string pattern = @"01[016789]-?\d{3,4}-?\d{4}";

            foreach (Match match in Regex.Matches(text, pattern))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }

        public List<string> CheckRegistrationNumber(string text)
        {
            List<string> regexStrings = new List<string>();
            string pattern3 = @"(\d{2})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|[3][01])-?([1-4][0-9]{6})";

            foreach (Match match in Regex.Matches(text, pattern3))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }

        public List<string> CheckCardNumber(string text)
        {
            List<string> regexStrings = new List<string>();
            string pattern = @"([34569]\d{3})-?(\d{4})-?(\d{4})-?(\d{4})";  // 16자리 숫자 기준

            foreach (Match match in Regex.Matches(text, pattern))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }
    }
}
