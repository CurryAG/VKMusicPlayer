using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace VKMusicPlayer
{
    public class VK
    {
        public static string Login;
        public static string Password;
        const int AppID = 51502358;
        public VkApi vkApi;
        public VK()
        {
            ServiceCollection service = new ServiceCollection();
            service.AddAudioBypass();
            vkApi = new VkApi(service);
        }
        public void Auth(string login, string password)
        {
            vkApi.Authorize(new ApiAuthParams
            {
                Login = login,
                Password = password,
                ApplicationId = AppID,
                Settings = Settings.All | Settings.Offline
            });
        }
        public void TokenAuth(string token)
        {
            vkApi.Authorize(new ApiAuthParams
            {
                AccessToken = token,
                ApplicationId = AppID,
                Settings = Settings.All | Settings.Offline
            });
        }
        public VkCollection<Audio> GetAudio()
        {
            VkCollection<Audio> audio = vkApi.Audio.Get(new AudioGetParams
            {
                OwnerId = vkApi.UserId.Value,
                Count = 6000,
            }) ;

            return audio;
        }
    }
}
