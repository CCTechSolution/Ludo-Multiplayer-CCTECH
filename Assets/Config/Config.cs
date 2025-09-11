using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

[System.Serializable]
public class Config : ScriptableObject
{
    #region DATA KEYS
    internal const string LOGIN_TYPE_KEY = "LOGIN_TYPE";
    internal const string GUEST_KEY = "GUEST";//guest
    internal const string FACEBOOK_KEY = "FACEBOOK";
    internal const string APPLE_KEY = "APPLE";
    internal const string GOOGLE_KEY = "GOOGLE";
    internal const string EMAIL_ACCOUNT_KEY = "EMAIL_ACCOUNT";
    internal const string EMAIL_ID_KEY = "EMAIL_ID";
    internal const string PASSWORD_KEY = "PASSWORD";
    internal const string REMEMBER_ME_KEY = "REMEMBER_ME";
    internal const string PLAYER_ONLINE_KEY = "PLAYER_ONLINE";
    internal const string PLAYER_NAME_KEY = "PLAYER_NAME";

    internal const string TITLE_FIRST_LOGIN_KEY = "TITLE_FIRST_LOGIN";
    internal const string TOTAL_EARNINGS_KEY = "TOTAL_EARNINGS";
    internal const string GAMES_PLAYED_KEY = "GAMES_PLAYED";
    internal const string TWO_PLAYER_WINS_KEY = "TWO_PLAYER_WINS";
    internal const string FOUR_PLAYER_WINS_KEY = "FOUR_PLAYER_WINS";
    internal const string TOTAL_WINS_KEY = "TOTAL_WINS_WINS";
    internal const string COINS_KEY = "COINS";
    internal static string GEMS_KEY = "GEMS";
    internal const string CHAT_KEY = "CHAT";
    internal const string EMOJI_KEY = "EMOJI";
    internal const string AVATAR_INDEX_KEY = "AVATAR_INDEX";
    internal const string AVATAR_FRAME_INDEX_KEY = "AVATAR_FRAME_INDEX";

    internal const string FORTUNE_WHEEL_LAST_FREE_TIME_KEY = "FORTUNE_WHEEL_LAST_FREE_TIME";
    internal const string FORTUNE_WHEEL_SPIN_KEY = "FORTUNE_WHEEL_SPIN";
    internal const string FACEBOOK_ID_KEY = "FACEBOOK_ID";
    internal const string FACEBOOK_TOKEN_KEY = "FACEBOOK_TOKEN";
    internal const string APPLE_TOKEN_KEY = "APPLE_TOKEN";
    internal const string PLAYER_AVATAR_URL_KEY = "PLAYER_AVATAR_URL";
    internal const string UNIQUE_IDENTIFIER_KEY = "UNIQUE_IDENTIFIER";

    internal const string SOUND_VOLUME_KEY = "SOUND_VOLUME";
    internal const string MUSIC_VOLUME_KEY = "MUSIC_VOLUME";
    internal const string VIBRATION_KEY = "VIBRATION";
    internal const string NOTIFICATIONS_KEY = "NOTIFICATIONS";
    internal const string FRIEND_REQUEST_KEY = "FRIEND_REQUEST";
    internal const string PRIVATE_ROOM_KEY = "PRIVATE_ROOM";
    internal const string LANGUAGE_KEY = "LANGUAGE";



    internal const string REMOVE_ADS_KEY = "REMOVE_ADS";
    internal const string AVATARS_KEY = "AVATARS";
    internal const string AVATAR_FRAMES_KEY = "AVATAR_FRAMES";
    internal const string PLAYER_ID_KEY = "PLAYER_ID";
    internal const string PLAYER_LEVEL_KEY = "PLAYER_LEVEL";
    internal const string PLAYER_XP_KEY = "PLAYER_XP";
    internal const string PLAYER_COUNTRY_KEY = "PLAYER_COUNTRY";
    internal const string PLAYER_DICE_KEY = "PLAYER_DICE";
    internal const string PLAYER_PAWN_KEY = "PLAYER_PAWN";
    internal const string DICES_KEY = "DICES";
    internal const string PAWNS_KEY = "PAWNS";
    internal const string CHESTS_KEY = "CHESTS";
    internal const string INBOX_KEY = "INBOX_MESSAGES";

