using UnityEngine;
using UnityEngine.Events;

namespace HK
{
    public partial class Events
    {

        public static Event<int> RequestLoading; // scene index
                                                 //public static Event RequestMainMenu;

        public static Event<bool> ShowUILoader;
        public static Event RequestHome;
        public static Event<int,bool> RequestEquipments;
        public static Event RequestPlayerProfile;
        public static Event<string> RequestPlayerInfo;
        public static Event<string> RequestOpponentProfile;
        public static Event RequestSettings;
        public static Event RequestShop;
        public static Event RequestUpdateUI;
        public static Event RequestProfileUpdateUI;
        public static Event<bool> RequestCurrencyUpdateUI;
        public static Event RequestGameConfig;
        public static Event<int> RequestPrivateConfig;
        public static Event<string> RequestGameMatch;
        public static Event<Opponent> RequestOpponentJoined;
        public static Event<Opponent> RequestOpponentDisconnected;
        public static Event RequestStartGame;
        public static Event<string> RequestUpdateRoomCode;
        public static Event RequestDailyRewards;
        public static Event<int, int,int> RequestItemLocked;
        public static Event<string> RequestToast;
        public static Event RequestFreeRewards;
        public static Event ClickSound;
        public static Event RequesOfflineMode;
        public static Event<RectTransform, int, UnityAction> RequesCoinsColectionAnimation;
        public static Event<RectTransform, int, UnityAction> RequesGemsColectionAnimation;
        public static Event<string, string, UnityAction, bool> RequestConfirmPopup;
        public static Event<string, string> RequestAlertPopup;
        public static Event<string, string, UnityAction> RequestNotEnoughCoinsPopup;
        public static Event<string, string, UnityAction> RequestNotEnoughGemsPopup;
        public static Event <GameObject, GameObject, GameObject> RequestChatPopup;
        public static Event<bool> RequestBanner;
        public static Event<string, UnityAction> RequestRewardedVideo;
        public static Event RequestExitGame;
        public static Event RequestFriends;
        public static Event RequestUpdateFriends;
        public static Event RequestRemoveAds;
        public static Event <UnityAction> RequestLevelUp;
        public static Event<Chest> RequestOpenChest;
        public static Event RequestChestsUpdate;
        public static Event<Chest> RequestInfoChest;
        public static Event<PlayerFriend> RequestOnlineNotification;
        public static Event<PlayerFriend,int,GameMode> RequestChallengeNotification;
        public static Event<PlayerFriend, string,int, GameMode> RequestChallengeAcceptedNotification;
        public static Event<PlayerFriend, string, int, GameMode> RequestPlayNowNotification;
        public static Event<PlayerFriend> RequestFriendNotification;
        public static Event<PlayerController> RequestPlayAgainNotification;
        public static Event<PlayerFriend,Sprite> RequestChallengeConfig;
        public static Event RequestSendChallenge;
        public static Event<int> RequestSendGift;
        public static Event<string ,bool> RequestCollectGift;
        public static Event<string> RequestInvite;
        public static Event<UnityAction> RequestLeaveRoom;
        public static Event<int, int, UnityAction> RequestItemSale;
        public static Event<PlayerFriend, string, GameMode> RequestPrivateGameInvitation;

        public static Event<System.Action> HideConfigurationScreenBanner;
    }
}
