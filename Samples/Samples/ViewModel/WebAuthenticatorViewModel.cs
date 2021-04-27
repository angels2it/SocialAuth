using System;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Samples.ViewModel
{
    public class WebAuthenticatorViewModel : BaseViewModel
    {
        const string authenticationUrl = "https://social-auth.daivietgo.vn/mobileauth/";

        public WebAuthenticatorViewModel()
        {
            MicrosoftCommand = new Command(async () => await OnAuthenticate("Microsoft"));
            LinkedInCommand = new Command(async () => await OnAuthenticate("LinkedIn"));
            GoogleCommand = new Command(async () => await OnAuthenticate("Google"));
            FacebookCommand = new Command(async () => await OnAuthenticate("Facebook"));
            AppleCommand = new Command(async () => await OnAuthenticate("Apple"));
        }

        public ICommand MicrosoftCommand { get; }
        public ICommand LinkedInCommand { get; }

        public ICommand GoogleCommand { get; }

        public ICommand FacebookCommand { get; }

        public ICommand AppleCommand { get; }

        string accessToken = string.Empty;

        public string AuthToken
        {
            get => accessToken;
            set => SetProperty(ref accessToken, value);
        }

        string email = string.Empty;

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        async Task OnAuthenticate(string scheme)
        {
            try
            {
                WebAuthenticatorResult r = null;

                if (false && scheme.Equals("Apple")
                    && DeviceInfo.Platform == DevicePlatform.iOS
                    && DeviceInfo.Version.Major >= 13)
                {
                    r = await AppleSignInAuthenticator.AuthenticateAsync();
                }
                else
                {
                    var authUrl = new Uri(authenticationUrl + scheme);
                    var callbackUrl = new Uri("xamarinessentials://");

                    r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                }
                var rawEmail = r?.Get("email");
                if (!string.IsNullOrEmpty(rawEmail))
                {
                    Email = HttpUtility.UrlDecode(rawEmail);
                }
                AuthToken = r?.AccessToken ?? r?.IdToken;
            }
            catch (Exception ex)
            {
                AuthToken = string.Empty;
                await DisplayAlertAsync($"Failed: {ex.Message}");
            }
        }
    }
}