    internal const string NEXT_FREE_COINS_TIME_NAME = "LastFreeCoinsTimeTicks";
    internal const string LAST_AD_COINS_TIME_NAME = "LastAdCoinsTimeTicks";
    internal const string LAST_AD_REWARD = "LastAdReward";

    internal const string AppleUserId_FullName = "AppleUserId_FullName";
    internal const string AppleUserId_Email = "AppleUserId_Email";
    internal const string AppleUserId_IdentityToken = "AppleUserId_IdentityToken";

    #endregion DATA KEYS

    public static float FriendRefreshTime = 180;

    public static float BannerDelayTime = 6;

    [SerializeField]
    public string BuildVersion => Application.version;
    [HideInInspector]
    public string BuildCode { get; set; }
    public static bool IsFourPlayerModeEnabled = true;




    public static string AndroidPackageName => Application.identifier;
    public const string ITunesAppID = "6504749341";

    // Notifications
    public static string notificationTitle => Application.productName;// "Modern Ludo Online";
    public const string notificationMessage = "Get your FREE Spin!";


    // Game configuration
    public const float WaitTimeUntilStartWithBots = /*1*/5.0f; // Time in seconds. If after that time new player doesnt join room game will start with bots

    // Services configration IDS
    public const string PlayFabTitleID = "F7105";
    public const string PhotonAppID = "81c9cad6-f914-46f0-9761-2bfbf6201f39";
    public const string PhotonChatID = "";

    public static string CustomPlayerID = "";
    // Admob Ads IDS
    public const string adMobAndroidID = "";
    public const string adMobiOSID = "";

    // Facebook share variables
    public const string facebookShareLinkTitle = "I'm playing this awesome game!. Available on Android and iOS.";
    // Facebook Invite variables
    public static string facebookInviteMessage = "Come play this great game!";
    public static int rewardCoinsForFriendInvite = 250;
    public static int rewardCoinsForShareViaFacebook = 50;

    // Share private room code
    public const string SharePrivateLinkMessage = "Let's play {0} with me. My ROOM CODE is: {1}";
    public const string SharePrivateLinkMessage2 = "Tap on link below to Download it:";
    public const string ShareScreenShotText = "I finished game. It's my score :-) Join me and download this Ludo game free :";

    // Photon configuration
    // Timeout in second when player will be disconnected when game in background
    internal const float photonDisconnectTimeout = 300.00f; // In game scene - its better to don't change it. Player that loose focus on app will be immediately disconnected
    internal const float photonDisconnectTimeoutLong = 300.01f; // In menu scene etc.



    public List<Sprite> botAvatars;
    //[HideInInspector]
    public List<Sprite> customAvatars;

    public List<Sprite> customAvatarFrames;



    public List<Dice> customDices;
    public List<Pawn> customPawns;

    public List<Chest> mChests;
    [HideInInspector]
    public Sprite[] rankBadges;
    [HideInInspector]
    public string[] rankNames = { "Beginner", "Trainee", "Explorer", "Hero", "Veteran", "Guru", "Lord", "Champion", "Elite", "Super Star", "Celebrity", "Phenomenon", "Legend", };

    //[HideInInspector]
    public string[] RegionNames = { "Islamabad Club", "Delhi Lounge", "Tokyo Arena", "London Palace", "Paris Galary", "Istanbul Theatre", "New York Park", "Hong Kong Stage" };


    public Sprite[] emojis;


    public string[] LANGUAGES_LIST = { "English", "Europe", "Arab", "Africa", "English", "US", "Japan", "Indea", "Europe", "Arab", "Africa", "English", "US", "Japan", "Indea", "Europe", "Arab", "Africa", "English", "US", "Japan", "Indea" };


    // Hide Coins tab in shop (In-App Purchases)
    public static bool hideCoinsTabInShop = false;
    public static string runOutOfTime = "ran out of time";
    public static string waitingForOpponent = "Waiting for your opponent";

