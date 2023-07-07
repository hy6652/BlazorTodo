using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BlazorTodo.Server.Services
{
    public class RegexService
    {
        public RegexService()
        {

        }

        public async Task<List<string>> CheckText(string text)
        {
            List<string> emailData = await CheckEmailRegex(text);
            List<string> phoneNumberData = await CheckPhoneNumberRegex(text);
            List<string> registrationNumberData = await CheckRegistrationNumber(text);

            return emailData.Concat(phoneNumberData).Concat(registrationNumberData).ToList();
        }

        public async Task<List<string>> CheckEmailRegex(string text)
        {
            List<string> regexStrings = new List<string>();
            string emailRegex = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";

            foreach (Match match in Regex.Matches(text, emailRegex))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }

        public async Task<List<string>> CheckPhoneNumberRegex(string text)
        {
            List<string> regexStrings = new List<string>();
            string phoneNumberPattern = @"01[016789]\d{7,8}|01[016789]-\d{3,4}-\d{4}";

            foreach (Match match in Regex.Matches(text, phoneNumberPattern))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }

        public async Task<List<string>> CheckRegistrationNumber(string text)
        {
            List<string> regexStrings = new List<string>();

            string pattern = @"(\d{6})-[1-4]\d{6}";
            Regex regex = new Regex(pattern);

            foreach (Match match in Regex.Matches(text, pattern))
            {
                regexStrings.Add(match.Value);
            }
            return regexStrings;
        }
    }
}
