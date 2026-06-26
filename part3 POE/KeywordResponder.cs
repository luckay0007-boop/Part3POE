using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPOE
{

    public class KeywordResponder
    {

        private Dictionary<string, List<string>> _responses;

        private Dictionary<string, string> _keywordAliases;

        private Random _random;

        public KeywordResponder()
        {
            _random = new Random();
            InitializeResponses();
            InitializeAliases();
        }


        private void InitializeResponses()
        {
            _responses = new Dictionary<string, List<string>>
            {
                { "password", new List<string> {
                    " Use at least 12 characters in your password. Mix uppercase letters, numbers, and symbols to make it much harder to crack.",
                    " A strong password kept longer is better than a weak one changed often. Only change it if you suspect it's been compromised.",
                    " Never reuse passwords across accounts! Credential stuffing attacks let hackers access multiple accounts with one stolen password.",
                    " Password managers are highly recommended — they use strong encryption and are much safer than reusing passwords across sites.",
                    " Consider using passphrases instead of passwords. Long phrases like 'correct-horse-battery-staple' are harder to crack but easier to remember.",
                    " Hackers use brute force attacks that try all possible combinations automatically. Longer passwords exponentially increase the time needed to crack them.",
                    " Check if your password has been leaked at HaveIBeenPwned.com — it's a free and trusted service used by millions.",
                    " Avoid common passwords like '123456' or 'password' — hackers try these first in dictionary attacks.",
                    " Adding '1' or '!' to the end of a password is predictable. Use random placement of special characters instead for better security.",
                    " Make sure to use strong, unique passwords for each account. Avoid using personal details like birthdays or pet names in your passwords."
                }},

                { "phishing", new List<string> {
                    " Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations like banks or government agencies.",
                    " Always check the sender's email address carefully  phishing emails often use addresses that look similar to legitimate ones but have subtle differences.",
                    " Don't click suspicious links! Hover over them first to see the actual URL destination before clicking anything.",
                    " If you clicked a phishing link but didn't enter any info, close it immediately and scan your device. You're likely safe, but stay vigilant.",
                    " Spear phishing is a targeted attack using your personal info gathered from social media. Be cautious even with emails that seem to know you.",
                    " The lock in your browser only means the connection is encrypted — it does NOT mean the site is safe or legitimate!",
                    " Smishing is phishing via SMS text messages. Don't click links in unexpected text messages from unknown numbers.",
                    " Phishing emails often create urgency ('Your account will be closed in 24 hours!') to make you panic and act without thinking.",
                    " Hackers get your email address from data breaches and publicly available information. Consider using email aliases for different services.",
                    " Vishing is phishing via phone calls. Remember: banks and companies will never call you unsolicited asking for your password or PIN."
                }},

                { "2fa", new List<string> {
                    " Two-Factor Authentication (2FA) adds a second layer of security even if your password is stolen. Always enable it where available!",
                    " Use authenticator apps like Google Authenticator or Authy instead of SMS-based 2FA — they're more secure against SIM-swap attacks.",
                    " Always save your backup codes when setting up 2FA. Store them offline in a secure location — they're your lifeline if you lose your phone!",
                    " While rare, hackers can bypass 2FA with advanced social engineering tricks. That's why using multiple security layers is always recommended.",
                    " Hardware security keys like YubiKey are the most secure form of 2FA — they're physical devices that plug into your computer for login verification.",
                    " 2FA codes change every 30 seconds because they use Time-based One-Time Passwords (TOTP) for maximum security.",
                    " Biometrics (fingerprint, face scan) count as a form of 2FA — it combines 'something you are' with 'something you know' (your password).",
                    " Enable 2FA at minimum for your email, banking, and social media accounts — these are the most critical targets for hackers.",
                    " If you lose your phone with 2FA, backup codes are your lifeline to regain access. Always keep them stored safely offline!"
                }},

                { "viruses", new List<string> {
                    " Keep your software and operating system updated — updates often patch security vulnerabilities that viruses exploit to infect your device.",
                    " Virus vs Worm vs Trojan: A virus attaches to files, a worm spreads on its own across networks, and a trojan disguises itself as legitimate software.",
                    " Signs of a virus infection include: slow computer performance, frequent crashes, strange popups, and unexpected system behaviour.",
                    " Yes, phones can get viruses too! Especially from downloading apps outside official app stores like Google Play or the App Store.",
                    " Ransomware locks your files and demands payment to unlock them. Never pay — there's no guarantee you'll get your files back!",
                    " Macs are safer than Windows PCs statistically, but they are NOT immune to malware. No system is 100% secure.",
                    " A keylogger is malware that secretly records every keystroke you type — including passwords and credit card numbers!",
                    " PDFs, images, and Office documents can contain hidden malware scripts. Only open files from trusted and verified sources.",
                    " Incognito/private browsing mode does NOT protect against viruses — it only hides your browsing history from other users of the same device.",
                    " A zero-day vulnerability is a security flaw with no available fix yet. Keep your antivirus updated to catch as many known threats as possible."
                }},

                { "scam", new List<string> {
                    " If an offer sounds too good to be true, it probably is! Scammers use unrealistic promises to lure victims into giving up money or personal info.",
                    " Never send money to someone you've only met online. Romance scams are one of the most costly types of online fraud worldwide.",
                    " Government agencies will never call you demanding immediate payment or threatening arrest. That's always a scam — hang up immediately!",
                    " Be wary of tech support scams — Microsoft, Apple, and Google will never call you unsolicited about computer problems.",
                    " Check website URLs carefully before entering any information. Scam sites often mimic real ones with slight spelling changes (e.g., 'amaz0n.com').",
                    " Social media scams are on the rise — be cautious of fake giveaways, 'investment opportunities', and friend requests from strangers.",
                    " Never share your OTP (One-Time Password) with anyone, even if they claim to be from your bank. Legitimate services will never ask for it.",
                    " Job scams often ask for upfront payments or excessive personal details before any real interview. Legitimate employers don't operate this way."
                }},

                { "privacy", new List<string> {
                    " Review the privacy settings on all your social media accounts regularly. Limit what strangers can see about your personal life.",
                    " Use a VPN (Virtual Private Network) when connected to public Wi-Fi to encrypt your internet traffic and protect your data.",
                    " Be cautious about what you share online — once it's posted, it can be very difficult (or impossible) to remove completely.",
                    " Read privacy policies before signing up for online services. Know what data they collect and how they plan to use it.",
                    " Use encrypted messaging apps like Signal for sensitive conversations. End-to-end encryption means only you and the recipient can read the messages.",
                    " Regularly check what apps have access to your camera, microphone, and location. Revoke unnecessary permissions to protect your privacy.",
                    " Consider using a privacy-focused browser like Brave or Firefox with enhanced tracking protection enabled for everyday browsing.",
                    " Data brokers collect and sell your personal information to advertisers. You can opt out from many of them to reduce your digital footprint."
                }}
            };
        }


        private void InitializeAliases()
        {
            _keywordAliases = new Dictionary<string, string>
            {
                { "password safety", "password" },
                { "password", "password" },
                { "passwords", "password" },
                { "passphrase", "password" },
                { "phishing tips", "phishing" },
                { "phishing tip", "phishing" },
                { "phishing", "phishing" },
                { "phish", "phishing" },
                { "scam email", "phishing" },
                { "fake email", "phishing" },
                { "two-factor authentication", "2fa" },
                { "two factor authentication", "2fa" },
                { "two-factor", "2fa" },
                { "two factor", "2fa" },
                { "2fa", "2fa" },
                { "mfa", "2fa" },
                { "authenticator", "2fa" },
                { "virus", "viruses" },
                { "viruses", "viruses" },
                { "malware", "viruses" },
                { "trojan", "viruses" },
                { "worm", "viruses" },
                { "ransomware", "viruses" },
                { "keylogger", "viruses" },
                { "scam", "scam" },
                { "scams", "scam" },
                { "fraud", "scam" },
                { "online scams", "scam" },
                { "privacy", "privacy" },
                { "private", "privacy" },
                { "data protection", "privacy" },
                { "personal data", "privacy" },
                { "vpn", "privacy" }
            };
        }


        public string? FindKeyword(string lowerInput)
        {
            var sortedAliases = _keywordAliases.Keys.OrderByDescending(k => k.Length);

            foreach (string alias in sortedAliases)
            {
                if (lowerInput.Contains(alias))
                {
                    return _keywordAliases[alias];
                }
            }

            return null;
        }


        public string GetResponse(string keyword, List<int>? excludeIndices = null)
        {
            var result = GetResponseWithIndex(keyword, excludeIndices);
            return result.response;
        }

        public (int index, string response) GetResponseWithIndex(string keyword, List<int>? excludeIndices = null)
        {
            if (!_responses.ContainsKey(keyword))
                return (-1, "I don't have information on that topic yet.");

            var responses = _responses[keyword];

            if (excludeIndices != null && excludeIndices.Count < responses.Count)
            {
                var available = Enumerable.Range(0, responses.Count)
                    .Where(i => !excludeIndices.Contains(i))
                    .ToList();

                if (available.Count > 0)
                {
                    int index = available[_random.Next(available.Count)];
                    return (index, responses[index]);
                }
            }

            int idx = _random.Next(responses.Count);
            return (idx, responses[idx]);
        }


        public List<string> GetTopics()
        {
            return new List<string>(_responses.Keys);
        }


        public int GetResponseCount(string keyword)
        {
            return _responses.ContainsKey(keyword) ? _responses[keyword].Count : 0;
        }
    }
}