    // Other strings
    public static string youAreBreaking = "You start, good luck";
    public static string opponentIsBreaking = "is starting";
    public static string IWantPlayAgain = "I want to play again!";
    public static string cantPlayRightNow = "Can't play right now";

    // Players names for training mode
    public static string offlineModePlayer1Name = "Player 1";
    public static string offlineModePlayer2Name = "Player 2";


    // Standard chat messages
    public static string[] chatMessages = new string[] {
            "Please don't kill",
            "Play Fast",
            "I will eat you",
            "You are good",
            "Well played",
            "Today is your day",
            "Hehehe",
            "Unlucky",
            "Thanks",
            "Yeah",
            "Remove Blockade",
            "Good Game",
            "Oops",
            "Today is my day",
            "All the best",
            "Hi",
            "Hello",
            "Nice move"
        };

    // Additional chat messages
    // Prices for chat packs
    public static int[] chatPrices = new int[] { 1000, 5000, 10000, 50000, 100000, 250000 };
    public static int[] emojisPrices = new int[] { 1000, 5000, 10000, 50000, 100000 };

    // Chat packs names
    public static string[] chatNames = new string[] { "Motivate", "Emoticons", "Cheers", "Gags", "Laughing", "Talking" };

    // Chat packs strings
    public static string[][] chatMessagesExtended = new string[][] {
            new string[] {
                "Never give up",
                "You can do it",
                "I know you have it in you!",
                "You play like a pro!",
                "You can win now!",
                "You're great!"
            },
            new string[] {
                ":)",
                ":(",
                ":o",
                ";D",
                ":P",
                ":|"
            },
            new string[] {
                "Keep it going",
                "Go opponents!",
                "Fabulastic",
                "You're awesome",
                "Best shot ever",
                "That was amazing",
            },
            new string[] {
                "OMG",
                "LOL",
                "ROFL",
                "O'RLY?!",
                "CYA",
                "YOLO"
            },
            new string[] {
                "Hahaha!!!",
                "Ho ho ho!!!",
                "Mwhahahaa",
                "Jejeje",
                "Booooo!",
                "Muuuuuuuhhh!"
            },
            new string[] {
                "Yes",
                "No",
                "I don't know",
                "Maybe",
                "Definitely",
                "Of course"
            }
        };

    internal Sprite GetAvatar(int avatarIndex)
    {
        return (avatarIndex > 0 && avatarIndex < customAvatars.Count) ? customAvatars[avatarIndex] : customAvatars[0];
    }


    // Bids Values
    public List<BidCard> bidCards;// = new int[] { 500, 700, 1000, 1200, 1500, 2000, 2500, 5000 };


    public List<InAppItem> InAppItems;

    public InAppItem GetInAppItem(IAP_ITEM_INDEX index) {

        if (Application.platform == RuntimePlatform.IPhonePlayer)
            index = (IAP_ITEM_INDEX)Enum.Parse(typeof(IAP_ITEM_INDEX), index.ToString()/*+"iOS"*/,true);


        return InAppItems[(int)index];
    }

    public enum IAP_ITEM_INDEX
    {
          ludo_gems_pack_1 =0
        , ludo_gems_pack_2 =1
        , ludo_gems_pack_3 =2
        , ludo_gems_pack_4 =3
        , ludo_gems_pack_5 =4
        , ludo_remove_ads  =5
        , ludo_gems_discount_1 =6
        , ludo_coins_discount_1=7

        , ludo_gems_pack_1iOS = 8
        , ludo_gems_pack_2iOS = 9
        , ludo_gems_pack_3iOS = 10
        , ludo_gems_pack_4iOS = 11
        , ludo_gems_pack_5iOS = 12
        , ludo_remove_adsiOS = 13
        , ludo_gems_discount_1iOS = 14
        , ludo_coins_discount_1iOS = 15
    };


    private Color colorRed = new Color(250.0f / 255.0f, 12.0f / 255, 12.0f / 255);
    private Color colorBlue = new Color(0, 86.0f / 255, 255.0f / 255);
    private Color colorYellow = new Color(255.0f / 255.0f, 163.0f / 255, 0);
    private Color colorGreen = new Color(8.0f / 255, 174.0f / 255, 30.0f / 255);

