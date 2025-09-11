
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Linq;
using HK;
using Newtonsoft.Json;

public class PlayerData
{
    public Dictionary<string, UserDataRecord> data;

    public int Coins
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.COINS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.COINS_KEY, "10000"));
            else
                return (this.data.ContainsKey(Config.COINS_KEY)) ? int.Parse(this.data[Config.COINS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.COINS_KEY, "10000"));
        }
        set
        {
            UpdateOrAddUserData(Config.COINS_KEY, value.ToString());
            Events.RequestCurrencyUpdateUI.Call(true);
        }
    }

    public int Gems
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.GEMS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.GEMS_KEY, "10"));
            else
                return (this.data.ContainsKey(Config.GEMS_KEY)) ? int.Parse(this.data[Config.GEMS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.GEMS_KEY, "10"));
        }
        set
        {
            UpdateOrAddUserData(Config.GEMS_KEY, value.ToString());
            Events.RequestCurrencyUpdateUI.Call(true);
        }
    }

    public int Spins
    {
        get
        {
            try
            {


                if (PlayerPrefs.HasKey(Config.FORTUNE_WHEEL_SPIN_KEY))
                    return int.Parse(PlayerPrefs.GetString(Config.FORTUNE_WHEEL_SPIN_KEY, "0"));
                else
                    return (this.data.ContainsKey(Config.FORTUNE_WHEEL_SPIN_KEY)) ? int.Parse(this.data[Config.FORTUNE_WHEEL_SPIN_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.FORTUNE_WHEEL_SPIN_KEY, "0"));
            }
            catch (Exception)
            {
                return 1;
            }
        }

        set
        {
            try
            {

                UpdateOrAddUserData(Config.FORTUNE_WHEEL_SPIN_KEY, value.ToString());
                UpdateLiveUserData(() =>
                {

                });
            }
            catch (Exception)
            {
            }
        }
    }

    public int TotalEarnings
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.TOTAL_EARNINGS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.TOTAL_EARNINGS_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.TOTAL_EARNINGS_KEY)) ? int.Parse(this.data[Config.TOTAL_EARNINGS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.TOTAL_EARNINGS_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.TOTAL_EARNINGS_KEY, value.ToString());
        }
    }


    public int TotalWins
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.TOTAL_WINS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.TOTAL_WINS_KEY, (TwoPlayerWins + FourPlayerWins).ToString()));
            else
                return (this.data.ContainsKey(Config.TOTAL_WINS_KEY)) ? int.Parse(this.data[Config.TOTAL_WINS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.TOTAL_WINS_KEY, (TwoPlayerWins + FourPlayerWins).ToString()));
        }
        set
        {
            UpdateOrAddUserData(Config.TOTAL_WINS_KEY, value.ToString());
        }
    }


    public int TwoPlayerWins
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.TWO_PLAYER_WINS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.TWO_PLAYER_WINS_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.TWO_PLAYER_WINS_KEY)) ? int.Parse(this.data[Config.TWO_PLAYER_WINS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.TWO_PLAYER_WINS_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.TWO_PLAYER_WINS_KEY, value.ToString());
        }
    }

    public int FourPlayerWins
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.FOUR_PLAYER_WINS_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.FOUR_PLAYER_WINS_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.FOUR_PLAYER_WINS_KEY)) ? int.Parse(this.data[Config.FOUR_PLAYER_WINS_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.FOUR_PLAYER_WINS_KEY, "0"));
        }

        set
        {
            UpdateOrAddUserData(Config.FOUR_PLAYER_WINS_KEY, value.ToString());
        }
    }

    public int PlayedGames
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.GAMES_PLAYED_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.GAMES_PLAYED_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.GAMES_PLAYED_KEY)) ? int.Parse(this.data[Config.GAMES_PLAYED_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.GAMES_PLAYED_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.GAMES_PLAYED_KEY, value.ToString());
        }
    }



    public int AvatarIndex
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.AVATAR_INDEX_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.AVATAR_INDEX_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.AVATAR_INDEX_KEY)) ? int.Parse(this.data[Config.AVATAR_INDEX_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.AVATAR_INDEX_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.AVATAR_INDEX_KEY, value.ToString());
        }
    }

    public int AvatarFrameIndex
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.AVATAR_FRAME_INDEX_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.AVATAR_FRAME_INDEX_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY)) ? int.Parse(this.data[Config.AVATAR_FRAME_INDEX_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.AVATAR_FRAME_INDEX_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.AVATAR_FRAME_INDEX_KEY, value.ToString());
        }
    }

    public string AvatarUrl
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_AVATAR_URL_KEY))
                return PlayerPrefs.GetString(Config.PLAYER_AVATAR_URL_KEY, "");
            else
                return (this.data.ContainsKey(Config.PLAYER_AVATAR_URL_KEY)) ? this.data[Config.PLAYER_AVATAR_URL_KEY].Value : PlayerPrefs.GetString(Config.PLAYER_AVATAR_URL_KEY, "");
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_AVATAR_URL_KEY, value.ToString());
        }
    }


    List<UserAvatar> userAvatars = new List<UserAvatar>();
    public List<UserAvatar> AvatarsList
    {
        get
        {
            //userAvatars.Clear();
            if (PlayerPrefs.HasKey(Config.AVATARS_KEY))
            {
                userAvatars = JsonConvert.DeserializeObject<List<UserAvatar>>(PlayerPrefs.GetString(Config.AVATARS_KEY));
            }
            else if (this.data.ContainsKey(Config.AVATARS_KEY))
            {
                userAvatars = JsonConvert.DeserializeObject<List<UserAvatar>>(this.data[Config.AVATARS_KEY].Value);
            }

            else
            {

                for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatars.Count; i++)
                {
                    UserAvatar userAvatar = new UserAvatar();
                    userAvatar.id = i;
                    if (i <= 3)
                    {
                        userAvatar.isRevealed = true;
                        userAvatar.isLocked = false;
                    }

                    userAvatars.Add(userAvatar);
                }

                string jsonstrning = JsonConvert.SerializeObject(userAvatars);
                UpdateOrAddUserData(Config.AVATARS_KEY, jsonstrning);
                /*
                List<UserAvatar> userAvatars1 = new List<UserAvatar>();
                Debug.Log("this.data[Config.USER_AVATARS_KEY].Value="+ this.data[Config.USER_AVATARS_KEY].Value);
                userAvatars1 = JsonConvert.DeserializeObject<List<UserAvatar>>(this.data[Config.USER_AVATARS_KEY].Value);//this.data[Config.USER_AVATARS_KEY].Value
                Debug.Log("Data updated successfull ");
                */

            }

            if (userAvatars.Count < StaticDataController.Instance.mConfig.customAvatars.Count)
            {

                for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatars.Count; i++)
                {
                    if (userAvatars.Count <= i)
                    {
                        UserAvatar userAvatar = new UserAvatar();
                        userAvatar.id = i;
                        userAvatars.Add(userAvatar);
                    }

                }

                string jsonstrning = JsonConvert.SerializeObject(userAvatars);
                UpdateOrAddUserData(Config.AVATARS_KEY, jsonstrning);
            }


            return userAvatars;
        }
        set
        {
            string jsonstrning = JsonConvert.SerializeObject(value);
            UpdateOrAddUserData(Config.AVATARS_KEY, jsonstrning);
        }
    }

    List<UserAvatarFrame> userAvatarFrames = new List<UserAvatarFrame>();
    public List<UserAvatarFrame> AvatarFramesList
    {
        get
        {
            // userAvatarFrames.Clear();
            if (PlayerPrefs.HasKey(Config.AVATAR_FRAMES_KEY))
            {
                userAvatarFrames = JsonConvert.DeserializeObject<List<UserAvatarFrame>>(PlayerPrefs.GetString(Config.AVATAR_FRAMES_KEY));
            }
            else if (this.data.ContainsKey(Config.AVATAR_FRAMES_KEY))
            {
                userAvatarFrames = JsonConvert.DeserializeObject<List<UserAvatarFrame>>(this.data[Config.AVATAR_FRAMES_KEY].Value);
            }

            else
            {

                for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatarFrames.Count; i++)
                {
                    UserAvatarFrame userFrame = new UserAvatarFrame();
                    userFrame.id = i;
                    if (i <= 3)
                    {
                        userFrame.isRevealed = true;
                        userFrame.isLocked = false;
                    }

                    userAvatarFrames.Add(userFrame);
                }

                string jsonstrning = JsonConvert.SerializeObject(userAvatarFrames);
                UpdateOrAddUserData(Config.AVATAR_FRAMES_KEY, jsonstrning);


            }

            if (userAvatarFrames.Count < StaticDataController.Instance.mConfig.customAvatarFrames.Count)
            {

                for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatarFrames.Count; i++)
                {
                    if (userAvatarFrames.Count <= i)
                    {
                        UserAvatarFrame userFrame = new UserAvatarFrame();
                        userFrame.id = i;
                        userAvatarFrames.Add(userFrame);
                    }

                }

                string jsonstrning = JsonConvert.SerializeObject(userAvatarFrames);
                UpdateOrAddUserData(Config.AVATAR_FRAMES_KEY, jsonstrning);
            }


            return userAvatarFrames;
        }
        set
        {
            string jsonstrning = JsonConvert.SerializeObject(value);
            UpdateOrAddUserData(Config.AVATAR_FRAMES_KEY, jsonstrning);
        }
    }




    public string Chats
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.CHAT_KEY))
                return PlayerPrefs.GetString(Config.CHAT_KEY, "error");
            else
                return (this.data.ContainsKey(Config.CHAT_KEY)) ? this.data[Config.CHAT_KEY].Value : PlayerPrefs.GetString(Config.CHAT_KEY, "error");
        }
        set
        {
            UpdateOrAddUserData(Config.CHAT_KEY, value);
        }
    }

    public string Emoji
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.EMOJI_KEY))
                return PlayerPrefs.GetString(Config.EMOJI_KEY, "error");
            else
                return (this.data.ContainsKey(Config.EMOJI_KEY)) ? this.data[Config.EMOJI_KEY].Value : PlayerPrefs.GetString(Config.EMOJI_KEY, "error");
        }
        set
        {
            UpdateOrAddUserData(Config.EMOJI_KEY, value);
        }
    }

    public string PlayerName
    {

        get
        {
            if (this.data.ContainsKey(Config.PLAYER_NAME_KEY) && !string.IsNullOrEmpty(this.data[Config.PLAYER_NAME_KEY].Value) && this.data[Config.PLAYER_NAME_KEY].Value.Length >= 3)
            {
                return this.data[Config.PLAYER_NAME_KEY].Value;
            }
            else
            {
                var nm = CreateGuestName();
                UpdateOrAddUserData(Config.PLAYER_NAME_KEY, nm);
                return nm;
            }
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_NAME_KEY, value);

        }

    }

    string CreateGuestName()
    {
        string name = "Guest_";
        for (int i = 0; i < 6; i++)
        {
            name += UnityEngine.Random.Range(0, 9);
        }

        if (PlayerPrefs.HasKey(Config.PLAYER_NAME_KEY))
        {
            name = PlayerPrefs.GetString(Config.PLAYER_NAME_KEY, name);
        }
        else
        {

            UpdateOrAddUserData(Config.PLAYER_NAME_KEY, name);
        }
        return name;
    }



    public string LastFortuneTime
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY))
            {
                return PlayerPrefs.GetString(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, DateTime.Now.AddHours(24).Ticks.ToString());
            }
            else if (this.data.ContainsKey(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY))
            {
                return this.data[Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY].Value;

            }
            else
            {
                string date = DateTime.Now.AddHours(24).Ticks.ToString();
                UpdateOrAddUserData(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, date);
                return date;
            }
        }
        set
        {
            UpdateOrAddUserData(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, value);
        }
    }

    public string PlayerID
    {
        get
        {
            return (this.data.ContainsKey(Config.UNIQUE_IDENTIFIER_KEY)) ? this.data[Config.UNIQUE_IDENTIFIER_KEY].Value : StaticDataController.Instance.mConfig.GetIniqueId();
        }
        set
        {
            UpdateOrAddUserData(Config.UNIQUE_IDENTIFIER_KEY, value);
        }
    }

    public int PlayerLevel
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_LEVEL_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.PLAYER_LEVEL_KEY, "1"));
            else
                return (this.data.ContainsKey(Config.PLAYER_LEVEL_KEY)) ? int.Parse(this.data[Config.PLAYER_LEVEL_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.PLAYER_LEVEL_KEY, "1"));
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_LEVEL_KEY, value.ToString());

        }
    }

    public int PlayerXp
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_XP_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.PLAYER_XP_KEY, "1"));
            else
                return (this.data.ContainsKey(Config.PLAYER_XP_KEY)) ? int.Parse(this.data[Config.PLAYER_XP_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.PLAYER_XP_KEY, "1"));
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_XP_KEY, value.ToString());
        }
    }



    public string PlayerCountryFlag
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_COUNTRY_KEY))
                return PlayerPrefs.GetString(Config.PLAYER_COUNTRY_KEY, "");
            else
                return (this.data.ContainsKey(Config.PLAYER_COUNTRY_KEY)) ? this.data[Config.PLAYER_COUNTRY_KEY].Value : PlayerPrefs.GetString(Config.PLAYER_COUNTRY_KEY, "");
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_COUNTRY_KEY, value.ToString());
        }
    }

    public string LoginType
    {
        get
        {
            return (this.data.ContainsKey(Config.LOGIN_TYPE_KEY)) ? this.data[Config.LOGIN_TYPE_KEY].Value : PlayerPrefs.GetString(Config.LOGIN_TYPE_KEY, "NONE");
        }
        set
        {
            UpdateOrAddUserData(Config.LOGIN_TYPE_KEY, value);
        }
    }

    public string FacebookId
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.FACEBOOK_ID_KEY))
                return PlayerPrefs.GetString(Config.FACEBOOK_ID_KEY, string.Empty);
            else
                return (this.data.ContainsKey(Config.FACEBOOK_ID_KEY)) ? this.data[Config.FACEBOOK_ID_KEY].Value : PlayerPrefs.GetString(Config.FACEBOOK_ID_KEY, string.Empty);
        }
        set
        {
            UpdateOrAddUserData(Config.FACEBOOK_ID_KEY, value);
        }
    }

    public string FacebookToken
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.FACEBOOK_TOKEN_KEY))
                return PlayerPrefs.GetString(Config.FACEBOOK_TOKEN_KEY, string.Empty);
            else
                return (this.data.ContainsKey(Config.FACEBOOK_TOKEN_KEY)) ? this.data[Config.FACEBOOK_TOKEN_KEY].Value : PlayerPrefs.GetString(Config.FACEBOOK_TOKEN_KEY, string.Empty);
        }
        set
        {
            UpdateOrAddUserData(Config.FACEBOOK_TOKEN_KEY, value);
        }
    }

    public string AppleToken
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.APPLE_TOKEN_KEY))
                return PlayerPrefs.GetString(Config.APPLE_TOKEN_KEY, string.Empty);
            else
                return (this.data.ContainsKey(Config.APPLE_TOKEN_KEY)) ? this.data[Config.APPLE_TOKEN_KEY].Value : PlayerPrefs.GetString(Config.APPLE_TOKEN_KEY, string.Empty);
        }
        set
        {
            UpdateOrAddUserData(Config.APPLE_TOKEN_KEY, value);
        }
    }

    public string LoginEmail { get { return PlayerPrefs.GetString(Config.EMAIL_ID_KEY, ""); } set { PlayerPrefs.SetString(Config.EMAIL_ID_KEY, value); } }
    public string LoginPassword { get { return PlayerPrefs.GetString(Config.PASSWORD_KEY, ""); } set { PlayerPrefs.SetString(Config.PASSWORD_KEY, value); } }

    public int DiceIndex
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_DICE_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.PLAYER_DICE_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.PLAYER_DICE_KEY)) ? int.Parse(this.data[Config.PLAYER_DICE_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.PLAYER_DICE_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_DICE_KEY, value.ToString());
        }
    }

    List<Dice> dicesList;
    public List<Dice> DicesList
    {
        get
        {
            if (dicesList == null)
            {
                if (PlayerPrefs.HasKey(Config.DICES_KEY))
                {
                    dicesList = JsonConvert.DeserializeObject<List<Dice>>(PlayerPrefs.GetString(Config.DICES_KEY));
                    dicesList.ForEach(d =>
                    {
                        Dice mDice = StaticDataController.Instance.mConfig.customDices.First(x => x.id == d.id);
                        d.mName = mDice.mName;
                        d.mType = mDice.mType;
                        d.mBooster = mDice.mBooster;
                        d.mBg = mDice.mBg;
                        d.mIcon = mDice.mIcon;
                        d.mSides = mDice.mSides;
                        d.mPieces = mDice.mPieces;
                    });

                }

                else if (this.data.ContainsKey(Config.DICES_KEY))
                {
                    dicesList = JsonConvert.DeserializeObject<List<Dice>>(this.data[Config.DICES_KEY].Value);
                    dicesList.ForEach(d =>
                    {
                        Dice mDice = StaticDataController.Instance.mConfig.customDices.First(x => x.id == d.id);
                        d.mName = mDice.mName;
                        d.mType = mDice.mType;
                        d.mBooster = mDice.mBooster;
                        d.mBg = mDice.mBg;
                        d.mIcon = mDice.mIcon;
                        d.mSides = mDice.mSides;
                        d.mPieces = mDice.mPieces;
                    });


                }
                else
                {
                    dicesList = StaticDataController.Instance.mConfig.customDices;
                    UpdateDice();

                }


            }

            if (dicesList.Count < StaticDataController.Instance.mConfig.customDices.Count)
            {
                StaticDataController.Instance.mConfig.customDices.ForEach(d =>
                {
                    if (!dicesList.Any(x => x.id == d.id))
                        dicesList.Add(d);
                });

                UpdateDice();
            }

            dicesList.OrderByDescending(x => x.mType);
            return dicesList;
        }

        set
        {
            dicesList = value;
            UpdateDice();
        }
    }

    public void UpdateDice()
    {
        string jsonstrning = JsonConvert.SerializeObject(dicesList);
        UpdateOrAddUserData(Config.DICES_KEY, jsonstrning);
    }


    public int PawnIndex
    {
        get
        {
            if (PlayerPrefs.HasKey(Config.PLAYER_PAWN_KEY))
                return int.Parse(PlayerPrefs.GetString(Config.PLAYER_PAWN_KEY, "0"));
            else
                return (this.data.ContainsKey(Config.PLAYER_PAWN_KEY)) ? int.Parse(this.data[Config.PLAYER_PAWN_KEY].Value) : int.Parse(PlayerPrefs.GetString(Config.PLAYER_PAWN_KEY, "0"));
        }
        set
        {
            UpdateOrAddUserData(Config.PLAYER_PAWN_KEY, value.ToString());
        }
    }

    List<Pawn> pawnsList;
    public List<Pawn> PawnsList
    {
        get
        {
            if (pawnsList == null)
            {
                if (PlayerPrefs.HasKey(Config.PAWNS_KEY))
                {
                    pawnsList = JsonConvert.DeserializeObject<List<Pawn>>(PlayerPrefs.GetString(Config.PAWNS_KEY));

                    pawnsList.ForEach(d =>
                    {
                        var mPawn = StaticDataController.Instance.mConfig.customPawns.First(x => x.id == d.id);
                        if (mPawn.id == 0 && d.mPiecesCount < mPawn.mPieces)
                            d.mPiecesCount = mPawn.mPieces;
                        d.mBg = mPawn.mBg;
                        d.mIcon = mPawn.mIcon;
                        d.mName = mPawn.mName;
                        d.mColors = mPawn.mColors;
                        d.mPieces = mPawn.mPieces;
                    });

                }
                else if (this.data.ContainsKey(Config.PAWNS_KEY))
                {
                    pawnsList = JsonConvert.DeserializeObject<List<Pawn>>(this.data[Config.PAWNS_KEY].Value);

                    pawnsList.ForEach(d =>
                    {
                        var mPawn = StaticDataController.Instance.mConfig.customPawns.First(x => x.id == d.id);
                        if (mPawn.id == 0 && d.mPiecesCount < mPawn.mPieces)
                            d.mPiecesCount = mPawn.mPieces;
                        d.mBg = mPawn.mBg;
                        d.mIcon = mPawn.mIcon;
                        d.mName = mPawn.mName;
                        d.mColors = mPawn.mColors;
                        d.mPieces = mPawn.mPieces;
                    });

                }
                else
                {
                    pawnsList = StaticDataController.Instance.mConfig.customPawns;
                    UpdatePawn();
                }
            }

            if (pawnsList.Count < StaticDataController.Instance.mConfig.customPawns.Count)
            {
                StaticDataController.Instance.mConfig.customPawns.ForEach(d =>
                {
                    if (!pawnsList.Any(x => x.id == d.id))
                        pawnsList.Add(d);
                });

                UpdateDice();
            }

            pawnsList.OrderByDescending(x => x.mType);
            return pawnsList;

        }
    }

    public void UpdatePawn()
    {
        string jsonstrning = JsonConvert.SerializeObject(pawnsList);
        UpdateOrAddUserData(Config.PAWNS_KEY, jsonstrning);
    }

    List<Chest> mChestList = new List<Chest>();
    public List<Chest> ChestList
    {
        get
        {
            mChestList = JsonConvert.DeserializeObject<List<Chest>>(PlayerPrefs.GetString(Config.CHESTS_KEY, "[]")); ;
            return mChestList;
        }

        set
        {
            mChestList = value;
            UpdateOrAddUserData(Config.CHESTS_KEY, JsonConvert.SerializeObject(value));
        }
    }



    public void AddToChests(Chest chest)
    {
        if (mChestList.Count < 4)
        {
            if (mChestList.Count <= 0)
                chest.mSlot = 0;
            else
            {



                for (int i = 0; i < 4; i++)
                {
                    var tmp = mChestList.FirstOrDefault(x => x.mSlot == i);
                    if (tmp == null)
                    {
                        chest.mSlot = i;
                        break;
                    }
                }
            }

            mChestList.Add(chest);
            ChestList = mChestList;

        }
    }

    public void UpdateChests(Chest chest)
    {
        mChestList.RemoveAll(c => c.mId == chest.mId);
        mChestList.Add(chest);
        ChestList = mChestList;

        //chestList.ForEach(x =>
        //{
        //    if (x.mId == chest.mId)
        //    {
        //        x.mIcon = null;
        //        x.mState = chest.mState;
        //        x.TimerMaxHours = chest.TimerMaxHours;
        //        x.TimerMaxMinutes = chest.TimerMaxMinutes;
        //        x.TimerMaxSeconds = chest.TimerMaxSeconds;
        //    }

        //});
        //string jsonstrning = JsonConvert.SerializeObject(chestList);
        //UpdateOrAddUserData(Config.CHESTS_KEY, jsonstrning);

    }

    public void RemoveChest(Chest chest)
    {

        ChestList = mChestList.FindAll(x => x.mId != chest.mId);
        PlayerPrefs.DeleteKey(chest.mId);
    }


    public string GetKeyValue(string _key)
    {
        return (this.data.ContainsKey(_key)) ? this.data[_key].Value : PlayerPrefs.GetString(_key, "");
    }

    public void SetKeyValue(string _key, string value)
    {
        UpdateOrAddUserData(_key, value);
    }

    public List<PlayerFriend> mFriendsList;

    public List<InboxMessage> mMessageList = new List<InboxMessage>();

    public PlayerData()
    {
        if (this.data == null)
            this.data = new Dictionary<string, UserDataRecord>();
    }

    public PlayerData(Dictionary<string, UserDataRecord> data, bool myData)
    {
        this.data = data;
    }



    //UpdateUserDataRequest request, Action<UpdateUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null
    public class RequestObject
    {
        public UpdateUserDataRequest request;
        public Action<UpdateUserDataResult> resultCallback;
        public Action<PlayFabError> errorCallback;

        public RequestObject(UpdateUserDataRequest request, Action<UpdateUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
        {
            this.request = request;
            this.resultCallback = resultCallback;
            this.errorCallback = errorCallback;
        }
    }

    public static bool ApiCallInProgress = false;
    // Queue<RequestObject> RequestQuoue = new Queue<RequestObject>();

    float requestNumber = 1;
    public IEnumerator SendOneUpdateRequest(RequestObject RequestCollection)
    {


        // yield return new WaitUntil(()=>ApiCallInProgress != true);
        while (ApiCallInProgress)
        {
            yield return new WaitForEndOfFrame();
        }

        //yield return new WaitForSeconds(requestNumber);
        //requestNumber ++;




        var item = RequestCollection;
        PlayFabClientAPI.UpdateUserData(item.request, item.resultCallback, item.errorCallback, null);


        yield return new WaitForEndOfFrame();
    }


    public void UpdateOrAddUserData(Dictionary<string, string> data)
    {

        if (this.data != null)
            foreach (var item in data)
            {
                if (!this.data.ContainsKey(item.Key))
                {
                    this.data.Add(item.Key, new UserDataRecord());
                }

                this.data[item.Key].Value = item.Value;

            }


        //UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
        //{
        //    Data = data,
        //    Permission = UserDataPermission.Public
        //};

        //RequestQuoue.Enqueue(new RequestObject(
        //    userDataRequest, (result1) =>
        //        {
        //            Debug.Log("Data updated successfull ");
        //            ApiCallInProgress = false;
        //        }, (error1) =>
        //        {
        //            Debug.Log("Data updated error " + error1.ErrorMessage);
        //            ApiCallInProgress = false;
        //        }
        //));




    }

    public void UpdateLiveUserData(UnityAction OnDone)
    {
        if (!GameManager.Instance.IsConnected)
            return;
        PlayFabManager.Instance.StartCoroutine(updateLiveDataAsync(OnDone));


    }

    bool updateProcessOnGoing = false;
    IEnumerator updateLiveDataAsync(UnityAction OnDone)
    {

        yield return new WaitUntil(() => updateProcessOnGoing == false);
        updateProcessOnGoing = true;

        if (this.data != null)
        {
            //UpdateUserDataRequest userDataRequest;
            //Queue<RequestObject> RequestQuoue = new Queue<RequestObject>();

            int Sent = 0;
            while (this.data.Count > Sent)
            {

                var chunk = this.data.Skip(Sent).Take(10).ToList();

                Dictionary<string, string> sendingData = new Dictionary<string, string>();
                foreach (var item in chunk)
                {
                    sendingData.Add(item.Key, item.Value.Value);
                }
                var userDataRequest = new UpdateUserDataRequest()
                {
                    Data = sendingData,
                    Permission = UserDataPermission.Public
                };
                var objReq = new RequestObject(
                       userDataRequest, (result1) =>
                       {
                           // Debug.Log("Data updated successfull ");
                           ApiCallInProgress = false;
                       }, (error1) =>
                       {
                           Debug.Log("Data updated error " + error1.ErrorMessage);
                           ApiCallInProgress = false;
                       }
                   );


                if (GameManager.Instance.IsConnected && !GameManager.Instance.OfflineMode)
                {
                    yield return SendOneUpdateRequest(objReq);

                    Sent += chunk.Count;
                }
                else
                    yield return new WaitForSeconds(3);
            }


            //foreach (var item in data)
            //{
            //    Debug.Log("SAVE: " + item.Key);

            //    Dictionary<string, string> data = new Dictionary<string, string>();
            //    data.Add(item.Key, item.Value.Value);
            //    userDataRequest = new UpdateUserDataRequest()
            //    {
            //        Data = data,
            //        Permission = UserDataPermission.Public
            //    };
            //    RequestQuoue.Enqueue(new RequestObject(
            //       userDataRequest, (result1) =>
            //       {
            //           Debug.Log("Data updated successfull ");
            //           ApiCallInProgress = false;
            //       }, (error1) =>
            //       {
            //           Debug.Log("Data updated error " + error1.ErrorMessage);
            //           ApiCallInProgress = false;
            //       }
            //   ));

            //    if (RequestQuoue.Count > 9)
            //    {

            //        while (RequestQuoue.Count > 0)
            //        {
            //            copyQuoue.Enqueue(RequestQuoue.Dequeue());
            //        }

            //        yield return SendOneUpdateRequest(copyQuoue);
            //    }
            //}



            //if (RequestQuoue.Count > 0)
            //{
            //    yield return SendOneUpdateRequest(RequestQuoue);
            //}
        }
        yield return new WaitForEndOfFrame();

        OnDone?.Invoke();

        updateProcessOnGoing = false;
    }


    public void UpdateOrAddUserData(string KEY, string VALUE)
    {
        if (this.data != null)
        {
            if (!this.data.ContainsKey(KEY))
            {
                this.data.Add(KEY, new UserDataRecord());
            }

            this.data[KEY].Value = VALUE;
        }

        PlayerPrefs.SetString(KEY, VALUE);
        PlayerPrefs.Save();
        /*
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateUserData",
            FunctionParameter = new { keyName = KEY, valueString = VALUE },
            GeneratePlayStreamEvent = false,

        }, (result) => {
            Debug.Log(result.FunctionResult);
            Debug.Log("Data updated successfull ");           
        }, (error) => {
            Debug.Log(error.ErrorMessage);
        });
        */

        /*
         Dictionary<string, string> data = new Dictionary<string, string>();
         data.Add(KEY, VALUE);

         UpdateUserDataRequest userDataRequest = new UpdateUserDataRequest()
         {
             Data = data,
             Permission = UserDataPermission.Public
         };

         RequestQuoue.Enqueue(new RequestObject(
            userDataRequest, (result1) =>
            {
                Debug.Log("Data updated successfull ");
                ApiCallInProgress = false;
            }, (error1) =>
            {
                Debug.Log("Data updated error " + error1.ErrorMessage+ "Data=>"+ data.Keys);

                ApiCallInProgress = false;
            }
        ));

         //PlayFabManager.Instance.StartCoroutine(SendOneUpdateRequest());
        */
        // UpdateLiveUserData();
    }

    public void InitialUserData(bool fb)
    {
        if (this.data == null)
            this.data = new Dictionary<string, UserDataRecord>();

        /*
        if (this.data != null)
        {
     
            if (!this.data.ContainsKey(Config.LOGIN_TYPE_KEY))
            {
                this.data.Add(Config.LOGIN_TYPE_KEY, new UserDataRecord());
            }
            this.data[Config.LOGIN_TYPE_KEY].Value = "NONE";

            if (!this.data.ContainsKey(Config.PLAYER_ONLINE_KEY))
            {
                this.data.Add(Config.PLAYER_ONLINE_KEY, new UserDataRecord());
            }
            this.data[Config.PLAYER_ONLINE_KEY].Value = "Online";

            if (!this.data.ContainsKey(Config.PLAYER_NAME_KEY))
            {
                this.data.Add(Config.PLAYER_NAME_KEY, new UserDataRecord());
            }
            this.data[Config.PLAYER_NAME_KEY].Value = CreateGuestName();

            if (!this.data.ContainsKey(Config.TOTAL_EARNINGS_KEY))
            {
                this.data.Add(Config.TOTAL_EARNINGS_KEY, new UserDataRecord());
            }
            this.data[Config.TOTAL_EARNINGS_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.GAMES_PLAYED_KEY))
            {
                this.data.Add(Config.GAMES_PLAYED_KEY, new UserDataRecord());
            }
            this.data[Config.GAMES_PLAYED_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.TWO_PLAYER_WINS_KEY))
            {
                this.data.Add(Config.TWO_PLAYER_WINS_KEY, new UserDataRecord());
            }
            this.data[Config.TWO_PLAYER_WINS_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.FOUR_PLAYER_WINS_KEY))
            {
                this.data.Add(Config.FOUR_PLAYER_WINS_KEY, new UserDataRecord());
            }
            this.data[Config.FOUR_PLAYER_WINS_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.COINS_KEY))
            {
                this.data.Add(Config.COINS_KEY, new UserDataRecord());
            }
            this.data[Config.COINS_KEY].Value = "500";

            if (!this.data.ContainsKey(Config.GEMS_KEY))
            {
                this.data.Add(Config.GEMS_KEY, new UserDataRecord());
            }
            this.data[Config.GEMS_KEY].Value = "10";

            if (!this.data.ContainsKey(Config.AVATAR_INDEX_KEY))
            {
                this.data.Add(Config.AVATAR_INDEX_KEY, new UserDataRecord());
            }
            if(fb)
                this.data[Config.AVATAR_INDEX_KEY].Value = "-1";
            else
                this.data[Config.AVATAR_INDEX_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.AVATAR_FRAME_INDEX_KEY))
            {
                this.data.Add(Config.AVATAR_FRAME_INDEX_KEY, new UserDataRecord());
            }
            this.data[Config.AVATAR_FRAME_INDEX_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY))
            {
                this.data.Add(Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY, new UserDataRecord());
            }
            this.data[Config.FORTUNE_WHEEL_LAST_FREE_TIME_KEY].Value = DateTime.Now.Ticks.ToString(); 

            if (!this.data.ContainsKey(Config.FORTUNE_WHEEL_SPIN_KEY))
            {
                this.data.Add(Config.FORTUNE_WHEEL_SPIN_KEY, new UserDataRecord());
            }
            this.data[Config.FORTUNE_WHEEL_SPIN_KEY].Value = "0";

            if (!this.data.ContainsKey(Config.FACEBOOK_ID_KEY))
            {
                this.data.Add(Config.FACEBOOK_ID_KEY, new UserDataRecord());
            }
            this.data[Config.FACEBOOK_ID_KEY].Value = "";

            if (!this.data.ContainsKey(Config.FACEBOOK_TOKEN_KEY))
            {
                this.data.Add(Config.FACEBOOK_TOKEN_KEY, new UserDataRecord());
            }
            this.data[Config.FACEBOOK_TOKEN_KEY].Value = "";

            if (!this.data.ContainsKey(Config.PLAYER_AVATAR_URL_KEY))
            {
                this.data.Add(Config.PLAYER_AVATAR_URL_KEY, new UserDataRecord());
            }
            this.data[Config.PLAYER_AVATAR_URL_KEY].Value = "";

            if (!this.data.ContainsKey(Config.UNIQUE_IDENTIFIER_KEY))
            {
                this.data.Add(Config.UNIQUE_IDENTIFIER_KEY, new UserDataRecord());
            }
            this.data[Config.UNIQUE_IDENTIFIER_KEY].Value = StaticDataController.Instance.mConfig.GetIniqueId();
        */

        /*
             SOUND_VOLUME_KEY = "SOUND_VOLUME";
             MUSIC_VOLUME_KEY = "MUSIC_VOLUME";
             VIBRATION_KEY = "VIBRATION";
             NOTIFICATIONS_KEY = "NOTIFICATIONS";
             FRIEND_REQUEST_KEY = "FRIEND_REQUEST";
             PRIVATE_ROOM_KEY = "PRIVATE_ROOM";
             LANGUAGE_KEY = "LANGUAGE";
             REMOVE_ADS_KEY = "REMOVE_ADS";
             AVATARS_KEY = "AVATARS";
             AVATAR_FRAMES_KEY = "AVATAR_FRAMES";
             PLAYER_ID_KEY = "PLAYER_ID";
             PLAYER_LEVEL_KEY = "PLAYER_LEVEL";
             PLAYER_XP_KEY = "PLAYER_XP";
             PLAYER_COUNTRY_KEY = "PLAYER_COUNTRY";
             PLAYER_DICE_KEY = "PLAYER_DICE";
             PLAYER_PAWN_KEY = "PLAYER_PAWN";
             DICES_KEY = "DICES";
             PAWNS_KEY = "PAWNS";

             LAST_FREE_COINS_TIME_NAME = "LastFreeCoinsTimeTicks";
             LAST_AD_COINS_TIME_NAME = "LastAdCoinsTimeTicks";
             LAST_AD_REWARD = "LastAdReward";




                }*/


        Coins = 10000;
        Spins = 0;
        TotalEarnings = 0;
        Chats = "";
        Emoji = "";
        AvatarIndex = 0;
        PawnIndex = 0;
        DiceIndex = 0;
        PlayerLevel = 1;
        PlayedGames = 0;
        TwoPlayerWins = 0;
        FourPlayerWins = 0;
        LastFortuneTime = DateTime.Now.Ticks.ToString();
        /*
        List<UserAvatar>  userAvatars = new List<UserAvatar>();        
        for (int i = 0; i < StaticDataController.Instance.mConfig.customAvatars.LongLength; i++)
        {
            UserAvatar userAvatar = new UserAvatar();
            userAvatar.id = i;
            if (i == 0)
            {
                userAvatar.isRevealed = true;
                userAvatar.isLocked = true;
            }

            userAvatars.Add(userAvatar);
        }

        string jsonText = JsonConvert.SerializeObject(userAvatars);
        
        data.Add(Config.USER_AVATARS_KEY, jsonText);
       
        return data;
         */
        PlayerPrefs.SetString(Config.TITLE_FIRST_LOGIN_KEY, "NO");
        PlayerPrefs.Save();

    }


}