    [Header("Time For Free Coins")]
    public int TimerMaxHours;
    [RangeAttribute(0, 59)]
    public int TimerMaxMinutes;
    [RangeAttribute(0, 59)]
    public int TimerMaxSeconds = 10;

    [Header("Time For Free Rewards")]
    public int adTimerMaxHours;
    [RangeAttribute(0, 59)]
    public int adTimerMaxMinutes;
    [RangeAttribute(0, 59)]
    public int adTimerMaxSeconds = 10;

    public Color GetColor(ColorNames _color)
    {
        switch (_color)
        {
            case ColorNames.yellow:
                return Color.yellow;
            case ColorNames.green:
                return Color.green;
            case ColorNames.blue:
                return Color.blue;
            case ColorNames.red:
                return Color.red;
            default:
                return Color.yellow;
        }

    }

    public string Mode
    {
        get
        {
            switch (GameManager.Instance.Mode)
            {
                case GameMode.Classic:
                    return "Classic";
                case GameMode.Master:
                    return "Master";
                case GameMode.Quick:
                    return "Quick";
            }
            return "";
        }
    }

    public string ModeHints
    {
        get
        {
            switch (GameManager.Instance.Mode)
            {
                case GameMode.Classic:
                    return "score 4 pucks";
                case GameMode.Master:
                    return "score 4 pucks";
                case GameMode.Quick:
                    return "score 2 pucks";
            }
            return "";
        }
    }
    public Sprite[] modeIcons;


    internal Sprite RandomAvatar(int index = -1)
    {
        if (index == -1)
            UnityEngine.Random.Range(0, this.botAvatars.Count - 1);
        return this.botAvatars[index];
    }

    public Sprite ModeIcon
    {
        get
        {
            switch (GameManager.Instance.Mode)
            {
                case GameMode.Classic:
                    return modeIcons[0];
                case GameMode.Master:
                    return modeIcons[1];
                case GameMode.Quick:
                    return modeIcons[2];
            }
            return modeIcons[0];
        }
    }


    public void GetAvatar(Image avatarImg, int id, bool isPlayer= true)
    {
        GetAvatar((spr) => avatarImg.sprite = spr, id, isPlayer);
    }

    public void GetAvatar(Action<Sprite> OnAvatar, int id, bool isPlayer=true) {

        switch (id)
        {
            case -1:
            case 0:
                {

                    if (isPlayer && string.IsNullOrEmpty(GameManager.Instance.PlayerData.AvatarUrl))
                        OnAvatar?.Invoke(customAvatars[0]);
                    else
                        FacebookAvatar(OnAvatar,id, isPlayer);
                }
                break;
            default:
                OnAvatar?.Invoke((id > 0 && id < customAvatars.Count) ? customAvatars[id] : customAvatars[0]);
                break;

        }
    }


    

    private void FacebookAvatar(Action<Sprite> OnAvatar, int id, bool isPlayer = true)
    {
        if (isPlayer)
        {
            if (GameManager.Instance.FacebookAvatar != null)
                OnAvatar?.Invoke(GameManager.Instance.FacebookAvatar);
            else if (!string.IsNullOrEmpty(GameManager.Instance.PlayerData.AvatarUrl))
            {
                //FacebookManager.Instance.loadMyAvatar(GameManager.Instance.PlayerData.AvatarUrl, true);
                FacebookManager.Instance.StartCoroutine(loadMyAvatarAsync(OnAvatar, GameManager.Instance.PlayerData.AvatarUrl, true));
            }
        }
        else
        {
            OnAvatar?.Invoke(customAvatars[0]);
        }
            ;//NA. HANDLE Opp. FACEBOOK AVATAR URL 
    }

    IEnumerator loadMyAvatarAsync(Action<Sprite> OnAvatar, string url, bool ForceReload)
    {
        OnAvatar(customAvatars[0]);//PLACE HOLDER

        if (GameManager.Instance.IsConnected == false)
        {
            GameManager.Instance.PlayerData.AvatarUrl = "";
            GameManager.Instance.FacebookAvatar = null;
            yield break;
        }

        //if (ForceReload || StaticDataController.Instance.mConfig.FacebookAvatar == null)
        //{

        WWW www = new WWW(url);
        if (www != null)
        {
            yield return www;
            if (www.texture != null)
            {
                GameManager.Instance.FacebookAvatar = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
                Debug.Log("My avatar => " + www.texture.name + " =>" + url);
                yield return new WaitForEndOfFrame();

               
            }
            else
            {
                GameManager.Instance.PlayerData.AvatarUrl = "";
                GameManager.Instance.FacebookAvatar = null;
            }
        }
        else
        {
            GameManager.Instance.PlayerData.AvatarUrl = "";
            GameManager.Instance.FacebookAvatar = null;
        }

        OnAvatar?.Invoke(GameManager.Instance.FacebookAvatar);//CAN BE NULL

        yield return new WaitForEndOfFrame();
    }


    public Sprite GetFrame(int id)
    {
        return (id >= 0 && id < customAvatarFrames.Count) ? customAvatarFrames[id] : customAvatarFrames[0];
    }
    public Sprite GetRandomFrame()
    {
        var index = UnityEngine.Random.Range(0, customAvatarFrames.Count);
        return customAvatarFrames[index];
    }
    public int GetRandomPawn()
    {
        return UnityEngine.Random.Range(0, customPawns.Count);
    }

    public int GetRandomDice()
    {
        return UnityEngine.Random.Range(0, customDices.Count);
    }

    public Sprite GetCountryFlag(string _countryCode)
    {
        List<Sprite> flags = Resources.LoadAll<Sprite>("Flags").ToList<Sprite>();
        Sprite flg = flags.FirstOrDefault(x => x.name == "flag_" + _countryCode.ToUpper());
        if (flg == null)
            flg = flags.FirstOrDefault(x => x.name == "flag_DEFAULT");
        return flg;
    }


    public Sprite GetRankBadge(int level)
    {
        if (level >= 0 && level < 3)
            return rankBadges[0];
        else if (level >= 3 && level < 6)
            return rankBadges[1];
        else if (level >= 6 && level < 11)
            return rankBadges[2];
        else if (level >= 11 && level < 16)
            return rankBadges[3];
        else if (level >= 16 && level < 21)
            return rankBadges[4];
        else if (level >= 21 && level < 31)
            return rankBadges[5];
        else if (level >= 31 && level < 41)
            return rankBadges[6];
        else if (level >= 41 && level < 56)
            return rankBadges[7];
        else if (level >= 56 && level < 71)
            return rankBadges[8];
        else if (level >= 71 && level < 91)
            return rankBadges[9];
        else if (level >= 91 && level < 111)
            return rankBadges[10];
        else if (level >= 111 && level < 136)
            return rankBadges[11];
        else if (level >= 136 && level < 151)
            return rankBadges[12];
        else if (level >= 151 && level < 176)
            return rankBadges[13];
        else if (level >= 176 && level < 201)
            return rankBadges[14];
        else if (level >= 201)
            return rankBadges[15];

        return rankBadges[0];
    }

    public string GetRankName(int level)
    {
        if (level < 3)
            return rankNames[0];
        else if (level >= 3 && level < 6)
            return rankNames[1];
        else if (level >= 6 && level < 11)
            return rankNames[2];
        else if (level >= 11 && level < 16)
            return rankNames[3];
        else if (level >= 16 && level < 21)
            return rankNames[4];
        else if (level >= 21 && level < 31)
            return rankNames[5];
        else if (level >= 31 && level < 41)
            return rankNames[6];
        else if (level >= 41 && level < 56)
            return rankNames[7];
        else if (level >= 56 && level < 71)
            return rankNames[8];
        else if (level >= 71 && level < 91)
            return rankNames[9];
        else if (level >= 91 && level < 111)
            return rankNames[10];
        else if (level >= 111 && level < 136)
            return rankNames[11];
        else if (level >= 136)
            return rankNames[12];

        return rankNames[0];
    }

    public string GetIniqueId()
    {
        string ticks = DateTime.Now.Ticks.ToString();
        System.Random random = new System.Random();
        string str1 = new string(ticks.Select(c => ticks[random.Next(ticks.Length)]).Take(4).ToArray());
        string str2 = new string(DateTime.UtcNow.Ticks.ToString().Select(c => DateTime.UtcNow.Ticks.ToString()[random.Next(DateTime.UtcNow.Ticks.ToString().Length)]).Take(4).ToArray()); ;
        string str3 = new string(ticks.Select(c => ticks[random.Next(ticks.Length)]).Take(4).ToArray());

        string uid = string.Format("FCLG-{0}-{1}-{2}-{3:0000}", str1, str2, str3, DateTime.UtcNow.Millisecond);

        if (PlayerPrefs.HasKey(UNIQUE_IDENTIFIER_KEY))
            return PlayerPrefs.GetString(UNIQUE_IDENTIFIER_KEY, uid);
        else
        {
            if (string.IsNullOrEmpty(CustomPlayerID))
                CustomPlayerID = uid;
            PlayerPrefs.SetString(UNIQUE_IDENTIFIER_KEY, CustomPlayerID);
            PlayerPrefs.Save();
            return CustomPlayerID;
        }

    }


    public string GetAbreviation(int value)
    {
        int NrOfDigits = GetNumberOfDigits(value);
        float FirstDigits;
        string Abreviation = "";
        //Debug.Log(NrOfDigits);
        if (NrOfDigits % 3 == 0)
        {
            FirstDigits = value / (Mathf.Pow(10, NrOfDigits - 3));
            if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000 == 1)
            {
                Abreviation = " K";
            }
            else if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000000 == 1)
            {
                Abreviation = " M";
            }
            else if ((Mathf.Pow(10, NrOfDigits - 3)) / 1000000000 == 1)
            {
                Abreviation = " B";
            }
        }
        else
        {
            FirstDigits = value / (Mathf.Pow(10, NrOfDigits - NrOfDigits % 3));
            if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000 == 1)
            {
                Abreviation = " K";
            }
            else if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000000 == 1)
            {
                Abreviation = " M";
            }
            else if ((Mathf.Pow(10, NrOfDigits - NrOfDigits % 3)) / 1000000000 == 1)
            {
                Abreviation = " B";
            }
        }
        return FirstDigits.ToString("F0") + Abreviation;
    }

    private int GetNumberOfDigits(int value)
    {
        int NrOfDigits = 0;
        while (value > 0)
        {
            value = value / 10;
            NrOfDigits++;
        }
        return NrOfDigits;
    }



    public Chest CreateChest(int type)
    {

        Chest temp = mChests.FirstOrDefault(x => x.mType == type);
        temp.mId = string.Format("CH-{0}-{1}", type, DateTime.UtcNow.Ticks.ToString());

        int ch = PlayerPrefs.GetInt("CreateChest", 0);
        if (type == 0)
        {
            switch (ch)
            {
                case 0:
                    temp.TimerMaxHours = 0;
                    temp.TimerMaxMinutes = 0;
                    temp.TimerMaxSeconds = 30;
                    break;
                case 1:
                    temp.TimerMaxHours = 0;
                    temp.TimerMaxMinutes = 10;
                    temp.TimerMaxSeconds = 0;
                    break;
                case 2:
                    temp.TimerMaxHours = 0;
                    temp.TimerMaxMinutes = 30;
                    temp.TimerMaxSeconds = 0;
                    break;
                case 3:
                    temp.TimerMaxHours = 1;
                    temp.TimerMaxMinutes = 30;
                    temp.TimerMaxSeconds = 0;
                    break;
                default: 
                    temp.TimerMaxHours = 3;
                    temp.TimerMaxMinutes = 0;
                    temp.TimerMaxSeconds = 0;
                    break;

            }
        }
        ch++;
        PlayerPrefs.SetInt("CreateChest", ch);
        PlayerPrefs.Save();
        return temp;
    }

}

[Serializable]
public class Country
{
    public CountryCode countryCode;
    public Sprite countryFlag;
}


#region ENUMS
[Serializable]
public enum DiceType
{
    Classic = 0,
    Palladium = 1,
    Silver = 2,
    Gold = 3,
    Ruthenium = 4,
    Iridium = 5,
    Platinum = 6,
    Rhodium = 7,
};

[Serializable]
public enum PawnType
{
    Classic = 0,
    Palladium = 1,
    Silver = 2,
    Gold = 3,
    Ruthenium = 4,
    Iridium = 5,
    Platinum = 6,
    Rhodium = 7,
};

[Serializable]
public enum GameMode
{
    Classic = 0, Quick = 1, Master = 2
}

#region Offline Mode
[Serializable]
public enum OpponentType
{
    LocalPlayer = 0, Computer = 1
}

/// <summary>
/// Plus 2 In the Index.
/// i.e. <b>(PlayerCount)(index+2)</b>
/// </summary>
[Serializable]
public enum PlayerCount
{
    TwoPlayers = 2, ThreePlayers = 3, FourPlayers = 4
}
#endregion

public enum GameType
{
    TwoPlayer, FourPlayer, Private, Challange
};


public enum EnumPhoton
{
    ReadyToPlay = 179,
    BeginPrivateGame = 171,
    NextPlayerTurn = 172,
    StartWithBots = 173,
    StartGame = 174,
    SendChatMessage = 175,
    SendChatEmojiMessage = 176,
    AddFriend = 177,
    FinishedGame = 178,
    PlayAgain = 180,
    CanNotPlayAgain = 182,
}

public enum EnumGame
{
    DiceRoll = 50,
    PawnMove = 51,
    PawnRemove = 52,
}

public enum ItemType
{
    COINS,
    GEMS,
    CHESTS,
    REMOVE_ADS
}

public enum ColorNames
{
    yellow = 0,
    blue = 1,
    red = 2,
    green = 3
};

public enum InboxEvent
{
    AcceptFriendRequest = 101,
    AcceptGift = 102,
    AcceptGiftRequest = 103,
}

#endregion ENUMS

[Serializable]
public class UserAvatar
{
    public int id = 0;
    public bool isRevealed = false;
    public bool isLocked = true;
    public int price = 15;
    public bool InUse { get { return (id == GameManager.Instance.PlayerData.AvatarIndex); } }

    //public Sprite GetAvatar()
    //{
    //    return StaticDataController.Instance.mConfig.GetAvatar( image ,id);
    //}

    public void GetAvatar(Image image)
    {
         StaticDataController.Instance.mConfig.GetAvatar(image, id);
    }

    public Sprite GetFrame()
    {
        return StaticDataController.Instance.mConfig.GetFrame(GameManager.Instance.PlayerData.AvatarFrameIndex);
    }
}

[Serializable]
public class UserAvatarFrame
{
    public int id = 0;
    public bool isRevealed = false;
    public bool isLocked = true;
    public int price = 10;
    public bool InUse { get { return (id == GameManager.Instance.PlayerData.AvatarFrameIndex); } }

    public Sprite GetFrame()
    {
        return StaticDataController.Instance.mConfig.customAvatarFrames[id];
    }
}

[Serializable]
public class Dice
{
    public int id = 0;
    [Newtonsoft.Json.JsonIgnore]
    public string mName;
    [Newtonsoft.Json.JsonIgnore]
    public DiceType mType;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mBg;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mIcon;
    public int mPiecesCount = 0;
    [Newtonsoft.Json.JsonIgnore]
    public int mPieces = 6;
    [Newtonsoft.Json.JsonIgnore]
    public int mCount { get { return Mathf.FloorToInt(mPiecesCount / mPieces); } }// = 0;
    [Newtonsoft.Json.JsonIgnore]
    public float mBooster = 0.15f;
    public bool IsRevealed { get { return (mCount > 0); } }
    [Newtonsoft.Json.JsonIgnore]
    public bool isLocked { get { return (mCount <= 0); } }
    [Newtonsoft.Json.JsonIgnore]
    public bool InUse { get { return (id == GameManager.Instance.PlayerData.DiceIndex); } }
    [Header("Params for each Side")]
    [Newtonsoft.Json.JsonIgnore]
    public DiceSide[] mSides;
    [Newtonsoft.Json.JsonIgnore]
    public int mPiecesPrice { get { return (50 + (5 * (int)mType)); } }

}


[Serializable]
public class DiceSide : System.Object
{
    [Tooltip("Image of value side")]
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mValueSprite;

    [Tooltip("Image of animation side")]
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mAnimSprite;

    [Tooltip("Value of Side")]
    public int mValue = 1;

    [Tooltip("Chance that this Side will be randomly selected")]
    [RangeAttribute(0, 100)]
    public int Probability = 100;
}



[Serializable]
public class Pawn
{
    public int id = 0;
    public string mName;
    public PawnType mType;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mBg;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mIcon;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite[] mColors;//yellow/blue/red/green
    public int mPiecesCount = 0;
    public int mPieces = 4;
    public int mCount { get { return Mathf.FloorToInt(mPiecesCount / mPieces); } }// = 0;    
    public bool IsRevealed { get { return (mCount > 0); } }
    public bool isLocked { get { return (mCount <= 0); } }
    public bool InUse { get { return (id == GameManager.Instance.PlayerData.PawnIndex); } }
    public int mColor = 0;
    public Sprite getSprite(int color) { return mColors[color]; }// { get { return (mColors[mColor]); } }
    public int mPiecesPrice { get { return (30 + (5 * (int)mType)); } }

}

[Serializable]
public class DicePice
{
    public int mId = 0;
    public bool isRevealed = false;
}

[Serializable]
public class BidCard
{
    public int mId = 0;
    public string mName;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite logo;
    public int entryFee;
}

[Serializable]
public class Opponent
{
    public string mId = "";
    //public string mName;
    //public int mRank;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mAvatar;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite mFrame;
    //public int mDice;
    //public int mPawn;
    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<string, UserDataRecord> data = new Dictionary<string, UserDataRecord>();


    public bool isBot() {
        return mId.Contains("_BOT");
    }
}

[Serializable]
public class InAppItem
{
    public string mId = "";
    public string mName;
    public ProductType productType;
    public string mPrice;
    public int mQuantity;
    public ItemType mItemType;
    public string mStore="GP";

}

[Serializable]
public class PlayerFriend
{
    public string mId = "";
    public string mName;
    public string FacebookId;
    public bool isOnline;
    public int mAvatar;
    public string mAvatarUrl;
    public int mFrame;
    public int mRank;
    public DateTime LastSeen;
    [Newtonsoft.Json.JsonIgnore]
    public DateTime LastStatusUpdateTime = DateTime.Now;
    [Newtonsoft.Json.JsonIgnore]
    public DateTime LastGiftSentTime = DateTime.Now.AddHours(-24);
    [Newtonsoft.Json.JsonIgnore]
    public DateTime LastGiftRequestSentTime = DateTime.Now.AddHours(-24);
    [Newtonsoft.Json.JsonIgnore]
    public Dictionary<string, UserDataRecord> data = new Dictionary<string, UserDataRecord>();
}

[Serializable]
public class Chest
{
    public string mId = "";
    public int mSlot = 0;
    public string mName;
    public int mType = 0;
    public int mState = 0;
    public int mCoins = 0;
    public int mGmes = 0;
    public int mPieces = 8;
    public int TimerMaxHours = 0;
    public int TimerMaxMinutes = 0;
    public int TimerMaxSeconds = 30;
    public int UnLockGems = 20;
    public int BuyGems = 100;

    [Newtonsoft.Json.JsonIgnore]
    public Sprite mIcon;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite[] mOpenAnimation;
    [Newtonsoft.Json.JsonIgnore]
    public Sprite[] mCloseAnimation;

}

[Serializable]
public class InboxMessage
{
    public int Id = 0;
    public string mSender = "";
    public PlayerFriend mFriend;
    public string mMessage = "";
    public InboxEvent Action;
}

[Serializable]
public class Gift
{
    public string SenderId = "";
    public string mTargetId = "";
    public string mSenderName = "";
    public string mTitle = "";
    public string mMessage = "";
    public string mMessageCode;
}
